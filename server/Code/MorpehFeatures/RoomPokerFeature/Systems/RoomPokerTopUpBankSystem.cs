using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerTopUpBankSystem : ISystem
{
    [Injectable] private Stash<RoomPokerSetBank> _roomPokerSetBank;
    [Injectable] private Stash<RoomPokerBank> _roomPokerBank;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerBank>()
            .With<RoomPokerSetBank>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerSetBank = ref _roomPokerSetBank.Get(roomEntity);
            ref var roomPokerBank = ref _roomPokerBank.Get(roomEntity);

            roomPokerBank.Total += roomPokerSetBank.Value;
            
            _roomPokerSetBank.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}