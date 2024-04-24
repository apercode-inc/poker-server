using server.Code.GlobalUtils;

namespace server.Code.MorpehFeatures.DataBaseFeature.Utils;

public static class FluentAsync
{
    public static void Forget(this Task task)
    {
        if (!task.IsCompleted || task.IsFaulted)
        {
            _ = ForgetAwaited(task);
        }
            
        static async Task ForgetAwaited(Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString(), true);
            }
        }
    }
}