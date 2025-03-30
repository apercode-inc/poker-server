using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerSetCardsTickTimerAndNextStateTableSystem : ISystem
{
    [Injectable] private Stash<RoomPokerSetCardsTickTimer> _roomPokerSetCardsTickTimer;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    
    [Injectable] private Stash<PlayerTurnCompleteFlag> _playerTurnCompleteFlag;
    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;
    [Injectable] private Stash<PlayerAway> _playerAway;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerId>()
            .With<RoomPokerSetCardsTickTimer>()
            .With<RoomPokerPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerSetCardsTickTimer = ref _roomPokerSetCardsTickTimer.Get(roomEntity);

            roomPokerSetCardsTickTimer.Value -= deltaTime;

            if (roomPokerSetCardsTickTimer.Value > 0)
            {
                continue;
            }

            _roomPokerSetCardsTickTimer.Remove(roomEntity);

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            foreach (var player in roomPokerPlayers.PlayersBySeat)
            {
                if (player.IsNullOrDisposed())
                {
                    continue;
                }
                _playerTurnCompleteFlag.Remove(player);
            }
            
            var startIndexSeat = roomPokerPlayers.DealerSeatPointer;
            var nextMoverIndexSeat = startIndexSeat;
            var playerCount = roomPokerPlayers.PlayersBySeat.Length;

            for (var i = 1; i < playerCount; i++)
            {
                var nextIndexSeat = (startIndexSeat + i) % playerCount;
                var nextPlayer = roomPokerPlayers.PlayersBySeat[nextIndexSeat];

                if (nextPlayer.IsNullOrDisposed() || _playerAway.Has(nextPlayer))
                {
                    continue;
                }

                nextMoverIndexSeat = nextIndexSeat;
                break;
            }
            var nextMoverPlayer = roomPokerPlayers.PlayersBySeat[nextMoverIndexSeat];
            roomPokerPlayers.MoverSeatPointer = nextMoverIndexSeat;
            
            _playerSetPokerTurn.Set(nextMoverPlayer);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}