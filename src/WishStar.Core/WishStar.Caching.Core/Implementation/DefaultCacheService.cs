using System;
using System.Collections.Generic;
using System.Text;
using WishStar.Caching.Core.Common;
using WishStar.Web.Framework.Engine.Implementation;

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
            catch (Exception)
            {

                throw;
            }
        }

        public void IsExist(CacheKeyCode cacheCode)
        {
            throw new NotImplementedException();
        }

        public bool KeyExpire(CacheKeyCode cacheCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> MGet<T>(List<CacheKeyCode> cacheCodes, Func<Dictionary<CacheKeyCode, T>> fun = null)
        {
            throw new NotImplementedException();
        }

        public void Remove(CacheKeyCode cacheCode)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }

        public void RemoveByPattern(string pattern)
        {
            throw new NotImplementedException();
        }

        public void Set(CacheKeyCode cacheCode, object data)
        {
            throw new NotImplementedException();
        }

        public string StringGet(CacheKeyCode cacheCode, Func<string> fun = null)
        {
            throw new NotImplementedException();
        }

        private ICacheProvider GetCacheProvider(string cacheType)
        {
            return EngineContext.Resolve<ICacheProvider>(cacheType);
        }
    }
}
