using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VoteBirthy.Data;
using VoteBirthy.Models;

namespace VoteBirthy.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _dbContext;

        public EfRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>()
                .AsNoTracking()
                .ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
                
            return await _dbContext.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            // FindAsync doesn't support AsNoTracking, but it's more efficient for simple key lookups
            // For repositories that need full AsNoTracking support, they should override this method
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                
            await _dbContext.Set<T>().AddAsync(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                
            _dbContext.Set<T>().Update(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _dbContext.Set<T>().Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
} 