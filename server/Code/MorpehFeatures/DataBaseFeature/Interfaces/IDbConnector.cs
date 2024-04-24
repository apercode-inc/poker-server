using System.Data.Common;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;
using SqlKata.Compilers;

namespace server.Code.MorpehFeatures.DataBaseFeature.Interfaces;

public interface IDbConnector
{
    
    public string ConnectionString { get; }
    public DbConnection GetConnection();
    public DatabaseSession GetSession();
    public Compiler GetCompiler();
}