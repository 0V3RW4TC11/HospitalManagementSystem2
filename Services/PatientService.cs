using DataTransfer.Patient;
using Domain;
using Domain.Constants;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Mapster;
using Services.Abstractions;

namespace Services;

internal sealed class PatientService : IPatientService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IAccountService _accountService;

    public PatientService(IRepositoryManager repositoryManager, IAccountService accountService)
    {
        _repositoryManager = repositoryManager;
        _accountService = accountService;
    }
    
    public async Task CreateAsync(PatientCreateDto patientCreateDto, CancellationToken cancellationToken = default)
    {
        ValidatePatientCreateDto(patientCreateDto);
        
        await _repositoryManager.UnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var patient = patientCreateDto.Adapt<Patient>();
            _repositoryManager.PatientRepository.Add(patient);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            await _accountService.CreateAsync(
                patient.Id,
                AuthRoles.Patient,
                patientCreateDto.Email,
                patientCreateDto.Password);
        });
    }

    public async Task<PatientDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await GetPatientByIdAsync(id);
        
        return patient.Adapt<PatientDto>();
    }

    public async Task UpdateAsync(PatientDto patientDto, CancellationToken cancellationToken = default)
    {
        var patient = await GetPatientByIdAsync(patientDto.Id);
        
        ValidatePatientDto(patientDto);
        
        patient.FirstName = patientDto.FirstName;
        patient.LastName = patientDto.LastName;
        patient.Gender = patientDto.Gender;
        patient.Address = patientDto.Address;
        patient.Phone = patientDto.Phone;
        patient.Email = patientDto.Email;
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await GetPatientByIdAsync(id);

        await _repositoryManager.UnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            _repositoryManager.PatientRepository.Remove(patient);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
            
            await _accountService.DeleteByUserIdAsync(patient.Id);
        });
    }

    private async Task<Patient> GetPatientByIdAsync(Guid id)
    {
        var patient = await _repositoryManager.PatientRepository.FindByIdAsync(id);
        if (patient is null)
            throw new PatientNotFoundException(id.ToString());

        return patient;
    }
    
    private static void ValidatePatientCreateDto(PatientCreateDto patientCreateDto)
    {
        try
        {
            ValidatePatientBaseDto(patientCreateDto);
            ArgumentException.ThrowIfNullOrWhiteSpace(patientCreateDto.Password, nameof(patientCreateDto.Password));
        }
        catch (Exception e)
        {
            throw new PatientBadRequestException(e.Message);
        }
    }

    private static void ValidatePatientDto(PatientDto patientDto)
    {
        try
        {
            ValidatePatientBaseDto(patientDto);
        }
        catch (Exception e)
        {
            throw new PatientBadRequestException(e.Message);
        }
    }

    private static void ValidatePatientBaseDto(PatientBaseDto baseDto)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.FirstName, nameof(baseDto.FirstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.Gender, nameof(baseDto.Gender));
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.Email, nameof(baseDto.Email));
    }
}