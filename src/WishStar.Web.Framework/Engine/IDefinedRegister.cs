using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using WishStar.Web.Framework.FrameWork;

namespace WishStar.Web.Framework.Engine
{
    public interface IDefinedRegister
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="typeFinder"></param>
        void Register(ContainerBuilder builder, ITypeFinder typeFinder);

        /// <summary>
        /// 排序
        /// </summary>
        int ShowOrder { get; }
    }
}
