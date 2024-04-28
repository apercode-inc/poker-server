using System.Collections.Concurrent;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;

namespace Server.GlobalUtils
{
    public class InMemoryLock : IDistributedLock
    {
        // Parameters
        private readonly ConcurrentDictionary<string, string> _keysToLockIdStorage;
        private readonly string _resource;
        private readonly TimeSpan _keyTimeout;

        // State
        private readonly string _lockId;
        private DateTime _acquiredTime;
        private bool _isAcquired;
        
        public bool IsAcquired => _isAcquired && DateTime.Now.Subtract(_acquiredTime) < _keyTimeout;

        internal InMemoryLock(ConcurrentDictionary<string, string> keysToLockIdStorage, string resource, TimeSpan keyTimeout)
        {
            _keysToLockIdStorage = keysToLockIdStorage;
            _resource = resource;
            _keyTimeout = keyTimeout;

            _lockId = Guid.NewGuid().ToString();
        }
        
        public Task TryAcquireAsync()
        {
            _isAcquired = false;
            
            TryLockKeyAndScheduleDelete();
            
            return Task.CompletedTask;
        }
        
        public Task TryReleaseAsync()
        {
            _isAcquired = false;

            SafeRemove();
            
            return Task.CompletedTask;
        }
        
        public async Task WaitUntilReleased(TimeSpan retryInterval)
        {
            if (IsAcquired)
            {
                throw new InvalidOperationException("Lock is acquired, cannot wait until release");
            }

            do
            {
                await Task.Delay(retryInterval);
            }
            while (_keysToLockIdStorage.ContainsKey(_resource));
        }
        
        public void TryAcquire(Action<IDistributedLock> callback)
        {
            Task.Run(async () =>
            {
                await TryAcquireAsync();
                
                // MainThread.Run(() => //todo thread
                // {
                //     try
                //     {
                //         callback?.Invoke(this);
                //     }
                //     catch (Exception e)
                //     {
                //         Log.Error(e, "LockAcquire | System error: {message}");
                //     }
                // });
            }).Forget();
        }
        
        public void TryRelease(Action<IDistributedLock> callback = null)
        {
            Task.Run(async () =>
            {
                await TryReleaseAsync();
                // MainThread.Run(() => //todo main thread 
                // {
                //     try
                //     {
                //         callback?.Invoke(this);
                //     }
                //     catch (Exception e)
                //     {
                //         Log.Error(e, "LockRelease | System error: {message}");
                //     }
                // });
            }).Forget();
        }

        private void TryLockKeyAndScheduleDelete()
        {
            if(_keysToLockIdStorage.TryAdd(_resource, _lockId))
            {
                _isAcquired = true;
                _acquiredTime = DateTime.Now;

                // schedule attempt to removing of concrete combination of key+value to avoid of memory leaking due to
                // customers use combined keys like some_const_string+<playerID/clan_id/so on> and may not release them
                Task.Run(async () =>
                {
                    await Task.Delay(_keyTimeout);

                    _keysToLockIdStorage?.TryRemove(new KeyValuePair<string, string>(_resource, _lockId));
                }).Forget();
            }
        }

        // remove key with validation due to key may be old and other thread could update it right before removing
        private void SafeRemove()
        {
            bool hasKey = _keysToLockIdStorage.TryGetValue(_resource, out string keyLockId);
            if (hasKey && keyLockId == _lockId)
            {
                _keysToLockIdStorage.TryRemove(new KeyValuePair<string, string>(_resource, keyLockId));
            }
        }
    }
}