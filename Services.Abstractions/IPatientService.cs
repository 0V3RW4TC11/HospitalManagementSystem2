using DataTransfer.Patient;

namespace Services.Abstractions;

public interface IPatientService
{
    Task CreateAsync(PatientCreateDto patientCreateDto, CancellationToken cancellationToken = default);
    
    Task<PatientDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(PatientDto patientDto, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}