using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerDealingCardsTimerSystem : ISystem
{
    [Injectable] private Stash<RoomPokerDealingTimer> _pokerDealingTimer;
    [Injectable] private Stash<RoomPokerDealingCardsToPlayer> _roomPokerDealingCardsToPlayer;
    [Injectable] private Stash<RoomPokerDealingCardsToPlayerSet> _roomPokerDealingCardsToPlayerSet;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;

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

            ref var roomPokerDealingCardsToPlayer = ref _roomPokerDealingCardsToPlayer.Get(roomEntity);

            if (roomPokerDealingCardsToPlayer.QueuePlayers.Count > 0)
            {
                _roomPokerDealingCardsToPlayerSet.Set(roomEntity);
            }
            else
            {
                ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

                if (roomPokerPlayers.MarkedPlayersBySeat.TryMoveMarker(PokerPlayerMarkerType.ActivePlayer,
                        out var nextPlayerByMarked))
                {
                    _playerSetPokerTurn.Set(nextPlayerByMarked.Value);
                }
            }

            _pokerDealingTimer.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}