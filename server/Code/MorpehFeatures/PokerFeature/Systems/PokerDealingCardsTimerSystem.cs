using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PokerFeature.Components;

namespace server.Code.MorpehFeatures.PokerFeature.Systems;

public class PokerDealingCardsTimerSystem : ISystem
{
    [Injectable] private Stash<PokerDealingTimer> _pokerDealingTimer;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PokerDealingTimer>()
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

                _pokerDealingTimer.Remove(roomEntity);
            }
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}