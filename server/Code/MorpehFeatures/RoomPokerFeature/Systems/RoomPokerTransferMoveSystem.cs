using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerTransferMoveSystem : ISystem
{
    [Injectable] private Stash<RoomPokerTransferMove> _roomPokerTransferMove;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;
    [Injectable] private Stash<PlayerAway> _playerAway;
    [Injectable] private Stash<PlayerSeat> _playerSeat;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerTransferMove>()
            .With<RoomPokerPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            Logger.DebugColor("call RoomPokerTransferMoveSystem");
            _roomPokerTransferMove.Remove(roomEntity);

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            var startIndexSeat = roomPokerPlayers.MoverSeatPointer;
            var newMoverIndexSeat = startIndexSeat;
            var playerCount = roomPokerPlayers.PlayersBySeat.Length;

            for (var i = 1; i < playerCount; i++)
            {
                var nextIndexSeat = (startIndexSeat + i) % playerCount;
                var nextPlayer = roomPokerPlayers.PlayersBySeat[nextIndexSeat];

                if (!nextPlayer.IsOccupied || _playerAway.Has(nextPlayer.Player))
                {
                    continue;
                }

                newMoverIndexSeat = nextIndexSeat;
                break;
            }

            roomPokerPlayers.MoverSeatPointer = newMoverIndexSeat;
            var moverPlayer = roomPokerPlayers.PlayersBySeat[newMoverIndexSeat].Player;
            
            _playerSetPokerTurn.Set(moverPlayer);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}