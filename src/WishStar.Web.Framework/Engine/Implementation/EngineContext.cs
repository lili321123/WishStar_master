using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using WishStar.Web.Framework.FrameWork;
using WishStar.Web.Framework.FrameWork.Implementation;

namespace WishStar.Web.Framework.Engine.Implementation
{
    public class EngineContext
    {
        private static IEngine _engineInstance;
        public static IEngine Current
        {
            get
            {
                return CreateInstance();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static IEngine CreateInstance()
        {
            if (_engineInstance == null)
            {
                _engineInstance = new AutofacEngine();
            }
            return _engineInstance;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(IServiceCollection services, AutofacExpand expand = null)
        {
            var typeFinder = new AppDomainTypeFinder();

            _engineInstance = new AutofacEngine(expand);
            _engineInstance.Initialize(services, typeFinder);

            RegisterAutoMapper(typeFinder);

            return _engineInstance;
        }

        public static IService Resolve<IService>()
        {
            return Current.Resolve<IService>();
        }

        public static IService Resolve<IService>(string name)
        {
            return Current.Resolve<IService>(name);
        }

        /// <summary>
        /// 注册auto mapper
        /// </summary>
        /// <param name="typeFinder"></param>
        protected static void RegisterAutoMapper(ITypeFinder typeFinder)
        {
            //寻找IAutoMapperProfile的实现并排序
            var mapperProfiles = typeFinder.FindClassesOfType<IAutoMapperProfile>(true);
            var instances = mapperProfiles
                .Select(x => (IAutoMapperProfile)Activator.CreateInstance(x))
                .OrderBy(x => x.ShowOrder);

            //初始化配置
            var config = new MapperConfiguration(cfg =>
            {
                foreach (var mapperProfile in instances)
                {
                    cfg.AddProfile(mapperProfile.GetType());
                }
            });

            //初始化映射
            config.CreateMapper();
        }
    }
}
