using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FreelanceManager.Data.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace FreelanceManager.Data.Repositories
{
    public class AppRepository<TEntity> : IAppRepository<TEntity> where TEntity : EntityBase
    {
        protected readonly DbSet<TEntity> EntitySet;
        protected readonly ApplicationDbContext _context;

        public AppRepository(ApplicationDbContext context)
        {
            EntitySet = context.Set<TEntity>();
            _context = context;
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            if (entity is not null)
            {
                await EntitySet.AddAsync(entity);
                await _context.SaveChangesAsync();
            }

            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> CreateAsync(IEnumerable<TEntity> entities)
        {
            if (entities is not null && entities.Any())
            {
                await EntitySet.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
            }

            return entities;
        }

        public async virtual Task<TEntity> Edit(TEntity entity)
        {
            if (entity is not null)
            {
                EntitySet.Update(entity);
                await _context.SaveChangesAsync();
            }

            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> Edit(IEnumerable<TEntity> entity)
        {
            if (entity is not null && entity.Any())
            {
                EntitySet.UpdateRange(entity);
                await _context.SaveChangesAsync();
            }

            return entity;
        }

        public virtual async Task<TEntity> FindAsync(Guid id) => await EntitySet.FindAsync(id);

        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> where) => await WhereIQueryable(where).FirstOrDefaultAsync();

        public IQueryable<TEntity> GetEntity() => EntitySet.Where(WhereIQueryableIsSystem).Where(WhereIQueryableIsDeleted).AsQueryable();

        public IQueryable<TEntity> GetEntity(Expression<Func<TEntity, bool>> where) => EntitySet.Where(WhereIQueryableIsSystem).Where(WhereIQueryableIsDeleted).Where(where);

        public IQueryable<TEntity> GetEntityAsNoTracking() => EntitySet.Where(WhereIQueryableIsSystem).Where(WhereIQueryableIsDeleted).AsNoTracking();

        public IQueryable<TEntity> GetEntityAsNoTracking(Expression<Func<TEntity, bool>> where) => EntitySet.Where(WhereIQueryableIsSystem).Where(WhereIQueryableIsDeleted).Where(where).AsNoTracking();

        public IQueryable<TEntity> GetEntityAsNoTrackingWithoutIsSystemWhere(Expression<Func<TEntity, bool>> where) => EntitySet.Where(WhereIQueryableIsDeleted).Where(where).AsNoTracking();
        public IQueryable<TEntity> GetEntityAsNoTrackingWithoutIsDeletedWhere(Expression<Func<TEntity, bool>> where) => EntitySet.Where(WhereIQueryableIsSystem).Where(where).AsNoTracking();

        public virtual async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> where) => await WhereIQueryable(where).ToListAsync();

        public async Task<IEnumerable<TEntity>> GetListAsync<TKey>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> order, bool ascending = true)
            => await (ascending ? this.WhereIQueryable(where).OrderBy(order).ToListAsync() : this.WhereIQueryable(where).OrderByDescending(order).ToListAsync());

        protected IQueryable<TEntity> WhereIQueryable(Expression<Func<TEntity, bool>> where) => EntitySet.Where(WhereIQueryableIsSystem).Where(WhereIQueryableIsDeleted).Where(where);

        private Expression<Func<TEntity, bool>> WhereIQueryableIsSystem => entity => entity.IsSystem != true;

        private Expression<Func<TEntity, bool>> WhereIQueryableIsDeleted => entity => entity.IsDeleted != true;

        #region delete
        public virtual async Task<TEntity> Delete(Guid id)
        {
            var entity = await FindAsync(id);

            if (entity is not null)
            {
                EntitySet.Remove(entity);
                await _context.SaveChangesAsync();
            }

            return entity;
        }

        public virtual async Task<TEntity> FakeDelete(Guid id)
        {
            var entity = await FindAsync(id);

            if (entity is not null)
            {
                entity.IsDeleted = true;
                await _context.SaveChangesAsync();
            }

            return entity;
        }

        public virtual async Task<TEntity> Delete(TEntity entity)
        {
            if (entity is not null)
            {
                //EntitySet.Remove(entity);
                entity.IsDeleted = true;
                await _context.SaveChangesAsync();
            }

            return entity;
        }

        public virtual async Task<TEntity> FakeDelete(TEntity entity)
        {
            if (entity is not null)
            {
                //EntitySet.Remove(entity);
                entity.IsDeleted = true;
                await _context.SaveChangesAsync();
            }

            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> Delete(IEnumerable<TEntity> entity)
        {
            if (entity is not null && entity.Any())
            {
                EntitySet.RemoveRange(entity);
                await _context.SaveChangesAsync();
            }

            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> FakeDelete(IEnumerable<TEntity> entity)
        {
            if (entity is not null && entity.Any())
            {
                EntitySet.RemoveRange(entity);
                await _context.SaveChangesAsync();
            }

            return entity;
        }
        #endregion
    }
}