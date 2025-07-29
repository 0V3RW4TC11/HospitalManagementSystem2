using DataTransfer.Admin;

namespace Services.Abstractions;

public interface IAdminService
{
    Task CreateAsync(AdminCreateDto adminCreateDto, CancellationToken cancellationToken = default);
    
    Task<AdminDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(AdminDto adminDto, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}