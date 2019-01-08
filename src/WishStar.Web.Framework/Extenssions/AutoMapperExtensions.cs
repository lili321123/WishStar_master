using System;
using System.Collections.Generic;
using System.Text;
using WishStar.Web.Framework.Data;
using WishStar.Web.Framework.Engine.Implementation;

namespace WishStar.Web.Framework.Extenssions
{
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// 模型映射
        /// </summary>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDestination MapTo<TDestination>(this BaseModel source)
            where TDestination : class
        {
            return AutoMapperConfiguration.Mapper.Map<TDestination>(source);
        }

        /// <summary>
        /// 实体映射
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDestination MapTo<TDestination>(this BaseEntity source)
            where TDestination : class
        {
            return AutoMapperConfiguration.Mapper.Map<TDestination>(source);
        }
    }
}
