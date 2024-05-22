using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using Server.GlobalUtils;

namespace server.Code.MorpehFeatures.DataBaseFeature.Systems;

public class DatabaseInitialization : IInitializer
{
    private DistributedLockFactory _distributedLockFactory;
    private const string AsyncOperationLockId = "database_schema_operation";

    public bool IsReady => _ready;

    private bool _ready = false;
    private Action isReadyAction;
    
    public World World { get; set; }
    
    public void OnAwake()
    {
        _distributedLockFactory = new DistributedLockFactory();
    }

    public void StartProcessTables(string connectionString)
    {
        Task.Run(() => ProcessTables(connectionString));
    }

    public void Subscribe(Action action)
    {
        if (_ready)
        {
            action?.Invoke();
            return;
        }

        isReadyAction += action;
    }

    public void Unsubscribe(Action action)
    {
        isReadyAction -= action;
    }

    private void SetReady()
    {
        _ready = true;
        isReadyAction?.Invoke();
        isReadyAction = null;
    }

    private async Task ProcessTables(string connectionString)
    {
        try
        {
            var opLock = _distributedLockFactory.Create(
                resource: AsyncOperationLockId,
                spinDuration: TimeSpan.FromSeconds(1),
                spinCount: 5,
                keyTimeout: TimeSpan.FromSeconds(30));

            await opLock.TryAcquireAsync();
            
            while (!opLock.IsAcquired)
            {
                LogMessage($"Not starting database operations - operation {AsyncOperationLockId} is locked by something else, waiting");

                await opLock.WaitUntilReleased(TimeSpan.FromMilliseconds(500));
                await opLock.TryAcquireAsync();
            }

            LogMessage("Start");

            if (opLock.IsAcquired)
            {
                var services = CreateServices(connectionString);

                using var scope = services.CreateScope();
                UpdateDatabase(scope.ServiceProvider);

                await opLock.TryReleaseAsync();
            }

            LogMessage("Finish");

            SetReady();
        }
        catch (Exception e)
        {
            LogMessage(e);
        }
    }

    private IServiceProvider CreateServices(string connectionString)
    {
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddMySql5()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.GetExecutingAssembly())
                .For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }

    private void UpdateDatabase(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    private void LogMessage(object obj)
    {
        Logger.Debug($"[MIGRATIONS] {obj}");
    }

    public void Dispose()
    {
        _distributedLockFactory = null;
    }
}