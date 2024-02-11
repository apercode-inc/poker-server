using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerDealingCardsTimerSystem : ISystem
{
    [Injectable] private Stash<RoomPokerDealingTimer> _pokerDealingTimer;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerDealingTimer>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var pokerDealingTimer = ref _pokerDealingTimer.Get(roomEntity);

            pokerDealingTimer.Timer -= deltaTime;

            if (pokerDealingTimer.Timer <= 0)
            {
                //TODO раздача окончена игрок начинает ходить...
                Debug.LogError("раздача окончена игрок начинает ходить...");

                _pokerDealingTimer.Remove(roomEntity);
            }
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}