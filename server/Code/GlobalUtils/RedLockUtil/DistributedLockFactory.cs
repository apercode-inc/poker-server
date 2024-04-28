using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Server.GlobalUtils
{
    public class DistributedLockFactory
    {
        private readonly ConcurrentDictionary<string, string> _keysToLockIdStorage;
        
        /// <summary>
        /// Create factory for in memory locks
        /// </summary>
        public DistributedLockFactory()
        {
            _keysToLockIdStorage = new ConcurrentDictionary<string, string>();
        }
        
        public IDistributedLock Create(string resource, TimeSpan spinDuration, int spinCount, TimeSpan keyTimeout)
        {
            ThrowIfInvalidSettings(keyTimeout);
            
            return new InMemoryLock(_keysToLockIdStorage, resource, keyTimeout);
        }
        
        private void ThrowIfInvalidSettings(TimeSpan keyTimeout)
        {
            if (keyTimeout.TotalSeconds < 5)
            {
                throw new ArgumentOutOfRangeException(nameof(keyTimeout), "Key timeout must be at least 5 seconds");
            }
        }
    }
}