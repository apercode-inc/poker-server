using System;
using System.Threading.Tasks;

namespace Server.GlobalUtils
{
    public interface IDistributedLock
    {
        /// <summary>
        /// Determines if lock is acquired or not
        /// </summary>
        public bool IsAcquired { get; }
        
        // Async API
        /// <summary>
        /// Tries to acquire the lock. If the lock is already acquired by something else, the method will early-return
        /// </summary>
        /// <returns></returns>
        public Task TryAcquireAsync();
        
        /// <summary>
        /// Tries to release the lock. Will do nothing if lock is not acquired
        /// </summary>
        /// <returns></returns>
        public Task TryReleaseAsync();
        
        /// <summary>
        /// Waits for already acquired-by-something-else lock to be released
        /// </summary>
        /// <param name="retryInterval">Retry duration to check for lock</param>
        /// <returns></returns>
        public Task WaitUntilReleased(TimeSpan retryInterval);
        
        // Callback API
        /// <summary>
        /// Tries to acquire the lock
        /// </summary>
        /// <param name="callback">Callback to call no matter if lock is acquired or not</param>
        public void TryAcquire(Action<IDistributedLock> callback);
        
        /// <summary>
        /// Tries to release the lock
        /// </summary>
        /// <param name="callback">Callback to call no matter if lock is released or not</param>
        public void TryRelease(Action<IDistributedLock> callback = null);
    }
}