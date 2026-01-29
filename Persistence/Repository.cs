using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Domain;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Persistence
{
    internal class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> _set;

        public Repository(RepositoryDbContext context)
        {
            _set = context.Set<T>();
        }

        public async Task AddAsync(T entity, CancellationToken ct = default)
        {
            await _set.AddAsync(entity, ct);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            await _set.AddRangeAsync(entities, ct);
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

        Task<T?> IReadRepositoryBase<T>.GetByIdAsync<TId>(TId id, CancellationToken ct)
        {
            throw new NotSupportedException("GetByIdAsync<TId> is not supported. Use FirstOrDefaultAsync<T> or SingleOrDefaultAsync<T> instead.");
        }

        Task<T?> IReadRepositoryBase<T>.GetBySpecAsync(ISpecification<T> specification, CancellationToken ct)
        {
            throw new NotSupportedException("GetBySpecAsync is not supported. Use FirstOrDefaultAsync<T> or SingleOrDefaultAsync<T> instead.");
        }

        Task<TResult?> IReadRepositoryBase<T>.GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken ct) where TResult : default
        {
            throw new NotSupportedException("GetBySpecAsync<TResult> is not supported. Use FirstOrDefaultAsync<T> or SingleOrDefaultAsync<T> instead.");
        }

        public async Task<IPagedList<T>> PagedListAsync(ISpecification<T> specification, int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var query = ApplySpecification(specification).AsNoTracking();
            return await GetPaged(query, pageNumber, pageSize, ct);
        }

        public async Task<IPagedList<TResult>> PagedListAsync<TResult>(ISpecification<T, TResult> specification, int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var query = ApplySpecification(specification);
            return await GetPaged(query, pageNumber, pageSize, ct);
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

        public Task RemoveAsync(T entity, CancellationToken ct = default)
        {
            _set.Remove(entity);
            return Task.CompletedTask;
        }

        public Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            _set.RemoveRange(entities);
            return Task.CompletedTask;
        }

        public async Task RemoveRangeAsync(ISpecification<T> specification, CancellationToken ct = default)
        {
            var entities = await ApplySpecification(specification).ToListAsync(ct);
            _set.RemoveRange(entities);
        }

        public async Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).SingleOrDefaultAsync(ct);
        }

        public async Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).SingleOrDefaultAsync(ct);
        }

        public Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            _set.Update(entity);
            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            _set.UpdateRange(entities);
            return Task.CompletedTask;
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> specification)
        {
            return SpecificationEvaluator.Default.GetQuery(_set, specification);
        }

        private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification)
        {
            return SpecificationEvaluator.Default.GetQuery(_set, specification);
        }

        private static async Task<StaticPagedList<TPaged>> GetPaged<TPaged>(IQueryable<TPaged> query, int pageNumber, int pageSize, CancellationToken ct)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 0 ? 5 : pageSize;

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new StaticPagedList<TPaged>(items, pageNumber, pageSize, totalCount);
        }
    }
}
