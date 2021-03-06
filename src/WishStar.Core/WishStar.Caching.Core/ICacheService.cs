﻿using System;
using System.Collections.Generic;
using System.Text;
using WishStar.Caching.Core.Common;

namespace WishStar.Caching.Core
{
    public interface ICacheService
    {
        T Get<T>(CacheKeyCode cacheCode, Func<T> fun = null);

        string StringGet(CacheKeyCode cacheCode, Func<string> fun = null);

        IEnumerable<T> MGet<T>(List<CacheKeyCode> cacheCodes);

        void Set(CacheKeyCode cacheCode, object data);

        void Remove(CacheKeyCode cacheCode);

        void RemoveByPattern(string cacheType, string pattern);
        

        bool KeyExpire(CacheKeyCode cacheCode);

        bool IsExist(CacheKeyCode cacheCode);
    }
}
