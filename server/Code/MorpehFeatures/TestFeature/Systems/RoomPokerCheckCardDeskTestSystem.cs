using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.TestFeature.Systems;

public class RoomPokerCheckCardDeskTestSystem : ISystem
{
    [Injectable] private Stash<RoomPokerCardDesk> _roomPokerCardDesk;
    
    private float _timer;
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerCardDesk>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        _timer += deltaTime;

        if (_timer < 5)
        {
            return;
        }

        _timer = 0;
        
        foreach (var entity in _filter)
        {
            ref var roomPokerCardDesk = ref _roomPokerCardDesk.Get(entity);
            
            Logger.Debug($"cards count = {roomPokerCardDesk.CardDesk.Count}");
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}
