using DataTransfer.Patient;
using Domain;
using Domain.Constants;
using Domain.Entities;
using Domain.Exceptions;
using Mapster;
using Services.Abstractions;
using Services.Helpers;

namespace Services;

internal sealed class PatientService : IPatientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountService _accountService;

    public PatientService(IUnitOfWork unitOfWork, IAccountService accountService)
    {
        _unitOfWork = unitOfWork;
        _accountService = accountService;
    }
    
    public async Task CreateAsync(PatientCreateDto patientCreateDto, CancellationToken cancellationToken = default)
    {
        ValidatePatientCreateDto(patientCreateDto);
        
        await TransactionHelper.ExecuteInTransactionAsync(_unitOfWork, async () =>
        {
            var patient = patientCreateDto.Adapt<Patient>();
            _unitOfWork.PatientRepository.Add(patient);
            await _unitOfWork.SaveChangesAsync();

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
        
        ValidatePatientBaseDto(patientDto);
        
        patient.FirstName = patientDto.FirstName;
        patient.LastName = patientDto.LastName;
        patient.Gender = patientDto.Gender;
        patient.Address = patientDto.Address;
        patient.Phone = patientDto.Phone;
        patient.Email = patientDto.Email;
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await GetPatientByIdAsync(id);

        await TransactionHelper.ExecuteInTransactionAsync(_unitOfWork, async () =>
        {
            _unitOfWork.PatientRepository.Remove(patient);
            await _unitOfWork.SaveChangesAsync();
            
            await _accountService.DeleteByUserIdAsync(patient.Id);
        });
    }

    private async Task<Patient> GetPatientByIdAsync(Guid id)
    {
        var patient = await _unitOfWork.PatientRepository.FindByIdAsync(id);
        if (patient is null)
            throw new PatientNotFoundException(id.ToString());

        return patient;
    }

    private static void ValidatePatientBaseDto(PatientBaseDto baseDto)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.FirstName, nameof(baseDto.FirstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.Gender, nameof(baseDto.Gender));
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.Email, nameof(baseDto.Email));
    }
    
    private static void ValidatePatientCreateDto(PatientCreateDto patientCreateDto)
    {
        ValidatePatientBaseDto(patientCreateDto);
        ArgumentException.ThrowIfNullOrWhiteSpace(patientCreateDto.Password, nameof(patientCreateDto.Password));
    }
}