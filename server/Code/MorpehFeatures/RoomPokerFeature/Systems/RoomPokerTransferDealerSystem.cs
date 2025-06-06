using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerTransferDealerSystem : ISystem
{
    [Injectable] private Stash<PlayerDealer> _playerDealer;
    [Injectable] private Stash<PlayerAway> _playerAway;
    [Injectable] private Stash<PlayerId> _playerId;
    
    [Injectable] private Stash<RoomPokerTransferDealer> _roomPokerTransferDealer;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerPlayers>()
            .With<RoomPokerTransferDealer>()
            .Build();
    }
    
    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            _roomPokerTransferDealer.Remove(roomEntity);
            
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
                        
            var dealerPlayer = MoveDealerSeatPointer(ref roomPokerPlayers);
            
            _playerDealer.Set(dealerPlayer);
        
            ref var playerId = ref _playerId.Get(dealerPlayer);
        
            var dataframe = new RoomPokerSetDealerDataframe
            {
                PlayerId = playerId.Id,
            };
            _server.SendInRoom(ref dataframe, roomEntity);
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

            if (nextPlayer.IsNullOrDisposed() || _playerAway.Has(nextPlayer))
            {
                continue;
            }

            newDealerIndexSeat = nextIndexSeat;
            break;
        }

        roomPokerPlayers.DealerSeatPointer = newDealerIndexSeat;
        return roomPokerPlayers.PlayersBySeat[newDealerIndexSeat];
    }

    public void Dispose()
    {
        _filter = null;
    }
}