using Dapper;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.DataBaseFeature.Interfaces;
using server.Code.MorpehFeatures.DataBaseFeature.Systems;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;
using server.Code.MorpehFeatures.PlayersFeature.DbModels;

namespace server.Code.MorpehFeatures.TestFeature.Systems;

public class TestCreateRecordInPlayersTable : ISystem
{
    [Injectable] private DatabaseInitialization _databaseInitialization;
    [Injectable] private IDbConnector _dbConnector;
    
    private float _timer = 1000;
    private Random _random;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _random = new Random();
        
        _databaseInitialization.Subscribe(Handler);
    }

    private void Handler()
    {
        Logger.Debug("Попытка сделать запись в таблицу");
        
        var playerModel = new DbPlayerModel
        {
            unique_id = Guid.NewGuid().ToString(),
            nickname = "Player_N",
            level = _random.Next(8, 88),
            experience = _random.Next(100, 10000),
            chips = _random.Next(1000, 1000000),
            gold = _random.Next(10, 100),
            stars = _random.Next(20, 120),
            registration_date = DateTime.UtcNow,
        };
        
        InsertPlayerThreadPool(playerModel).Forget();
    }

    public void OnUpdate(float deltaTime)
    {
    }
    
    private async Task<int> InsertPlayerThreadPool(DbPlayerModel playerModel)
    {
        return await Task.Run(async () => await InsertPlayerAsync(playerModel));
    }

    private async Task<int> InsertPlayerAsync(DbPlayerModel playerModel)
    {
        return await _dbConnector.ExecuteAsync(session =>
        {
            return session.UnitOfWork.Connection.ExecuteAsync(@"INSERT INTO players (unique_id, nickname, level, 
                     experience, chips, gold, stars, registration_date) 
                VALUES (@unique_id, @nickname, @level, @experience, @chips, @gold, @stars, @registration_date)", playerModel);
        });
    }

    public void Dispose()
    {
        _databaseInitialization.Unsubscribe(Handler);
        _random = null;
    }
}