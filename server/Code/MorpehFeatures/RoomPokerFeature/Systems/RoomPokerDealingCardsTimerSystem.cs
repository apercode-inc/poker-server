using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerDealingCardsTimerSystem : ISystem
{
    [Injectable] private Stash<RoomPokerDealingTimer> _pokerDealingTimer;
    [Injectable] private Stash<RoomPokerDealingCardsToPlayer> _roomPokerDealingCardsToPlayer;
    [Injectable] private Stash<RoomPokerDealingCardsToPlayerSet> _roomPokerDealingCardsToPlayerSet;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;
    [Injectable] private Stash<PlayerSeat> _playerSeat;

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerDealingTimer>()
            .With<RoomPokerDealingCardsToPlayer>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var pokerDealingTimer = ref _pokerDealingTimer.Get(roomEntity);

            pokerDealingTimer.Timer -= deltaTime;

            if (pokerDealingTimer.Timer > 0)
            {
                continue;
            }

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            foreach (var playerBySeat in roomPokerPlayers.PlayersBySeat)
            {
                if (playerBySeat.Player.IsNullOrDisposed())
                {
                    continue;
                }

                var player = playerBySeat.Player;

                ref var playerSeat = ref _playerSeat.Get(player);
                
                if (playerSeat.SeatIndex != roomPokerPlayers.MoverSeatPointer)
                {
                    continue;
                }
                
                _playerSetPokerTurn.Set(player);
            }

            _pokerDealingTimer.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}