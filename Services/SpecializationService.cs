using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Mapster;
using Services.Abstractions;
using Services.Dtos.Specialization;

namespace Services;

internal sealed class SpecializationService : ISpecializationService
{
    private readonly IRepositoryManager _repositoryManager;

    public SpecializationService(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }

    public async Task CreateAsync(SpecializationCreateDto specializationCreateDto, CancellationToken cancellationToken = default)
    {
        await ValidateSpecializationDto(specializationCreateDto);

        var specialization = specializationCreateDto.Adapt<Specialization>();

        _repositoryManager.SpecializationRepository.Add(specialization);

        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var spec = await GetSpecializationByIdAsync(id);

        _repositoryManager.SpecializationRepository.Remove(spec);

        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<SpecializationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var specializations = await _repositoryManager.SpecializationRepository.GetAllAsync();

        return specializations.Adapt<IEnumerable<SpecializationDto>>();
    }

    public async Task<SpecializationDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var specialization = await GetSpecializationByIdAsync(id);
        return specialization.Adapt<SpecializationDto>();
    }

    public async Task<IEnumerable<SpecializationDto>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var result = await _repositoryManager.SpecializationRepository
            .ContainsNameAsync(name);

        return result.Select(s => s.Adapt<SpecializationDto>());
    }

    public async Task<(SpecializationDto[] List, int TotalCount)> SpecializationsPaged(int pageNumber, int pageSize)
    {
        var specs = await _repositoryManager.SpecializationRepository
            .SpecializationsPaged(pageNumber, pageSize);
        var totalCount = await _repositoryManager.SpecializationRepository
            .TotalCount();
        var dtos = specs.Select(s => s.Adapt<SpecializationDto>()).ToArray();

        return (List: dtos, TotalCount: totalCount);
    }

    public async Task UpdateAsync(SpecializationDto specializationDto, CancellationToken cancellationToken = default)
    {
        var spec = await GetSpecializationByIdAsync(specializationDto.Id);

        await ValidateSpecializationDto(specializationDto);

        spec.Name = specializationDto.Name;

        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    private async Task<Specialization> GetSpecializationByIdAsync(Guid id)
    {
        var specialization = await _repositoryManager.SpecializationRepository.FindByIdAsync(id)
            ?? throw new SpecializationNotFoundException();

        return specialization;
    }

    private async Task IsExistingAsync(SpecializationCreateDto dto)
    {
        var exists = await _repositoryManager.SpecializationRepository.ExistsAsync(dto.Name);

        if (exists)
            throw new SpecializationDuplicationException("A specialization with the same name already exists.");
    }

    private async Task ValidateSpecializationDto(SpecializationCreateDto dto)
    {
        await IsExistingAsync(dto);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Name, nameof(dto.Name));
    }
}