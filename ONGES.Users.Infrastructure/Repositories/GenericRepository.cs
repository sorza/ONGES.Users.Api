using Microsoft.EntityFrameworkCore;
using ONGES.Users.Application.Repositories;
using ONGES.Users.Domain.Shared.Entities;
using ONGES.Users.Infrastructure.Data;
using System.Linq.Expressions;

namespace ONGES.Users.Infrastructure.Repositories
{
    public class GenericRepository<T>(AppDbContext context) : IRepository<T> where T : Entity
    {
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public Task CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Add(entity);
            return context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
           var existingEntity = await _dbSet.FindAsync(id, cancellationToken);

            if (existingEntity is not null)
            {
                _dbSet.Remove(existingEntity);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AnyAsync(predicate, cancellationToken);

        public async Task<IEnumerable<T>?> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(predicate).ToListAsync(cancellationToken);

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            return context.SaveChangesAsync(cancellationToken);
        }
    }
}
