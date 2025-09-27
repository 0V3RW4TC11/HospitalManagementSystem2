using Services.Dtos.Doctor;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Mapster;
using Services.Abstractions;
using System.Threading.Tasks;

namespace Services;

internal sealed class DoctorService : IDoctorService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IStaffEmailService _staffEmailService;
    private readonly AccountService _accountHelper;
    private readonly DoctorSpecializationService _doctorSpecializationService;

    public DoctorService(
        IRepositoryManager repositoryManager, 
        IStaffEmailService staffEmailService,
        AccountService accountHelper,
        DoctorSpecializationService doctorSpecializationService)
    {
        _repositoryManager = repositoryManager;
        _accountHelper = accountHelper;
        _staffEmailService = staffEmailService;
        _doctorSpecializationService = doctorSpecializationService;
    }

    public async Task<(DoctorDto[] List, int TotalCount)> Doctors(int pageNumber, int pageSize)
    {
        var doctors = await _repositoryManager.DoctorRepository.Doctors(pageNumber, pageSize);
        var dtos = doctors
            .Select(d => d.Adapt<DoctorDto>())
            .ToArray();
        var totalCount = await _repositoryManager.DoctorRepository.GetTotalCount();

        return (List: dtos, TotalCount: totalCount);
    }

    public async Task<DoctorDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var doctor = await GetDoctorFromIdAsync(id);

        var doctorDto = doctor.Adapt<DoctorDto>();

        doctorDto.SpecializationIds = await _repositoryManager.DoctorSpecializationRepository
            .GetSpecIdsByDoctorIdAsync(doctor.Id);

        return doctorDto;
    }

    public async Task CreateAsync(DoctorCreateDto doctorCreateDto, CancellationToken cancellationToken = default)
    {
        await ValidateDoctorCreateDtoAsync(doctorCreateDto);
        
        await _repositoryManager.UnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var doctor = doctorCreateDto.Adapt<Doctor>();
            _repositoryManager.DoctorRepository.Add(doctor);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
            
            await _doctorSpecializationService.UpdateAsync(doctor.Id, doctorCreateDto.SpecializationIds);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
            
            var username = await _staffEmailService.CreateStaffEmailAsync(doctorCreateDto.FirstName, doctorCreateDto.LastName);

            await _accountHelper.CreateAsync(doctor.Id, Constants.AuthRoles.Doctor, username, doctorCreateDto.Password);
        });
    }
    
    public async Task UpdateAsync(DoctorDto doctorDto, CancellationToken cancellationToken = default)
    {
        var doctor = await GetDoctorFromIdAsync(doctorDto.Id);
        
        await ValidateDoctorDto(doctorDto);
        
        doctor.FirstName = doctorDto.FirstName;
        doctor.LastName = doctorDto.LastName;
        doctor.Gender = doctorDto.Gender;
        doctor.Address = doctorDto.Address;
        doctor.Phone = doctorDto.Phone;
        doctor.Email = doctorDto.Email;
        doctor.DateOfBirth = doctorDto.DateOfBirth;

        await _doctorSpecializationService.UpdateAsync(doctor.Id, doctorDto.SpecializationIds);
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var doctor = await GetDoctorFromIdAsync(id);

        await _repositoryManager.UnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            _repositoryManager.DoctorRepository.Remove(doctor);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
            await _accountHelper.DeleteAsync(doctor.Id);
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
            throw new DoctorDuplicationException($"Email {dto.Email} is used by another Doctor.");

        ValidateDoctorBaseDto(dto);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Password, nameof(dto.Password));
        await ValidateSpecializations(dto.SpecializationIds);
    }
    
    private async Task ValidateDoctorDto(DoctorDto dto)
    {
        ValidateDoctorBaseDto(dto);
        await ValidateSpecializations(dto.SpecializationIds);
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

    private async Task ValidateSpecializations(IEnumerable<Guid>? specializationIds)
    {
        if (specializationIds == null || !specializationIds.Any())
            throw new Exception("Doctor must have at least one specialization.");

        var isValidIds = await _repositoryManager.SpecializationRepository
            .IsExistingIdsAsync(specializationIds);
        if (isValidIds is false)
            throw new Exception("One or more Specialization Ids are invalid");
    }
    
    private async Task<bool> IsExistingAsync(DoctorCreateDto doctorCreateDto)
    {
        return await _repositoryManager.DoctorRepository.ExistsAsync(d =>
            d.Email.ToLower() == doctorCreateDto.Email.ToLower());
    }
}