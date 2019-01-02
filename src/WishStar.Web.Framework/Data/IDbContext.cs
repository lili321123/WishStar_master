using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WishStar.Web.Framework.Data
{
    /// <summary>
    /// 提供数据上下文接口
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        /// 写入实体类型
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// 根据sql查询指定对象
        /// </summary>
        /// <typeparam name="TQuery">指定对象</typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        IQueryable<TQuery> QueryFromSql<TQuery>(string sql) where TQuery : class;

        /// <summary>
        /// 根据sql查询实体对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters) where TEntity : BaseEntity;

        /// <summary>
        /// 执行指定的sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="notEnsureTransaction">默认创建事务</param>
        /// <param name="timeout">设置命令超时时间</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回操作条数</returns>
        int ExecuteSqlCommand(RawSqlString sql, bool notEnsureTransaction = false, int? timeout = null, params object[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        void Detach<TEntity>(TEntity entity) where TEntity : BaseEntity;
    }
}
