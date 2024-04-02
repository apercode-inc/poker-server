using NetFrame.Server;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;
using server.Code.MorpehFeatures.RoomPokerFeature.Storages;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Services;

public class RoomPokerService : IInitializer
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerId> _roomPokerId;
    [Injectable] private Stash<RoomPokerCardDesk> _roomPokerCardDesk;
    [Injectable] private Stash<RoomPokerPlayersGivenBank> _roomPokerPlayersGivenBank;

    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerDealer> _playerDealer;
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerSeat> _playerSeat;
    [Injectable] private Stash<PlayerPokerContribution> _playerPokerContribution;
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;
    [Injectable] private Stash<PlayerTurnTimer> _playerTurnTimer;
    [Injectable] private Stash<PlayerTurnCompleteFlag> _playerTurnCompleteFlag;

    [Injectable] private NetFrameServer _server;
    [Injectable] private RoomPokerStorage _roomPokerStorage;
    [Injectable] private RoomPokerCardDeskService _roomPokerCardDeskService;

    public World World { get; set; }

    private Dictionary<PokerPlayerMarkerType, Entity> _markersByPlayer;

    public void OnAwake()
    {
        _markersByPlayer = new Dictionary<PokerPlayerMarkerType, Entity>();
    }

    public void RemovePlayerFromRoom(Entity roomEntity, Entity playerLeft)
    {
        ref var roomPokerId = ref _roomPokerId.Get(roomEntity);
        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
        var markedPlayersBySeat = roomPokerPlayers.MarkedPlayersBySeat;

        _markersByPlayer.Clear();
        _roomPokerCardDeskService.ReturnCardsInDeskToPlayer(roomEntity, playerLeft);
        
        var isRemove = markedPlayersBySeat.Remove(playerLeft, _markersByPlayer);
        var overOnePlayerToTable = markedPlayersBySeat.Count > 1;

        if (isRemove && overOnePlayerToTable)
        {
            foreach (var markerByPlayer in _markersByPlayer)
            {
                var marker = markerByPlayer.Key;
                var nextPlayerMarked = markerByPlayer.Value;
                
                switch (marker)
                {
                    case PokerPlayerMarkerType.DealerPlayer:
                        SetDealerPlayerMarker(roomEntity, nextPlayerMarked);
                        break;
                    case PokerPlayerMarkerType.ActivePlayer:
                        SetActivePlayerMarker(nextPlayerMarked);
                        break;
                    case PokerPlayerMarkerType.NextRoundActivePlayer:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        ref var playerId = ref _playerId.Get(playerLeft);

        var dataframe = new RoomPokerLeftResponseDataframe
        {
            RoomId =  roomPokerId.Value,
            PlayerId = playerId.Id,
        };
        _server.SendInRoom(ref dataframe, roomEntity);

        _playerDealer.Remove(playerLeft);
        _playerCards.Remove(playerLeft);
        _playerRoomPoker.Remove(playerLeft);
        _playerSeat.Remove(playerLeft);
        _playerPokerContribution.Remove(playerLeft);
        _playerPokerCurrentBet.Remove(playerLeft);
        _playerTurnTimer.Remove(playerLeft);
        _playerTurnCompleteFlag.Remove(playerLeft);
        
        if (markedPlayersBySeat.Count != 0)
        {
            return;
        }

        _roomPokerStorage.Remove(roomPokerId.Value);
    }

    public void DropCards(Entity roomEntity, Entity playerEntity)
    {
        _roomPokerCardDeskService.ReturnCardsInDeskToPlayer(roomEntity, playerEntity);

        ref var playerId = ref _playerId.Get(playerEntity);
        ref var playerSeat = ref _playerSeat.Get(playerEntity);
                
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
        var markedPlayersBySeat = roomPokerPlayers.MarkedPlayersBySeat;

        markedPlayersBySeat.TryGetValueByMarkers(playerSeat.SeatIndex, out var playerByMarkers);

        foreach (var marker in playerByMarkers.Markers)
        {
            var isMarked = marker.Value;
            var markerType = marker.Key;

            if (isMarked && markerType == PokerPlayerMarkerType.ActivePlayer)
            {
                if (markedPlayersBySeat.TryMoveMarker(markerType, out var nextPlayerActive))
                {
                    SetActivePlayerMarker(nextPlayerActive.Value);
                }
            }
        }
        
        var withCardsPlayers = new FastList<Entity>();
        
        foreach (var playerMarked in markedPlayersBySeat)
        {
            var player = playerMarked.Value;
            
            ref var playerCards = ref _playerCards.Get(player);

            if (playerCards.CardsState != CardsState.Empty)
            {
                continue;
            }

            withCardsPlayers.Add(player);
        }

        if (withCardsPlayers.length == 1)
        {
            _roomPokerPlayersGivenBank.Set(roomEntity, new RoomPokerPlayersGivenBank
            {
                Players = withCardsPlayers,
            });
        }
    }

    private void SetDealerPlayerMarker(Entity roomEntity, Entity nextMarkedPlayer)
    {
        _playerDealer.Set(nextMarkedPlayer);
        
        ref var playerId = ref _playerId.Get(nextMarkedPlayer);
        
        var dataframe = new RoomPokerSetDealerDataframe
        {
            PlayerId = playerId.Id,
        };
        _server.SendInRoom(ref dataframe, roomEntity);
    }
    
    private void SetActivePlayerMarker(Entity nextMarkedPlayer)
    {
        _playerSetPokerTurn.Set(nextMarkedPlayer);
    }

    public void Dispose()
    {
        _markersByPlayer = null;
    }
}