using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCleanupTimerSystem : ISystem
{
    [Injectable] private Stash<RoomPokerCleanupTimer> _roomPokerCleanupTimer;
    [Injectable] private Stash<RoomPokerCleanup> _roomPokerCleanup;
    [Injectable] private Stash<RoomPokerActive> _roomPokerActive;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerCleanupTimer>()
            .With<RoomPokerPlayers>()
            .Without<RoomPokerShowOrHideCards>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerNextDealingTimer = ref _roomPokerCleanupTimer.Get(roomEntity);
            
            if (!_roomPokerActive.Has(roomEntity))
            {
                StartCleanup(roomEntity);
                continue;
            }
            
            roomPokerNextDealingTimer.Value -= deltaTime;
            
            if (roomPokerNextDealingTimer.Value > 0)
            {
                continue;
            }
            
            StartCleanup(roomEntity);
        }
    }

    private void StartCleanup(Entity roomEntity)
    {
        _roomPokerCleanup.Set(roomEntity);
        _roomPokerCleanupTimer.Remove(roomEntity);
    }

    public void Dispose()
    {
        _filter = null;
    }
}