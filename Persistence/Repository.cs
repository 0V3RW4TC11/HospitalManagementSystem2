using Abstractions;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    internal class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> _set;

        public Repository(RepositoryDbContext context)
        {
            _set = context.Set<T>();
        }

        public Task<T> AddAsync(T entity, CancellationToken ct = default)
        {
            _set.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            _set.AddRange(entities);
            return Task.FromResult(entities);
        }

        public async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).AnyAsync(ct);
        }

        public async Task<bool> AnyAsync(CancellationToken ct = default)
        {
            return await _set.AnyAsync(ct);
        }

        public IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification)
        {
            return ApplySpecification(specification).AsNoTracking().AsAsyncEnumerable();
        }

        public async Task<int> CountAsync(ISpecification<T> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).CountAsync(ct);
        }

        public async Task<int> CountAsync(CancellationToken ct = default)
        {
            return await _set.CountAsync(ct);
        }

        public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).AsNoTracking().FirstOrDefaultAsync(ct);
        }

        public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync(ct);
        }

        public async Task<List<T>> ListAsync(CancellationToken ct = default)
        {
            return await _set.AsNoTracking().ToListAsync(ct);
        }

        public async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).AsNoTracking().ToListAsync(ct);
        }

        public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).ToListAsync(ct);
        }

        public Task<int> DeleteAsync(T entity, CancellationToken ct = default)
        {
            _set.Remove(entity);
            return Task.FromResult(1);
        }

        public Task<int> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            _set.RemoveRange(entities);
            return Task.FromResult(entities.Count());
        }

        public async Task<int> DeleteRangeAsync(ISpecification<T> specification, CancellationToken ct = default)
        {
            var entities = await ApplySpecification(specification).ToListAsync(ct);
            _set.RemoveRange(entities);
            return entities.Count;
        }

        public async Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).SingleOrDefaultAsync(ct);
        }

        public async Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).SingleOrDefaultAsync(ct);
        }

        public Task<int> UpdateAsync(T entity, CancellationToken ct = default)
        {
            _set.Update(entity);
            return Task.FromResult(1);
        }

        public Task<int> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            _set.UpdateRange(entities);
            return Task.FromResult(entities.Count());
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> specification)
        {
            return SpecificationEvaluator.Default.GetQuery(_set, specification);
        }

        private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification)
        {
            return SpecificationEvaluator.Default.GetQuery(_set, specification);
        }
    }
}
