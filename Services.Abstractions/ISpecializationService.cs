using Services.Dtos.Specialization;

namespace Services.Abstractions;

public interface ISpecializationService
{
    Task<IEnumerable<SpecializationDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<SpecializationDto>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);

    Task CreateAsync(SpecializationCreateDto specializationCreateDto, CancellationToken cancellationToken = default);

    Task UpdateAsync(SpecializationDto specializationDto, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}