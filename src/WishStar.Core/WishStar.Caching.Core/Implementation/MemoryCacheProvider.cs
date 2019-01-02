using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WishStar.Caching.Core.Implementation
{
    public class MemoryCacheProvider:ICacheProvider,ILocker
    {
        private readonly IMemoryCache _memoryCache;
        /// <summary>
        /// 全部缓存key
        /// </summary>
        private static readonly ConcurrentDictionary<string, bool> _allKeys;

        private CancellationTokenSource _cancellationTokenSource;
        static MemoryCacheProvider()
        {
            _allKeys = new ConcurrentDictionary<string, bool>();
        }

        public MemoryCacheProvider(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private string AddKey(string key)
        {
            _allKeys.TryAdd(key, true);
            return key;
        }

        private string RemoveKey(string key)
        {
            if (!_allKeys.TryRemove(key, out _))
            {
                _allKeys.TryUpdate(key, false, true);
            }
            return key;
        }

        private void ClearKeys()
        {
            foreach (var key in _allKeys.Where(p => !p.Value).Select(p => p.Key))
            {
                this.RemoveKey(key);
            }
        }
        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions(TimeSpan cacheTime)
        {
            var options = new MemoryCacheEntryOptions()
                // add cancellation token for clear cache
                .AddExpirationToken(new CancellationChangeToken(_cancellationTokenSource.Token))
                //add post eviction callback
                .RegisterPostEvictionCallback(PostEviction);

            //set cache time
            options.AbsoluteExpirationRelativeToNow = cacheTime;

            return options;
        }
        private void PostEviction(object key, object value, EvictionReason reason, object state)
        {
            //if cached item just change, then nothing doing
            if (reason == EvictionReason.Replaced)
                return;

            //try to remove all keys marked as not existing
            ClearKeys();

            //try to remove this key from dictionary
            RemoveKey(key.ToString());
        }


        public string StringGet(string key)
        {
            return this._memoryCache.Get<string>(key);
        }

        public T Get<T>(string key)
        {
            return this._memoryCache.Get<T>(key);
        }

        public Dictionary<string, T> MGet<T>(IEnumerable<string> keys)
        {
            Dictionary<string, T> dic = new Dictionary<string, T>();
            foreach (var key in keys)
            {
                T t = this.Get<T>(key);
                if (!dic.ContainsKey(key)
                    && !EqualityComparer<T>.Default.Equals(t, default(T)))
                {
                    dic.Add(key, t);
                }
            }
            return dic;
        }

        public void Set(string key, object data, int cacheTime)
        {
            if (data == null)
            {
                return;
            }
            _memoryCache.Set(AddKey(key), data, GetMemoryCacheEntryOptions(TimeSpan.FromMinutes(cacheTime)));
        }

        public void Set(string key, object data, DateTime expiresAt)
        {
            int cacheTime = (int)expiresAt.Subtract(DateTime.Now).TotalMinutes;
            this.Set(AddKey(key), data, cacheTime);
        }

        public bool IsExist(string key)
        {
            return _memoryCache.TryGetValue(key, out object _);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(RemoveKey(key));
        }

        public void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            var matchesKeys = _allKeys.Where(p => p.Value&& regex.IsMatch(p.Key)).ToList();
            foreach (var item in matchesKeys)
            {
                this.Remove(item.Key);
            }
        }

        public void Clear()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public bool KeyExpire(string key, DateTime? expiry)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public bool PerformActionWithLock(string key, TimeSpan expirationTime, Action action)
        {
            //ensure that lock is acquired
            if (!_allKeys.TryAdd(key, true))
                return false;

            try
            {
                _memoryCache.Set(key, key, GetMemoryCacheEntryOptions(expirationTime));

                //perform action
                action();

                return true;
            }
            finally
            {
                //release lock even if action fails
                Remove(key);
            }
        }
    }
}
