using DataTransfer.Admin;
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
        await ValidateDoctorCreateDtoAsync(doctorCreateDto);
        
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
        
        doctorDto.SpecializationIds = await _repositoryManager.DoctorSpecializationRepository
            .GetSpecIdsByDoctorIdAsync(doctor.Id);
        
        return doctorDto;
    }
    
    public async Task UpdateAsync(DoctorDto doctorDto, CancellationToken cancellationToken = default)
    {
        var doctor = await GetDoctorFromIdAsync(doctorDto.Id);
        
        ValidateDoctorDto(doctorDto);
        
        doctor.FirstName = doctorDto.FirstName;
        doctor.LastName = doctorDto.LastName;
        doctor.Gender = doctorDto.Gender;
        doctor.Address = doctorDto.Address;
        doctor.Phone = doctorDto.Phone;
        doctor.Email = doctorDto.Email;
        doctor.DateOfBirth = doctorDto.DateOfBirth;

        await _repositoryManager.DoctorSpecializationRepository.UpdateAsync(doctor.Id, doctorDto.SpecializationIds);
        
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
        
        return doctor ?? throw new DoctorNotFoundException();
    }

    private async Task ValidateDoctorCreateDtoAsync(DoctorCreateDto dto)
    {
        if (await IsExistingAsync(dto))
            throw new Exception($"Email {dto.Email} is used by another Doctor.");
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Password, nameof(dto.Password));
        ValidateDoctorBaseDto(dto);
        ValidateSpecializations(dto.SpecializationIds);
    }
    
    private static void ValidateDoctorDto(DoctorDto dto)
    {
        ValidateDoctorBaseDto(dto);
        ValidateSpecializations(dto.SpecializationIds);
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

    private static void ValidateSpecializations(IEnumerable<Guid> specializationIds)
    {
        if (!specializationIds.Any())
            throw new Exception("Doctor must have at least one specialization.");
    }
    
    private async Task<bool> IsExistingAsync(DoctorCreateDto doctorCreateDto)
    {
        return await _repositoryManager.DoctorRepository.ExistsAsync(d =>
            d.Email.ToLower() == doctorCreateDto.Email.ToLower());
    }
}