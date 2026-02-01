using Ardalis.Specification;

namespace Abstractions
{
    public interface IRepository<T> : IRepositoryBase<T> where T : class
    {
        Task<T?> IReadRepositoryBase<T>.GetByIdAsync<TId>(TId id, CancellationToken cancellationToken)
        {
            throw new NotSupportedException("GetByIdAsync is not supported. Use FirstOrDefault or SingleOrDefault instead.");
        }

        Task<int> IRepositoryBase<T>.SaveChangesAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException("SaveChangesAsync is not supported. Use SaveChangesAsync in UnitOfWork instead.");
        }
    }
}