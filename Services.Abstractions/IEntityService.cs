using X.PagedList;

namespace Services.Abstractions
{
    public interface IEntityService<TDto, TCreateDto>
    {
        Task CreateAsync(TCreateDto dto, CancellationToken ct = default);

        Task DeleteAsync(Guid id, CancellationToken ct = default);

        Task<TDto> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<IPagedList<TDto>> GetPagedListAsync(int pageNumber, int pageSize, CancellationToken ct = default);

        Task UpdateAsync(TDto dto, CancellationToken ct = default);
    }
}
