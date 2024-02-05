using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PokerFeature.Components;
using server.Code.MorpehFeatures.PokerFeature.Factories;

namespace server.Code.MorpehFeatures.PokerFeature.Systems;

public class PokerInitializeSystem : ISystem
{
    [Injectable] private Stash<PokerInitialize> _pokerInitialize;
    [Injectable] private Stash<PokerActive> _pokerActive;
    [Injectable] private Stash<PokerCardDesk> _pokerCardDesk;

    [Injectable] private PokerCardDeskFactory _cardDeskFactory;

    private Filter _filter;
    private Filter _testFilter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PokerInitialize>()
            .Build();

        _testFilter = World.Filter
            .With<PokerActive>()
            .With<PokerCardDesk>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            _pokerActive.Set(roomEntity);

            if (!_pokerCardDesk.Has(roomEntity))
            {
                _pokerCardDesk.Set(roomEntity, new PokerCardDesk
                {
                    CardDesk = _cardDeskFactory.CreateCardDeskPokerStandard()
                });
            }

            _pokerInitialize.Remove(roomEntity);
        }
        
        //todo test
        foreach (var entity in _testFilter)
        {
            Debug.Log("покер активен можно играть!!!");
        }
    }

    public void Dispose()
    {
        _filter = null;
        _testFilter = null;
    }
}