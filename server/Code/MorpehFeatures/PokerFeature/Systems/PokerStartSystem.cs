using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PokerFeature.Components;

namespace server.Code.MorpehFeatures.PokerFeature.Systems;

public class PokerStartSystem : ISystem
{
    [Injectable] private Stash<PokerStart> _pokerStart;
    [Injectable] private Stash<PokerActive> _pokerActive;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PokerStart>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            _pokerActive.Set(roomEntity);
            _pokerStart.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}