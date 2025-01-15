using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using server.Code.GlobalUtils;
using server.Code.MorpehFeatures.DataBaseFeature.Interfaces;

namespace server.Code.MorpehFeatures.DataBaseFeature.Utils;

public static class IDbConnectorExtensions
    {
        private const int MAX_RETRIES = 3;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T> ExecuteAsync<T>(this IDbConnector connector, Func<DatabaseSession, Task<T>> func)
        {
#if DEBUG
            var dateTime = DateTime.Now;
            Logger.Debug($"[{dateTime}] IDbConnectorExtensions call at {func.GetMethodInfo()}");
#endif
            
            ThrowIfMainThread();
            DatabaseSession session = null;

            var attempts = 0;
            Exception exception;
            
            while (true)
            {
                try
                {
                    session = connector.GetSession();
                    
                    var stopwatch = Stopwatch.StartNew();
                    var task = await func.Invoke(session);
                    stopwatch.Stop();

                    return task;
                }
                catch (Exception e)
                {
                    exception = e;
                }
                finally
                {
                    session?.Dispose();
                }

                if (attempts >= MAX_RETRIES)
                {
                    Logger.Error($"IDbConnector.ExecuteAsync retry {attempts}, error: {exception.Message}, stackTrace: {new StackTrace()}");
                    SentrySdk.CaptureException(exception);
                    throw exception;
                }
                
                attempts++;

                await Task.Delay(500);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T> ExecuteOnThreadPool<T>(this IDbConnector connector, Func<DatabaseSession, Task<T>> func)
        {
            return await Task.Run(async () => await connector.ExecuteAsync(func));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ThrowIfMainThread()
        {
            if (MainThread.IsMainThread)
            {
                throw new Exception("IDbConnectorExtensions.ExecuteAsync not allowed in main thread");
            }
        }
    }