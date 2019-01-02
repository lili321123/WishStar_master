using System;
using System.Collections.Generic;
using System.Text;

namespace WishStar.Caching.Core.Common
{
    public struct CacheType
    {
        /// <summary>
        /// memcache
        /// </summary>
        public const string MemcacheCache = "memcache";

        /// <summary>
        /// 内存缓存
        /// </summary>
        public const string MemoryCache = "memorycache";

        /// <summary>
        /// http生命周期缓存
        /// </summary>
        public const string PerRequestCache = "requestcache";

        /// <summary>
        /// redis缓存
        /// </summary>
        public const string RedisCache = "rediscache";
    }
}
