using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using server.Code.GlobalUtils;
using Server.GlobalUtils;

namespace server.Code.MorpehFeatures.DataBaseFeature.Utils;

public class DatabaseInitialization
{
    public string ConnectionString; //todo refactor
    
    private DistributedLockFactory _distributedLockFactory;
    private const string AsyncOperationLockId = "database_schema_operation";

    public bool IsReady => _ready;

    private bool _ready = false;
    private Action isReadyAction;

    public DatabaseInitialization(DistributedLockFactory distributedLockFactory)
    {
        _distributedLockFactory = distributedLockFactory;
    }

    public void StartProcessTables()
    {
        Task.Run(ProcessTables);
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

    private async Task ProcessTables()
    {
        try
        {
            var opLock = _distributedLockFactory.Create(
                resource: AsyncOperationLockId,
                spinDuration: TimeSpan.FromSeconds(1),
                spinCount: 5,
                keyTimeout: TimeSpan.FromSeconds(30));

            await opLock.TryAcquireAsync();

            LogMessage("Start");

            if (opLock.IsAcquired)
            {
                var services = CreateServices();

                using var scope = services.CreateScope();
                UpdateDatabase(scope.ServiceProvider);

                await opLock.TryReleaseAsync();
            }
            else
            {
                LogMessage(
                    $"Not starting database operations - operation {AsyncOperationLockId} is locked by something else, waiting");

                await opLock.WaitUntilReleased(TimeSpan.FromMilliseconds(500));

                LogMessage($"{AsyncOperationLockId} released, finished all operations");
            }

            LogMessage("Finish");

            SetReady();
        }
        catch (Exception e)
        {
            LogMessage(e);
        }
    }

    private IServiceProvider CreateServices()
    {
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddMySql5()
                .WithGlobalConnectionString(ConnectionString)
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
}