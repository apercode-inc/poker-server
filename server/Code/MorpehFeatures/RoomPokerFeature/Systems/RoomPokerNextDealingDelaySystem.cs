using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerNextDealingDelaySystem : ISystem
{
    [Injectable] private Stash<RoomPokerNextDealingTimer> _roomPokerNextDealingTimer;
    [Injectable] private Stash<RoomPokerGameInitialize> _roomPokerGameInitialize;
    [Injectable] private Stash<RoomPokerActive> _roomPokerActive;
    [Injectable] private Stash<RoomPokerCombinationMax> _roomPokerCombinationMax;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerNextDealingTimer>()
            .With<RoomPokerPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            if (!_roomPokerActive.Has(roomEntity))
            {
                _roomPokerNextDealingTimer.Remove(roomEntity);
            }
            
            ref var roomPokerNextDealingTimer = ref _roomPokerNextDealingTimer.Get(roomEntity);
            roomPokerNextDealingTimer.Value -= deltaTime;
            
            if (roomPokerNextDealingTimer.Value > 0)
            {
                continue;
            }
            
            _roomPokerNextDealingTimer.Remove(roomEntity);
            _roomPokerGameInitialize.Set(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}