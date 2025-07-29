using DataTransfer.Doctor;

namespace Services.Abstractions;

public interface IDoctorService
{
    Task CreateAsync(DoctorCreateDto doctorCreateDto, CancellationToken cancellationToken = default);
    
    Task<DoctorDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(DoctorDto doctorDto, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}