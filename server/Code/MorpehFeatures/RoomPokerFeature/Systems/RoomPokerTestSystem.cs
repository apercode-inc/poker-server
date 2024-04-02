using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerTestSystem : ISystem
{
    [Injectable] private Stash<RoomPokerCardsToTable> _roomPokerCardsToTable;
    
    private float _timer;
    
    private Filter _filter;
    
    public World World { get; set; }


    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerCardsToTable>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        _timer += deltaTime;

        if (_timer < 1)
        {
            return;
        }

        _timer = 0;

        foreach (var roomEntity in _filter)
        {
            ref var roomPokerCardsToTable = ref _roomPokerCardsToTable.Get(roomEntity);
            
            //Logger.Error($"state = {roomPokerCardsToTable.State}");
        }
    }

    public void Dispose()
    {
    }
}