using System;
using System.Collections.Generic;
using System.Text;
using WishStar.Caching.Core.Common;
using WishStar.Web.Framework.Engine.Implementation;
using System.Linq;

namespace WishStar.Caching.Core.Implementation
{
    public class DefaultCacheService : ICacheService
    {
        private ICacheProvider perRequestCache;
        public DefaultCacheService()
        {
            perRequestCache = EngineContext.Resolve<ICacheProvider>(CacheType.PerRequestCache);
        }
        public T Get<T>(CacheKeyCode cacheCode, Func<T> fun = null)
        {
            try
            {
                if (cacheCode == null)
                {
                    return fun.Invoke();
                }
                string cacheKey = cacheCode.ToString();
                T result = perRequestCache.Get<T>(cacheKey);
                var cacheProvider = this.GetCacheProvider(cacheCode.CacheType);
                if (result == null || EqualityComparer<T>.Default.Equals(result, default(T)))
                {
                    result = cacheProvider.Get<T>(cacheKey);
                }
                
                if (result == null || EqualityComparer<T>.Default.Equals(result, default(T)))
                {
                    result = fun.Invoke();
                    cacheProvider.Set(cacheKey, result, cacheCode.CacheTime);
                }

                perRequestCache.Set(cacheKey, result, cacheCode.CacheTime);
                return result;
            }
            catch (Exception e)
            {
                //log
            }
            return default(T);
        }

        public bool IsExist(CacheKeyCode cacheCode)
        {
            string cacheKey = cacheCode.ToString();
            var cacheProvider = this.GetCacheProvider(cacheCode.CacheType);
            return cacheProvider.IsExist(cacheKey);

        }

        public bool KeyExpire(CacheKeyCode cacheCode)
        {
            string cacheKey = cacheCode.ToString();
            var cacheProvider = this.GetCacheProvider(cacheCode.CacheType);
            return cacheProvider.KeyExpire(cacheKey, DateTime.Now.AddMinutes(cacheCode.CacheTime));
        }

        public IEnumerable<T> MGet<T>(List<CacheKeyCode> cacheCodes)
        {
            if (cacheCodes == null || !cacheCodes.Any())
            {
                return null;
            }
            var cacheCode = cacheCodes.First();
            List<string> keys = cacheCodes.Select(x => x.ToString()).ToList();
            var cacheProvider = this.GetCacheProvider(cacheCode.CacheType);
            Dictionary<string, T> partialResult = cacheProvider.MGet<T>(keys);
            return partialResult.Values;
        }

        public void Remove(CacheKeyCode cacheCode)
        {
            string cacheKey = cacheCode.ToString();
            var cacheProvider = this.GetCacheProvider(cacheCode.CacheType);
            cacheProvider.Remove(cacheKey);
        }
        

        public void RemoveByPattern(string cacheType,string pattern)
        {
            var cacheProvider = this.GetCacheProvider(cacheType);
            cacheProvider.RemoveByPattern(pattern);
        }

        public void Set(CacheKeyCode cacheCode, object data)
        {
            string cacheKey = cacheCode.ToString();
            var cacheProvider = this.GetCacheProvider(cacheCode.CacheType);
            perRequestCache.Set(cacheKey, data, cacheCode.CacheTime);
            cacheProvider.Set(cacheKey, data, cacheCode.CacheTime);
        }

        public string StringGet(CacheKeyCode cacheCode, Func<string> fun = null)
        {
            try
            {
                if (cacheCode == null)
                {
                    return fun.Invoke();
                }
                string cacheKey = cacheCode.ToString();
                string result = perRequestCache.StringGet(cacheKey);
                var cacheProvider = this.GetCacheProvider(cacheCode.CacheType);
                if (string.IsNullOrWhiteSpace(result))
                {
                    result = cacheProvider.StringGet(cacheKey);
                }

                if (string.IsNullOrWhiteSpace(result))
                {
                    result = fun.Invoke();
                    cacheProvider.Set(cacheKey, result, cacheCode.CacheTime);
                }

                perRequestCache.Set(cacheKey, result, cacheCode.CacheTime);
                return result;
            }
            catch (Exception e)
            {
                //log
            }
            return string.Empty;
        }

        private ICacheProvider GetCacheProvider(string cacheType)
        {
            return EngineContext.Resolve<ICacheProvider>(cacheType);
        }
    }
}
