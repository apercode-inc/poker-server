using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.DbModels;
using server.Code.MorpehFeatures.TestFeature.SafeContainers;

namespace server.Code.MorpehFeatures.TestFeature.Systems;

public class TestCreateRecordInPlayersTable : ISystem
{
    [Injectable] private Stash<PlayerNicknameSetDatabaseTest> _playerNicknameSetDatabaseTest;
    
    [Injectable] private ThreadSafeFilter<TestSafeContainer> _safeFilter;

    [Injectable] private TestPlayerDbService _testPlayerDbService;
    
    private Random _random;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _random = new Random();
    }
    
    public void OnUpdate(float deltaTime)
    {
        foreach (var container in _safeFilter)
        {
            var playerModel = new DbPlayerModel
            {
                unique_id = Guid.NewGuid().ToString(),
                nickname = container.Nickname,
                level = _random.Next(8, 88),
                experience = _random.Next(100, 10000),
                chips = _random.Next(1000, 1000000),
                gold = _random.Next(10, 100),
                stars = _random.Next(20, 120),
                registration_date = DateTime.UtcNow,
            };
        
            _testPlayerDbService.InsertPlayerThreadPool(playerModel).Forget();
        }
    }

    public void Dispose()
    {
        _random = null;
    }
}