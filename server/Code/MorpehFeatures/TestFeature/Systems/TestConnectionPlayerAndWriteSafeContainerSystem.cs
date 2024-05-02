using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.TestFeature.SafeContainers;

namespace server.Code.MorpehFeatures.TestFeature.Systems;

public class TestConnectionPlayerAndWriteSafeContainerSystem : ISystem
{
    [Injectable] private Stash<PlayerNicknameSetDatabaseTest> _playerNicknameSetDatabaseTest;
    
    [Injectable] private ThreadSafeFilter<TestSafeContainer> _safeFilter;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerNicknameSetDatabaseTest>()
            .Build();
    }
    
    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerNicknameSetDatabaseTest = ref _playerNicknameSetDatabaseTest.Get(playerEntity);
            
            _safeFilter.Add(new TestSafeContainer
            {
                Nickname = playerNicknameSetDatabaseTest.Value,
            });
            
            _playerNicknameSetDatabaseTest.Remove(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}