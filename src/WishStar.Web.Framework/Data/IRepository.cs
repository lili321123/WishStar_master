using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace WishStar.Web.Framework.Data
{
    public interface IRepository<TEntity> 
        where TEntity:BaseEntity
    {
        /// <summary>
        /// 通过主键获取唯一数据行
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetById(object id);

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveChanges">是否立即保存</param>
        void Insert(TEntity entity, bool saveChanges = true);

        /// <summary>
        /// 新增实体集合
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="saveChanges">是否立即保存</param>
        void Insert(IEnumerable<TEntity> entitys, bool saveChanges = true);

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveChanges">是否立即保存</param>
        void Update(TEntity entity, bool saveChanges = true);

        /// <summary>
        /// 修改实体集合
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="saveChanges">是否立即保存</param>
        void Update(IEnumerable<TEntity> entitys, bool saveChanges = true);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveChanges">是否立即保存</param>
        void Delete(TEntity entity, bool saveChanges = true);

        /// <summary>
        /// 删除实体集合
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="saveChanges">是否立即保存</param>
        void Delete(IEnumerable<TEntity> entitys, bool saveChanges = true);

        /// <summary>
        /// 保存实体
        /// 根据实体isnew确定操作类型
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveChanges">是否立即保存</param>
        void Save(TEntity entity, bool saveChanges = true);

        /// <summary>
        /// 保存实体集合
        /// 根据实体isnew确定操作类型
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="saveChanges">是否立即保存</param>
        void Save(IEnumerable<TEntity> entitys, bool saveChanges = true);

        /// <summary>
        /// 获取当前实体表数据
        /// </summary>
        IQueryable<TEntity> Table { get; }

        /// <summary>
        /// 获取当前无ef特性跟踪实体表数据
        /// </summary>
        IQueryable<TEntity> TableNoTracking { get; }

        /// <summary>
        /// 根据条件和选择字段修改
        /// </summary>
        /// <param name="expression">修改条件</param>
        /// <param name="value">修改字段值</param>
        /// <returns></returns>
        int Update(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TEntity>> value);

        /// <summary>
        /// 自定义字段修改
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        int Update(Expression<Func<TEntity, TEntity>> value);

        /// <summary>
        /// 批量删除操作
        /// </summary>
        /// <param name="expression">删除条件</param>
        /// <returns></returns>
        int Delete(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 删除全部
        /// </summary>
        /// <returns></returns>
        int DeleteAll();
    }
}
