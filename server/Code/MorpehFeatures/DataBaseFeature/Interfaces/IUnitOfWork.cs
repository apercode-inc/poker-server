using System.Data;

namespace server.Code.MorpehFeatures.DataBaseFeature.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public IDbConnection Connection { get; }
    public IDbTransaction Transaction { get; }

    void Begin();
    void Commit();
    void Rollback();
}