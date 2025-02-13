using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils.CustomCollections;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;
using server.Code.MorpehFeatures.RoomPokerFeature.Storages;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Services;

public class RoomPokerService : IInitializer
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerId> _roomPokerId;
    [Injectable] private Stash<RoomPokerCardDesk> _roomPokerCardDesk;
    [Injectable] private Stash<RoomPokerShowOrHideCardsActivate> _roomPokerShowOrHideCardsActivate;
    [Injectable] private Stash<RoomPokerPayoutWinnings> _roomPokerPayoutWinnings;
    [Injectable] private Stash<RoomPokerOnePlayerRoundGame> _roomPokerOnePlayerRoundGame;
    [Injectable] private Stash<RoomPokerShowdownChoiceCheck> _roomPokerShowdownChoiceCheck;

    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerDealer> _playerDealer;
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerSeat> _playerSeat;
    [Injectable] private Stash<PlayerPokerContribution> _playerPokerContribution;
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;
    [Injectable] private Stash<PlayerTurnTimer> _playerTurnTimer;
    [Injectable] private Stash<PlayerShowOrHideTimer> _playerShowOrHideTimer;
    [Injectable] private Stash<PlayerTurnCompleteFlag> _playerTurnCompleteFlag;
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    [Injectable] private Stash<PlayerAllin> _playerAllin;
    [Injectable] private Stash<PlayerAway> _playerAway;
    [Injectable] private Stash<PlayerTurnShowdownTimer> _playerTurnShowdownTimer;

    [Injectable] private NetFrameServer _server;
    [Injectable] private RoomPokerStorage _roomPokerStorage;
    [Injectable] private RoomPokerCardDeskService _roomPokerCardDeskService;

    public World World { get; set; }

    private Dictionary<PokerPlayerMarkerType, Entity> _markersByPlayer;

    public void OnAwake()
    {
        _markersByPlayer = new Dictionary<PokerPlayerMarkerType, Entity>();
    }

    public void RemovePlayerFromRoom(Entity roomEntity, Entity playerLeave)
    {
        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
        var markedPlayersBySeat = roomPokerPlayers.MarkedPlayersBySeat;
        var awayPlayers = roomPokerPlayers.AwayPlayers;
        var playerPotModels = roomPokerPlayers.PlayerPotModels;

        _markersByPlayer.Clear();
        
        roomPokerPlayers.AwayPlayers.Remove(playerLeave);
        
        var totalPlayersCount = awayPlayers.Count + markedPlayersBySeat.Count;

        SetPlayerFoldForPotModels(playerLeave, playerPotModels);
        RemoveFromMarkedPlayers(roomEntity, playerLeave, markedPlayersBySeat);
        CleanupPlayer(roomEntity, playerLeave, totalPlayersCount);
        
        var dataframe = new RoomPokerLocalPlayerLeaveDataframe();
        _server.Send(ref dataframe, playerLeave);
    }

    public void RemoveForAwayPlayer(Entity roomEntity, Entity awayPlayer)
    {
        _markersByPlayer.Clear();
        
        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
        var markedPlayersBySeat = roomPokerPlayers.MarkedPlayersBySeat;
        
        RemoveFromMarkedPlayers(roomEntity, awayPlayer, markedPlayersBySeat);
    }

    public void DropCards(Entity roomEntity, Entity playerEntity, bool isNextTurn = true)
    {
        ref var playerId = ref _playerId.Get(playerEntity);

        _playerCards.Set(playerEntity, new PlayerCards
        {
            CardsState = CardsState.Empty,
            Cards = null,
        });
                
        var dataframe = new RoomPokerSetCardsByPlayerDataframe
        {
            PlayerId = playerId.Id,
            CardsState = CardsState.Empty,
            Cards = null,
        };
        _server.SendInRoom(ref dataframe, roomEntity);
        
        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

        SetPlayerFoldForPotModels(playerEntity, roomPokerPlayers.PlayerPotModels);
        
        if (!isNextTurn)
        {
            return;
        }
        
        ref var playerSeat = ref _playerSeat.Get(playerEntity);
        
        var markedPlayersBySeat = roomPokerPlayers.MarkedPlayersBySeat;

        markedPlayersBySeat.TryGetValueByMarkers(playerSeat.SeatIndex, out var playerByMarkers);

        foreach (var marker in playerByMarkers.Markers)
        {
            var isMarked = marker.Value;
            var markerType = marker.Key;

            if (isMarked && markerType == PokerPlayerMarkerType.ActivePlayer)
            {
                SetActivePlayerMarkerOrGivenBank(roomEntity);
            }
        }
    }

    public void SetDealerPlayerMarker(Entity roomEntity, Entity nextMarkedPlayer)
    {
        _playerDealer.Set(nextMarkedPlayer);
        
        ref var playerId = ref _playerId.Get(nextMarkedPlayer);
        
        var dataframe = new RoomPokerSetDealerDataframe
        {
            PlayerId = playerId.Id,
        };
        _server.SendInRoom(ref dataframe, roomEntity);
    }

    private void SetPlayerFoldForPotModels(Entity playerEntity, List<PlayerPotModel> playerPotModels)
    {
        ref var playerAuthData = ref _playerAuthData.Get(playerEntity);

        foreach (var playerPotModel in playerPotModels)
        {
            if (playerPotModel.Guid != playerAuthData.Guid)
            {
                continue;
            }
            playerPotModel.SetFold();
            break;
        }
    }

    private void RemoveFromMarkedPlayers(Entity roomEntity, Entity playerLeave, 
        MovingMarkersDictionary<Entity, PokerPlayerMarkerType> markedPlayersBySeat)
    {
        var isRemove = markedPlayersBySeat.Remove(playerLeave, _markersByPlayer);

        if (isRemove)
        {
            foreach (var markerByPlayer in _markersByPlayer)
            {
                var marker = markerByPlayer.Key;
                var nextPlayerMarked = markerByPlayer.Value;

                switch (marker)
                {
                    case PokerPlayerMarkerType.DealerPlayer:
                    {
                        SetDealerPlayerMarker(roomEntity, nextPlayerMarked);
                        break;
                    }
                    case PokerPlayerMarkerType.ActivePlayer:
                    {
                        if (!_playerShowOrHideTimer.Has(playerLeave))
                        {
                            SetActivePlayerMarkerOrGivenBank(roomEntity);
                        }
                        break;
                    }
                    case PokerPlayerMarkerType.NextRoundActivePlayer:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    private void SetActivePlayerMarkerOrGivenBank(Entity roomEntity)
    {
        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
        var markedPlayersBySeat = roomPokerPlayers.MarkedPlayersBySeat;

        if (markedPlayersBySeat.TryMoveMarker(PokerPlayerMarkerType.ActivePlayer, out var nextPlayerActive))
        {
            var player = nextPlayerActive.Value;

            if (!player.IsNullOrDisposed())
            {
                _playerSetPokerTurn.Set(player);
            }
        }
    }
    
    private void CleanupPlayer(Entity roomEntity, Entity playerLeave, int totalPlayersCount)
    {
        if (_playerShowOrHideTimer.Has(playerLeave))
        {
            _roomPokerShowOrHideCardsActivate.Set(roomEntity);
            _playerShowOrHideTimer.Remove(playerLeave);
        }
        
        if (_playerTurnShowdownTimer.Has(playerLeave))
        {
            _playerTurnShowdownTimer.Remove(playerLeave);
            _roomPokerShowdownChoiceCheck.Set(roomEntity);
        }

        ref var playerId = ref _playerId.Get(playerLeave);

        var dataframe = new RoomPokerLeaveResponseDataframe
        {
            PlayerId = playerId.Id,
        };
        _server.SendInRoom(ref dataframe, roomEntity);

        _playerDealer.Remove(playerLeave);
        _playerCards.Remove(playerLeave);
        _playerRoomPoker.Remove(playerLeave);
        _playerSeat.Remove(playerLeave);
        _playerPokerContribution.Remove(playerLeave);
        _playerPokerCurrentBet.Remove(playerLeave);
        _playerTurnTimer.Remove(playerLeave);
        _playerTurnCompleteFlag.Remove(playerLeave);
        _playerAllin.Remove(playerLeave);
        _playerAway.Remove(playerLeave);

        if (totalPlayersCount != 0)
        {
            return;
        }
        
        ref var roomPokerId = ref _roomPokerId.Get(roomEntity);
        
        _roomPokerStorage.Remove(roomPokerId.Value);
    }

    public void Dispose()
    {
        _markersByPlayer = null;
    }
}