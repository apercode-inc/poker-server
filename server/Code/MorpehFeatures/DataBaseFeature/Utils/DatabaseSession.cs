using System.Data;
using server.Code.GlobalUtils;
using server.Code.MorpehFeatures.DataBaseFeature.Interfaces;
using SqlKata;
using SqlKata.Execution;

namespace server.Code.MorpehFeatures.DataBaseFeature.Utils;

public sealed class DatabaseSession : IDisposable
{
    private IDbConnector _connector;
    private IDbConnection _connection;
    private QueryFactory _queryFactory;

    public IUnitOfWork UnitOfWork { get; private set; }

    public static bool LogQuery;

    private DatabaseSession()
    {
    }
        
    public DatabaseSession(IDbConnector connector)
    {
        _connector = connector;
        _connection = _connector.GetConnection();

        UnitOfWork = new UnitOfWork(_connection);
    }
        
    public QueryFactory QueryFactory(bool log = false)
    {
        if (_queryFactory == null)
        {
            _queryFactory = new QueryFactory(UnitOfWork.Connection, _connector.GetCompiler());
                
            if (log || LogQuery)
            {
                _queryFactory.Logger = LogSql;
            }
        }

        return _queryFactory; 
    }

    private static void LogSql(SqlResult compiled)
    {
        Logger.Debug(compiled.ToString());
    }

    public void Dispose()
    {
        _connector = null;

        UnitOfWork?.Dispose();
        UnitOfWork = null;

        _connection?.Dispose();
        _connection = null;

        _queryFactory?.Dispose();
        _queryFactory = null;
    }
}