using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace WishStar.Web.Framework.Engine.Implementation
{
    public class AutofacExpand
    {
        /// <summary>
        /// 构造容器扩展
        /// </summary>
        public Action<ContainerBuilder> BuildExpand { get; set; }

        /// <summary>
        /// 依赖注册扩展
        /// </summary>
        public Action<IContainer> RegisterExpand { get; set; }

        /// <summary>
        /// 作用域生命周期扩展
        /// </summary>
        public Func<ILifetimeScope> ScopeExpand { get; set; }

    }
}
