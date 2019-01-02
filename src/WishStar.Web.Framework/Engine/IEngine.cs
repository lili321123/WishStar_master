using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using WishStar.Web.Framework.FrameWork;

namespace WishStar.Web.Framework.Engine
{
    public interface IEngine
    {
        /// <summary>
        /// 项目引擎初始化
        /// </summary>
        void Initialize(IServiceCollection services, ITypeFinder typeFinder);

        /// <summary>
        /// 解析T类型对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        T Resolve<T>();
        /// <summary>
        /// 解析T类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">别名</param>
        /// <returns></returns>
        T Resolve<T>(string name);
        /// <summary>
        /// 解析指定类型的全部对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T[] ResolveAll<T>();
        /// <summary>
        /// 解析指定类型别名的全部对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T[] ResolveAll<T>(string name);
    }
}
