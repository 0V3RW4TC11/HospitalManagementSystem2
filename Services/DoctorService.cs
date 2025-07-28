using DataTransfer.Doctor;
using DataTransfer.Specialization;
using Domain;
using Domain.Constants;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Mapster;
using Services.Abstractions;

namespace Services;

internal sealed class DoctorService : IDoctorService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IAccountService _accountService;
    private readonly IStaffEmailService _staffEmailService;

    public DoctorService(
        IRepositoryManager repositoryManager, 
        IAccountService accountService, 
        IStaffEmailService staffEmailService)
    {
        _repositoryManager = repositoryManager;
        _accountService = accountService;
        _staffEmailService = staffEmailService;
    }

    public async Task CreateAsync(DoctorCreateDto doctorCreateDto, CancellationToken cancellationToken = default)
    {
        await ValidateDoctorCreateDto(doctorCreateDto);
        
        await _repositoryManager.UnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var doctor = doctorCreateDto.Adapt<Doctor>();
            _repositoryManager.DoctorRepository.Add(doctor);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
            
            await _repositoryManager.DoctorSpecializationRepository.UpdateAsync(doctor.Id, doctorCreateDto.SpecializationIds);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
            
            var username = await _staffEmailService.CreateStaffEmailAsync(doctorCreateDto.FirstName, doctorCreateDto.LastName);

            await _accountService.CreateAsync(doctor.Id, AuthRoles.Doctor, username, doctorCreateDto.Password);
        });
    }

    public async Task<DoctorDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var doctor = await GetDoctorFromIdAsync(id);
        
        var doctorDto = doctor.Adapt<DoctorDto>();
        
        var specIds = await _repositoryManager.DoctorSpecializationRepository
            .GetSpecIdsByDoctorIdAsync(doctor.Id);
        
        var specs = await _repositoryManager.SpecializationRepository.GetFromIdsAsync(specIds);
        
        doctorDto.Specializations = specs.Adapt<IEnumerable<SpecializationDto>>();
        
        return doctorDto;
    }
    
    public async Task UpdateAsync(DoctorUpdateDto doctorUpdateDto, CancellationToken cancellationToken = default)
    {
        var doctor = await GetDoctorFromIdAsync(doctorUpdateDto.Id);
        
        await ValidateDoctorUpdateDto(doctorUpdateDto);
        
        doctor.FirstName = doctorUpdateDto.FirstName;
        doctor.LastName = doctorUpdateDto.LastName;
        doctor.Gender = doctorUpdateDto.Gender;
        doctor.Address = doctorUpdateDto.Address;
        doctor.Phone = doctorUpdateDto.Phone;
        doctor.Email = doctorUpdateDto.Email;
        doctor.DateOfBirth = doctorUpdateDto.DateOfBirth;

        await _repositoryManager.DoctorSpecializationRepository.UpdateAsync(doctor.Id, doctorUpdateDto.SpecializationIds);
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var doctor = await GetDoctorFromIdAsync(id);

        await _repositoryManager.UnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            _repositoryManager.DoctorRepository.Remove(doctor);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
            await _accountService.DeleteByUserIdAsync(doctor.Id);
        });
    }

    private async Task<Doctor> GetDoctorFromIdAsync(Guid id)
    {
        var doctor = await _repositoryManager.DoctorRepository.FindByIdAsync(id);
        if (doctor is null)
            throw new DoctorNotFoundException(id.ToString());
        return doctor;
    }

    private static void ValidateDoctorBaseDto(DoctorBaseDto baseDto)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.FirstName, nameof(baseDto.FirstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.LastName, nameof(baseDto.LastName));
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.Gender, nameof(baseDto.Gender));
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.Address, nameof(baseDto.Address));
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.Phone, nameof(baseDto.Phone));
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.Email, nameof(baseDto.Email));
    }
    
    private async Task ValidateDoctorCreateDto(DoctorCreateDto doctorCreateDto)
    {
        ValidateDoctorBaseDto(doctorCreateDto);
        
        ArgumentException.ThrowIfNullOrWhiteSpace(doctorCreateDto.Password, nameof(doctorCreateDto.Password));

        await ValidateSpecializations(doctorCreateDto.SpecializationIds);
    }
    
    private async Task ValidateDoctorUpdateDto(DoctorUpdateDto doctorUpdateDto)
    {
        ValidateDoctorBaseDto(doctorUpdateDto);
        
        await ValidateSpecializations(doctorUpdateDto.SpecializationIds);
    }

    private async Task ValidateSpecializations(IEnumerable<Guid> specializationIds)
    {
        var specSet = specializationIds.ToHashSet();

        if (specSet.Count == 0)
            throw new Exception("Specializations are required.");

        foreach (var specId in specSet)
        {
            var isExisting = await _repositoryManager.SpecializationRepository.ContainsAsync(specId);
            if (!isExisting)
                throw new SpecNotFoundException(specId.ToString());
        }
    }
}