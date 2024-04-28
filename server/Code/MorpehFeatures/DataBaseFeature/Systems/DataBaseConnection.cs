using System.Data.Common;
using MySqlConnector;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.DataBaseFeature.Interfaces;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;
using SqlKata.Compilers;

namespace server.Code.MorpehFeatures.DataBaseFeature.Systems;

public class DataBaseConnector : IInitializer, IDbConnector
{
    [Injectable] private ServerParameters _serverParameters;
    [Injectable] private DatabaseInitialization _databaseInitialization;
    
    public string ConnectionString { get; private set; }
    
    public World World { get; set; }

    public void OnAwake()
    {
        var connectionString = new MySqlConnectionStringBuilder
        {
            Server = _serverParameters.SqlHost,
            Port = (uint)_serverParameters.SqlPort,
            UserID = _serverParameters.SqlUser,
            Password = _serverParameters.SqlPassword,
            Database = _serverParameters.SqlDatabase,
            SslMode = MySqlSslMode.None,
            CharacterSet = "utf8",
            MinimumPoolSize = 0,
            MaximumPoolSize = 32,
        };
        ConnectionString = connectionString.ToString();

        _databaseInitialization.StartProcessTables(ConnectionString);
    }

    public DbConnection GetConnection()
    {
        return new MySqlConnection(ConnectionString);
    }

    public DatabaseSession GetSession()
    {
        return new DatabaseSession(this);
    }

    public Compiler GetCompiler()
    {
        return new MySqlCompiler();
    }
    
    public void Dispose()
    {
    }
}