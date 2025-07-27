using DataTransfer.Specialization;
using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Mapster;
using Services.Abstractions;

namespace Services;

internal sealed class SpecializationService : ISpecializationService
{
    private readonly IUnitOfWork _unitOfWork;

    public SpecializationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(SpecializationCreateDto specializationCreateDto, CancellationToken cancellationToken = default)
    {
        ValidateSpecializationCreateDto(specializationCreateDto);
        
        var specialization = specializationCreateDto.Adapt<Specialization>();

        _unitOfWork.SpecializationRepository.Add(specialization);
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<SpecializationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var specializations = await _unitOfWork.SpecializationRepository.GetAllAsync();
        
        return specializations.Adapt<IEnumerable<SpecializationDto>>();
    }

    public async Task UpdateAsync(SpecializationDto specializationDto, CancellationToken cancellationToken = default)
    {
        var spec = await GetSpecializationFromIdAsync(specializationDto.Id);
        
        ValidateSpecializationCreateDto(specializationDto);
        
        spec.Name = specializationDto.Name;
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var spec = await GetSpecializationFromIdAsync(id);
        
        _unitOfWork.SpecializationRepository.Remove(spec);
        
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Specialization> GetSpecializationFromIdAsync(Guid id)
    {
        var specialization = await _unitOfWork.SpecializationRepository.FindByIdAsync(id) 
            ?? throw new SpecNotFoundException(id.ToString());
        
        return specialization;
    }

    private static void ValidateSpecializationCreateDto(SpecializationCreateDto dto)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Name, nameof(dto.Name));
    }
}