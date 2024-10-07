using Dapper;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.DataBaseFeature.Interfaces;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;
using server.Code.MorpehFeatures.PlayersFeature.DbModels;
using Server.Database;

namespace server.Code.MorpehFeatures.PlayersFeature.Systems;

public class PlayerDbService : IInitializer
{
    [Injectable] private IDbConnector _dbConnector;
    
    public World World { get; set; }

    public void OnAwake()
    {
    }
    
    public async Task<int> InsertPlayerThreadPool(DbPlayerModel playerModel)
    {
        return await Task.Run(async () => await InsertPlayerAsync(playerModel));
    }

    public async Task<int> InsertPlayerAsync(DbPlayerModel playerModel)
    {
        return await _dbConnector.ExecuteAsync(session =>
        {
            return session.UnitOfWork.Connection.ExecuteAsync(@"INSERT INTO players (unique_id, nickname, level, 
                     experience, chips, gold, stars, avatar_id, avatar_url, registration_date) 
                VALUES (@unique_id, @nickname, @level, @experience, @chips, @gold, @stars, @avatar_id, @avart_url, @registration_date)", playerModel);
        });
    }
    
    public async Task<DbPlayerModel> UpdateChipsPlayerThreadPool(string uniqueId, long chips)
    {
        return await Task.Run(async () => await UpdateChipsPlayerAsync(uniqueId, chips));
    }
    
    public async Task<DbPlayerModel> UpdateChipsPlayerAsync(string uniqueId, long chips)
    {
        return await _dbConnector.ExecuteAsync(session =>
        {
            return session.UnitOfWork.Connection.QueryFirstOrDefaultAsync<DbPlayerModel>(@"
                    UPDATE players SET chips = @chips WHERE unique_id = @unique_id", new QueryParameters
            {
                { "unique_id", uniqueId },
                { "chips", chips },
            });
        });
    }
    
    public async Task<DbPlayerModel> IncreaseChipsPlayerThreadPool(string uniqueId, long chips)
    {
        return await Task.Run(async () => await UpdateChipsPlayerAsync(uniqueId, chips));
    }
    
    public async Task<DbPlayerModel> IncreaseChipsPlayerAsync(string uniqueId, long chips)
    {
        return await _dbConnector.ExecuteAsync(session =>
        {
            return session.UnitOfWork.Connection.QueryFirstOrDefaultAsync<DbPlayerModel>(@"
                    UPDATE players SET chips = chips + @chips WHERE unique_id = @unique_id", new QueryParameters
            {
                { "unique_id", uniqueId },
                { "chips", chips },
            });
        });
    }

    public async Task<IEnumerable<DbPlayerModel>> GetPlayerThreadPool(string uniqueId)
    {
        return await Task.Run(async () => await GetPlayerAsync(uniqueId));
    }
    
    public async Task<IEnumerable<DbPlayerModel>> GetPlayerAsync(string uniqueId)
    {
        return await _dbConnector.ExecuteAsync(session =>
        {
            return session.UnitOfWork.Connection.QueryAsync<DbPlayerModel>(
                @"SELECT * FROM players WHERE unique_id = @unique_id", new QueryParameters
                {
                    { "unique_id", uniqueId },
                });
        });
    }

    public void Dispose()
    {
    }
}