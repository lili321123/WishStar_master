using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using WishStar.Caching.Core.Common;
using WishStar.Caching.Core.Implementation;
using WishStar.Web.Framework.Engine;
using WishStar.Web.Framework.FrameWork;

namespace WishStar.Caching.Core
{
    public class AutofacRegister : IDefinedRegister
    {
        public int ShowOrder => 1000;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<DefaultCacheService>().As<ICacheService>().SingleInstance();
            builder.RegisterType<PerRequestCacheProvider>()
                .Named<ICacheProvider>(CacheType.PerRequestCache).SingleInstance();
            builder.RegisterType<MemoryCacheProvider>()
                .Named<ICacheProvider>(CacheType.MemoryCache).SingleInstance();
            builder.RegisterType<RedisCacheProvider>()
                .Named<ICacheProvider>(CacheType.RedisCache).SingleInstance();
        }
    }
}
