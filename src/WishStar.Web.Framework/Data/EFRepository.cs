using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace WishStar.Web.Framework.Data
{
    public partial class EFRepository<TEntity> : IRepository<TEntity>
        where TEntity : BaseEntity
    {
        private readonly IDbContext _dbContext;
        private DbSet<TEntity> _entities;

        public EFRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected virtual DbSet<TEntity> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = _dbContext.Set<TEntity>();
                }
                return _entities;
            }
        }

        public IQueryable<TEntity> Table => this.Entities;

        public IQueryable<TEntity> TableNoTracking => this.Entities.AsNoTracking();

        public void Delete(TEntity entity, bool saveChanges = true)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            try
            {
                Entities.Remove(entity);
                if (saveChanges)
                {
                    _dbContext.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetErrorMsgAndRollbackEntityChanges(ex, saveChanges), ex);
            }
        }

        public void Delete(IEnumerable<TEntity> entitys, bool saveChanges = true)
        {
            if (entitys == null)
            {
                throw new ArgumentNullException(nameof(entitys));
            }
            try
            {
                Entities.RemoveRange(entitys);
                if (saveChanges)
                {
                    _dbContext.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetErrorMsgAndRollbackEntityChanges(ex, saveChanges), ex);
            }
        }

        public int DeleteAll()
        {
            Entities.RemoveRange(Entities);
            return this._dbContext.SaveChanges();
        }

        public TEntity GetById(object id)
        {
            return this.Entities.Find(id);
        }

        public void Insert(TEntity entity, bool saveChanges = true)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            try
            {
                Entities.Add(entity);
                if (saveChanges)
                {
                    _dbContext.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetErrorMsgAndRollbackEntityChanges(ex, saveChanges), ex);
            }
        }

        public void Insert(IEnumerable<TEntity> entitys, bool saveChanges = true)
        {
            if (entitys == null)
            {
                throw new ArgumentNullException(nameof(entitys));
            }
            try
            {
                Entities.AddRange(entitys);
                if (saveChanges)
                {
                    _dbContext.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetErrorMsgAndRollbackEntityChanges(ex, saveChanges), ex);
            }
        }

        public void Save(TEntity entity, bool saveChanges = true)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.IsNew)
            {
                this.Insert(entity, saveChanges);
            }
            else
            {
                this.Update(entity, saveChanges);
            }
        }

        public void Save(IEnumerable<TEntity> entitys, bool saveChanges = true)
        {
            if (entitys == null)
            {
                throw new ArgumentNullException(nameof(entitys));
            }
            foreach (var entity in entitys)
            {
                this.Save(entity, saveChanges);
            }
        }

        public void Update(TEntity entity, bool saveChanges = true)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            try
            {
                Entities.Update(entity);
                if (saveChanges)
                {
                    _dbContext.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetErrorMsgAndRollbackEntityChanges(ex, saveChanges), ex);
            }
        }

        public void Update(IEnumerable<TEntity> entitys, bool saveChanges = true)
        {
            if (entitys == null)
            {
                throw new ArgumentNullException(nameof(entitys));
            }
            try
            {
                Entities.UpdateRange(entitys);
                if (saveChanges)
                {
                    _dbContext.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetErrorMsgAndRollbackEntityChanges(ex, saveChanges), ex);
            }
        }

        [Obsolete("未实现，暂不适用")]
        public int Update(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TEntity>> value)
        {
            throw new NotImplementedException();
        }

        [Obsolete("未实现，暂不适用")]
        public int Update(Expression<Func<TEntity, TEntity>> value)
        {
            throw new NotImplementedException();
        }

        [Obsolete("未实现，暂不适用")]
        public int Delete(Expression<Func<TEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 异常回滚
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="exception"></param>
        /// <param name="saveChanges"></param>
        /// <returns></returns>
        protected virtual string GetErrorMsgAndRollbackEntityChanges<T>(T exception, bool saveChanges)
            where T : Exception
        {
            if (saveChanges && this._dbContext is DbContext dbContext)
            {
                var entries = dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                    .ToList();
                entries.ForEach(x => x.State = EntityState.Unchanged);
                _dbContext.SaveChanges();
            }
            return exception.ToString();
        }

    }
}
