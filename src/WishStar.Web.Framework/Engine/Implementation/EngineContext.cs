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

            //初始化automapper
            AutoMapperConfiguration.AddAutoMapper(typeFinder);

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

    }
}
