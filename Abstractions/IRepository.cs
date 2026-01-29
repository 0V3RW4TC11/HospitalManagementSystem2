namespace Abstractions
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);

        Task RemoveAsync(T entity);

        Task UpdateAsync(T entity);
    }
}