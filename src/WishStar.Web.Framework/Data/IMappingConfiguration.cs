using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace WishStar.Web.Framework.Data
{
    public interface IMappingConfiguration
    {
        /// <summary>
        /// 添加并应用该配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        void ApplyConfiguration(ModelBuilder modelBuilder);
    }
}
