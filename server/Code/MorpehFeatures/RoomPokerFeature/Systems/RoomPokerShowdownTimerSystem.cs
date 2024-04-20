using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerShowdownTimerSystem : ISystem
{
    [Injectable] private Stash<RoomPokerShowOrHideCardsActivate> _roomPokerShowOrHideCardsActivate;
    [Injectable] private Stash<RoomPokerShowdownTimer> _roomPokerShowdownTimer;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerShowdownTimer>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerShowdownTimer = ref _roomPokerShowdownTimer.Get(roomEntity);

            roomPokerShowdownTimer.Value -= deltaTime;

            if (roomPokerShowdownTimer.Value > 0)
            {
                continue;
            }
            
            _roomPokerShowOrHideCardsActivate.Set(roomEntity);
            _roomPokerShowdownTimer.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
    }
}