using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CleanupDestroyFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerDestroySystem : ISystem
{
    [Injectable] private Stash<Destroy> _destroy;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerReadyDestroy>()
            .Without<RoomPokerPaidOutToPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            _destroy.Set(roomEntity);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}