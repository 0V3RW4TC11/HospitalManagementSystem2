using DataTransfer.Attendance;
using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Mapster;
using Services.Abstractions;

namespace Services;

internal sealed class AttendanceService : IAttendanceService
{
    private readonly IRepositoryManager _repositoryManager;

    public AttendanceService(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }

    public async Task CreateAsync(AttendanceCreateDto attendanceCreateDto)
    {
        await ValidateAttendanceCreateDto(attendanceCreateDto);
        
        var attendance = attendanceCreateDto.Adapt<Attendance>();
        
        _repositoryManager.AttendanceRepository.Add(attendance);
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<AttendanceShortDto>> GetAllByPatientIdAsync(Guid id)
    {
        var attendances = 
            await _repositoryManager.AttendanceRepository.FindShortAttendancesByPatientIdAsync(id);
        
        if (!attendances.Any())
            throw new AttendanceNotFoundForPatientException(id.ToString());

        return attendances;
    }

    public async Task<IEnumerable<AttendanceShortDto>> GetAllByDoctorIdAsync(Guid id)
    {
        var attendances = 
            await _repositoryManager.AttendanceRepository.FindShortAttendancesByDoctorIdAsync(id);
        
        if (!attendances.Any())
            throw new AttendanceNotFoundForDoctorException(id.ToString());

        return attendances;
    }
    
    public async Task<AttendanceDto> GetByIdAsync(Guid id)
    {
        var attendance = await GetAttendanceFromIdAsync(id);
        
        return attendance.Adapt<AttendanceDto>();
    }

    public async Task UpdateAsync(AttendanceDto attendanceDto)
    {
        var attendance = await GetAttendanceFromIdAsync(attendanceDto.Id);
        
        await ValidateAttendanceCreateDto(attendanceDto);
        
        attendance.Diagnosis = attendanceDto.Diagnosis;
        attendance.Remarks = attendanceDto.Remarks;
        attendance.Therapy = attendanceDto.Therapy;
        attendance.DateTime = attendanceDto.DateTime;
        attendance.PatientId = attendanceDto.PatientId;
        attendance.DoctorId = attendanceDto.DoctorId;
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var attendance = await GetAttendanceFromIdAsync(id);
        
        _repositoryManager.AttendanceRepository.Remove(attendance);
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    private async Task<Attendance> GetAttendanceFromIdAsync(Guid id)
    {
        var attendance = await _repositoryManager.AttendanceRepository.FindByIdAsync(id);
        if (attendance is null)
            throw new AttendanceNotFoundException(id.ToString());
        
        return attendance;
    }

    private async Task ValidateAttendanceCreateDto(AttendanceCreateDto dto)
    {
        try
        {
            ValidateAttendanceDetails(dto);
            await ValidateNotDuplicateAsync(dto);
            await ValidateDoctorIdAsync(dto.DoctorId);
            await ValidatePatientIdAsync(dto.PatientId);
        }
        catch (Exception e)
        {
            throw new AttendanceBadRequestException(e.Message);
        }
    }

    private async Task ValidateAttendanceDto(AttendanceDto dto)
    {
        try
        {
            ValidateAttendanceDetails(dto);
            await ValidateDoctorIdAsync(dto.DoctorId);
            await ValidatePatientIdAsync(dto.PatientId);
        }
        catch (Exception e)
        {
            throw new AttendanceBadRequestException(e.Message);
        }
    }

    private async Task ValidateNotDuplicateAsync(AttendanceCreateDto dto)
    {
        var exists = await _repositoryManager.AttendanceRepository.ExistsAsync(a =>
            a.DateTime == dto.DateTime && 
            a.DoctorId == dto.DoctorId && 
            a.PatientId == dto.PatientId);
        
        if (exists)
            throw new Exception("An Attendance with the same details already exists.");
    }
    
    private async Task ValidateDoctorIdAsync(Guid id)
    {
        var exists = await _repositoryManager.DoctorRepository.ExistsAsync(d => d.Id == id);
        if (!exists)
            throw new DoctorNotFoundException(id.ToString());
    }

    private async Task ValidatePatientIdAsync(Guid id)
    {
        var exists = await _repositoryManager.PatientRepository.ExistsAsync(p => p.Id == id);
        if (!exists)
            throw new PatientNotFoundException(id.ToString());
    }

    private static void ValidateAttendanceDetails(AttendanceCreateDto dto)
    {
        if (dto.DateTime < DateTime.Now)
            throw new ArgumentException("DateTime cannot be in the past");
        
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Diagnosis, nameof(dto.Diagnosis));
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Remarks, nameof(dto.Remarks));
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Therapy, nameof(dto.Therapy));
    }
}