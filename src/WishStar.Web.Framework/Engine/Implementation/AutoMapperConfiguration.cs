using System;
using System.Collections.Generic;
using System.Text;
using WishStar.Web.Framework.FrameWork;
using System.Linq;
using AutoMapper;
using WishStar.Web.Framework.FrameWork.Implementation;

namespace WishStar.Web.Framework.Engine.Implementation
{
    public static class AutoMapperConfiguration
    {
        public static IMapper Mapper { get; private set; }

        /// <summary>
        /// 添加自动映射配置
        /// </summary>
        public static void AddAutoMapper(AppDomainTypeFinder typeFinder)
        {
            var types = typeFinder.FindClassesOfType<IAutoMapperProfile>(true);

            if (types == null || types.Count() <= 0)
            {
                return;
            }

            var instances = types.Select(x => (IAutoMapperProfile)Activator.CreateInstance(x))
                .OrderByDescending(x => x.ShowOrder);

            //添加配置映射表
            var configs = new MapperConfiguration(config =>
            {
                foreach (var instance in instances)
                {
                    config.AddProfile(instance.GetType());
                }
            });

            //初始化映射
            Mapper = configs.CreateMapper();
        }
    }
}
