using Dapper;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AuthenticationFeature.DbModels;
using server.Code.MorpehFeatures.DataBaseFeature.Interfaces;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;
using Server.Database;

namespace server.Code.MorpehFeatures.AuthenticationFeature.Systems;

public class AuthenticationDbService : IInitializer
{
    [Injectable] private IDbConnector _dbConnector;
    
    public World World { get; set; }

    public void OnAwake()
    {
    }
    
    public async Task<IEnumerable<DbUserModel>> GetUserThreadPool(string uniqueId)
    {
        return await Task.Run(async () => await GetUserAsync(uniqueId));
    }
    
    public async Task<IEnumerable<DbUserModel>> GetUserAsync(string uniqueId)
    {
        return await _dbConnector.ExecuteAsync(session =>
        {
            return session.UnitOfWork.Connection.QueryAsync<DbUserModel>(
                @"SELECT * FROM users WHERE unique_id = @unique_id", new QueryParameters
                {
                    { "unique_id", uniqueId },
                });
        });
    }

    public void Dispose()
    {
    }
}