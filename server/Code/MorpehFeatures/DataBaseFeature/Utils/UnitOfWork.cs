using System.Data;
using server.Code.MorpehFeatures.DataBaseFeature.Interfaces;

namespace server.Code.MorpehFeatures.DataBaseFeature.Utils;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private IDbTransaction _transaction;

    public UnitOfWork(IDbConnection connection)
    {
        _connection = connection;
    }

    IDbConnection IUnitOfWork.Connection => _connection;
    IDbTransaction IUnitOfWork.Transaction => _transaction;

    public void Begin()
    {
        _transaction = _connection.BeginTransaction();
    }

    public void Commit()
    {
        _transaction.Commit();
        Dispose();
    }

    public void Rollback()
    {
        _transaction.Rollback();
        Dispose();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _transaction = null;
    }
}