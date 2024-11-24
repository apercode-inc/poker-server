using Dapper;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.DbModels;
using server.Code.MorpehFeatures.DataBaseFeature.Interfaces;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;
using Server.Database;

namespace server.Code.MorpehFeatures.AdsFeature.Systems;

public class AdsDbService : IInitializer
{
    [Injectable] private IDbConnector _dbConnector;
    
    public World World { get; set; }

    public void OnAwake()
    {
    }
    
    public async Task<IEnumerable<DbPlayerAdsCooldownModel>> GetPlayerAdsCooldownsAsync(string playerId)
    {
        return await _dbConnector.ExecuteAsync(session
            => session.UnitOfWork.Connection.QueryAsync<DbPlayerAdsCooldownModel>(
                $@"SELECT * FROM {DbPlayerAdsCooldownConstants.TableName} WHERE {DbPlayerAdsCooldownConstants.PlayerId} = @player_id", new QueryParameters
                {
                    { "player_id", playerId },
                })
            );
    }

    public async Task<int> UpdatePlayerAdsCooldownAsync(DbPlayerAdsCooldownModel model)
    {
        return await Task.Run(async () => await _dbConnector.ExecuteAsync(session
            => session.UnitOfWork.Connection.ExecuteAsync(
                $"UPDATE {DbPlayerAdsCooldownConstants.TableName} SET {DbPlayerAdsCooldownConstants.EndTimestamp} = @timestamp" +
                $"WHERE {DbPlayerAdsCooldownConstants.PlayerId} = @player_id AND {DbPlayerAdsCooldownConstants.PanelId} = @panel_id",
                new QueryParameters
                {
                    { "player_id", model.player_id },
                    { "panel_id", model.panel_id },
                    { "timestamp", model.end_timestamp },
                })
            ));
    }

    public async Task<int> InsertPlayerAdsCooldownAsync(DbPlayerAdsCooldownModel model)
    {
        return await Task.Run(async () => await _dbConnector.ExecuteAsync(session
            => session.UnitOfWork.Connection.ExecuteAsync(
                $"INSERT INTO {DbPlayerAdsCooldownConstants.TableName}" +
                $"VALUES (@player_id, @panel_id, @timestamp)",
                new QueryParameters
                {
                    { "player_id", model.player_id },
                    { "panel_id", model.panel_id },
                    { "timestamp", model.end_timestamp },
                })
            ));
    }

    public async Task<int> RemovePlayerAdsCooldownAsync(DbPlayerAdsCooldownModel model)
    {
        return await Task.Run(async () => await _dbConnector.ExecuteAsync(session
            => session.UnitOfWork.Connection.ExecuteAsync(
                $@"DELETE FROM {DbPlayerAdsCooldownConstants.TableName} WHERE {DbPlayerAdsCooldownConstants.PlayerId} = @player_id AND {DbPlayerAdsCooldownConstants.PanelId} = @panel_id",
                new QueryParameters
                {
                    { "player_id", model.player_id },
                    { "panel_id", model.panel_id },
                })
        ));
    }

    public void Dispose()
    {
    }
}