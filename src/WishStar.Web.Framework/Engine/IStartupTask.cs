using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;

namespace WishStar.Web.Framework.Engine
{
    public interface IStartupTask
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        void ConfigureServices(IServiceCollection services, IConfiguration config);

        /// <summary>
        /// 添加配置中间件
        /// </summary>
        /// <param name="app"></param>
        void Configure(IApplicationBuilder app);

        /// <summary>
        /// 排序
        /// </summary>
        int ShowOrder { get; }
    }
}
