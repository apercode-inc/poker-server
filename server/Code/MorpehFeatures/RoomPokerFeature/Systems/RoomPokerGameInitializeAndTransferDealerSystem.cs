using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;
using server.Code.MorpehFeatures.RoomPokerFeature.Services;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

//todo может переименовать надо по нормальному, система выполняет роль инициализации и роль передвижения диллера
public class RoomPokerGameInitializeAndTransferDealerSystem : ISystem
{
    [Injectable] private Stash<RoomPokerGameInitialize> _roomPokerGameInitialize;
    [Injectable] private Stash<RoomPokerActive> _roomPokerActive;
    [Injectable] private Stash<RoomPokerBank> _roomPokerBank;
    [Injectable] private Stash<RoomPokerCardDesk> _roomPokerCardDesk;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerDealingCardsToPlayer> _roomPokerDealingCardsToPlayer;
    [Injectable] private Stash<RoomPokerDealingCardsToPlayerSet> _roomPokerDealingCardsToPlayerSet;
    [Injectable] private Stash<RoomPokerSetBlinds> _roomPokerSetBlinds;
    [Injectable] private Stash<RoomPokerCleanedGame> _roomPokerCleanedGame;
    
    [Injectable] private Stash<PlayerDealer> _playerDealer;
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    [Injectable] private Stash<PlayerNickname> _playerNickname;
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerAway> _playerAway;

    [Injectable] private RoomPokerCardDeskService _cardDeskService;
    [Injectable] private RoomPokerService _roomPokerService;
    
    [Injectable] private NetFrameServer _server;

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerPlayers>()
            .With<RoomPokerGameInitialize>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            _roomPokerGameInitialize.Remove(roomEntity);
            
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            if (roomPokerPlayers.TotalPlayersCount <= 1)
            {
                continue;
            }
            
            if (!_roomPokerCardDesk.Has(roomEntity))
            {
                _roomPokerCardDesk.Set(roomEntity, new RoomPokerCardDesk
                {
                    CardDesk = _cardDeskService.CreateCardDeskPokerStandard()
                });
            }
            else
            {
                ref var roomPokerCardDesk = ref _roomPokerCardDesk.Get(roomEntity);

                _cardDeskService.FillTheDesk(roomPokerCardDesk.CardDesk);
            }

            var dealerPlayer = MoveDealerSeatPointer(ref roomPokerPlayers);
            SetDealerPlayerMarker(roomEntity, dealerPlayer);

            roomPokerPlayers.PlayerPotModels.Clear();
            var playersEntities = new Queue<Entity>();

            foreach (var playerBySeat in roomPokerPlayers.PlayersBySeat)
            {
                if (playerBySeat.Player.IsNullOrDisposed())
                {
                    continue;
                }
                
                var playerEntity = playerBySeat.Player;
                
                ref var playerAuthData = ref _playerAuthData.Get(playerEntity);
                ref var playerNickname = ref _playerNickname.Get(playerEntity);
                
                roomPokerPlayers.PlayerPotModels.Add(new PlayerPotModel(playerAuthData.Guid, playerNickname.Value));

                if (_playerAway.Has(playerEntity))
                {
                    continue;
                }
                
                playersEntities.Enqueue(playerEntity);
            }
            
            _roomPokerSetBlinds.Set(roomEntity, new RoomPokerSetBlinds());
            
            _roomPokerDealingCardsToPlayer.Set(roomEntity, new RoomPokerDealingCardsToPlayer
            {
                QueuePlayers = playersEntities,
            });
            _roomPokerDealingCardsToPlayerSet.Set(roomEntity);
            _roomPokerCleanedGame.Remove(roomEntity);
            
            _roomPokerActive.Set(roomEntity);
            _roomPokerBank.Set(roomEntity);
        }
    }
    
    private Entity MoveDealerSeatPointer(ref RoomPokerPlayers roomPokerPlayers)
    {
        var startIndexSeat = roomPokerPlayers.DealerSeatPointer;
        var newDealerIndexSeat = startIndexSeat;
        var playerCount = roomPokerPlayers.PlayersBySeat.Length;

        for (var i = 1; i < playerCount; i++)
        {
            var nextIndexSeat = (startIndexSeat + i) % playerCount;
            var nextPlayer = roomPokerPlayers.PlayersBySeat[nextIndexSeat];

            if (nextPlayer.Player.IsNullOrDisposed() || _playerAway.Has(nextPlayer.Player))
            {
                continue;
            }

            newDealerIndexSeat = nextIndexSeat;
            break;
        }

        roomPokerPlayers.DealerSeatPointer = newDealerIndexSeat;
        var dealerPlayer = roomPokerPlayers.PlayersBySeat[newDealerIndexSeat].Player;
        return dealerPlayer;
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

    public void Dispose()
    {
        _filter = null;
    }
}