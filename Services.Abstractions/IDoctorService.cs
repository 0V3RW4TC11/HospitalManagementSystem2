using DataTransfer.Doctor;

namespace Services.Abstractions;

public interface IDoctorService
{
    Task<(DoctorDto[] List, int TotalCount)> Doctors(int pageNumber, int pageSize);

    Task<DoctorDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task CreateAsync(DoctorCreateDto doctorCreateDto, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(DoctorDto doctorDto, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}