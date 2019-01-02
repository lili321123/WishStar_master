using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using WishStar.Web.Framework.Engine.Implementation;
using WishStar.Web.Framework.FrameWork;

namespace WishStar.Web.Framework.Data
{
    public class EFDBContext : DbContext, IDbContext
    {
        public EFDBContext(DbContextOptions<EFDBContext> options) 
            : base(options)
        {
        }

        /// <summary>
        /// 重载创建模型的配置约定
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var typeFinder = EngineContext.Resolve<ITypeFinder>();
            var efConfigs = typeFinder.FindClassesOfType<IMappingConfiguration>(true);

            foreach (var type in efConfigs)
            {
                var configuration = (IMappingConfiguration)Activator.CreateInstance(type);
                configuration.ApplyConfiguration(modelBuilder);
            }
            base.OnModelCreating(modelBuilder);
        }
        public virtual new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }
        


        public void Detach<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var entityEntry = this.Entry(entity);
            if (entityEntry == null)
                return;

            //设置上下文没有跟踪实体
            entityEntry.State = EntityState.Detached;
        }

        public IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters) where TEntity : BaseEntity
        {
            return this.Set<TEntity>().FromSql(this.CreateSqlByParams(sql, parameters), parameters);
        }

        public int ExecuteSqlCommand(RawSqlString sql, bool notEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            int result = 0;
            //设置连接超时时间
            var preTimeout = this.Database.GetCommandTimeout();
            this.Database.SetCommandTimeout(timeout);

            if (notEnsureTransaction)
            {
                result = this.Database.ExecuteSqlCommand(sql, parameters);
            }
            else
            {
                using (var transaction = this.Database.BeginTransaction())
                {
                    result = this.Database.ExecuteSqlCommand(sql, parameters);
                    transaction.Commit();
                }
            }

            this.Database.SetCommandTimeout(preTimeout);
            return result;
        }

        public IQueryable<TQuery> QueryFromSql<TQuery>(string sql) where TQuery : class
        {
            return this.Query<TQuery>().FromSql(sql);
        }

        /// <summary>
        /// 构造参数化sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual string CreateSqlByParams(string sql,params object[] parameters)
        {
            for (int i = 0; i < (parameters?.Length ?? 0) - 1; i++)
            {
                if(!(parameters[i] is DbParameter parameter))
                {
                    continue;
                }
                sql = $"{sql}{(i > 0 ? "," : string.Empty)} @{parameter.ParameterName}";

                if (parameter.Direction== ParameterDirection.InputOutput
                    || parameter.Direction == ParameterDirection.Output)
                {
                    sql += " output";
                }
            }
            return sql;
        }
    }
}
