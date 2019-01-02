using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace WishStar.Web.Framework.Data
{
    public partial class BaseEntityTypeConfiguration<TEntity> : IMappingConfiguration, IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        /// <summary>
        /// 可在部分类中重写该方法，添加自定义配置
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void PostConfigure(EntityTypeBuilder<TEntity> builder)
        {

        }
        /// <summary>
        /// 配置实体约束
        /// </summary>
        /// <param name="builder"></param>
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            //添加自定义配置
            this.PostConfigure(builder);
        }

        /// <summary>
        /// 应用此配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        public void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(this);
        }
    }
}
