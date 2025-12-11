using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Mapster;
using Services.Abstractions;
using Services.Dtos.Attendance;
using System.Linq.Expressions;

namespace Services;

internal sealed class AttendanceService : IAttendanceService
{
    private readonly IRepositoryManager _repositoryManager;
    private static readonly TimeSpan AttendanceDateTolerance = TimeSpan.FromHours(6);

    public AttendanceService(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }

    public async Task CreateAsync(AttendanceCreateDto attendanceCreateDto)
    {
        await ThrowOnInvalidAttendanceAsync(attendanceCreateDto);

        await ThrowOnDuplicateAsync(attendanceCreateDto);

        var attendance = attendanceCreateDto.Adapt<Attendance>();

        _repositoryManager.AttendanceRepository.Add(attendance);
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var attendance = await GetAttendanceFromIdAsync(id);

        _repositoryManager.AttendanceRepository.Remove(attendance);

        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task<(DoctorAttendanceSearchResultDto[] List, int TotalCount)> FindAttendancesByDoctorPagedAsync(Guid doctorId, int pageNumber, int pageSize)
    {
        Expression<Func<Attendance, bool>> filter = a => a.DoctorId == doctorId;
        Expression<Func<Attendance, DoctorAttendanceSearchResultDto>> selector = a => new() { AttendanceId = a.Id, PatientId = a.PatientId, DateTime = a.DateTime };

        return await _repositoryManager.AttendanceRepository.GetPagedListAsync(filter, selector, pageNumber, pageSize);
    }

    public async Task<(PatientAttendanceSearchResultDto[] List, int TotalCount)> FindAttendancesByPatientPagedAsync(Guid patientId, int pageNumber, int pageSize)
    {
        Expression<Func<Attendance, bool>> filter = a => a.PatientId == patientId;
        Expression<Func<Attendance, PatientAttendanceSearchResultDto>> selector = a => new() { AttendanceId = a.Id, DoctorId = a.DoctorId, DateTime = a.DateTime };

        return await _repositoryManager.AttendanceRepository.GetPagedListAsync(filter, selector, pageNumber, pageSize);
    }

    public async Task<AttendanceDto> GetByIdAsync(Guid id)
    {
        var attendance = await GetAttendanceFromIdAsync(id);
        
        return attendance.Adapt<AttendanceDto>();
    }

    public async Task UpdateAsync(AttendanceDto attendanceDto)
    {
        await ThrowOnInvalidAttendanceAsync(attendanceDto);

        var attendance = await GetAttendanceFromIdAsync(attendanceDto.Id);

        attendance.Diagnosis = attendanceDto.Diagnosis;
        attendance.Remarks = attendanceDto.Remarks;
        attendance.Therapy = attendanceDto.Therapy;
        attendance.DateTime = attendanceDto.DateTime;
        attendance.PatientId = attendanceDto.PatientId;
        attendance.DoctorId = attendanceDto.DoctorId;
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    private async Task<Attendance> GetAttendanceFromIdAsync(Guid id)
    {
        var attendance = await _repositoryManager.AttendanceRepository.FindByIdAsync(id) 
            ?? throw new AttendanceNotFoundException();
        
        return attendance;
    }

    private async Task ThrowOnDuplicateAsync(AttendanceBaseDto dto)
    {
        var exists = await _repositoryManager.AttendanceRepository.ExistsAsync(a =>
            IsSameDate(a.DateTime, dto.DateTime) &&
            a.DoctorId == dto.DoctorId &&
            a.PatientId == dto.PatientId);

        if (exists)
            throw new AttendanceDuplicationException("An Attendance with similar details already exists.");
    }

    private async Task ThrowOnInvalidAttendanceAsync(AttendanceBaseDto dto)
    {
        await ThrowOnInvalidDoctorIdAsync(dto.DoctorId);
        await ThrowOnInvalidPatientIdAsync(dto.PatientId);
        ThrowOnInvalidDate(dto.DateTime);
        ThrowOnInvalidText(dto);
    }

    private async Task ThrowOnInvalidDoctorIdAsync(Guid id)
    {
        var exists = await _repositoryManager.DoctorRepository.ExistsAsync(d => d.Id == id);
        if (!exists)
            throw new DoctorNotFoundException();
    }

    private async Task ThrowOnInvalidPatientIdAsync(Guid id)
    {
        var exists = await _repositoryManager.PatientRepository.ExistsAsync(p => p.Id == id);
        if (!exists)
            throw new PatientNotFoundException();
    }

    private static void ThrowOnInvalidDate(DateTime dateTime)
    {
        if (dateTime > DateTime.UtcNow)
            throw new AttendanceInvalidException("Attendance date cannot be in the future.");
    }

    private static void ThrowOnInvalidText(AttendanceBaseDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Diagnosis))
            throw new AttendanceInvalidException("Diagnosis cannot be empty.");
        if (string.IsNullOrWhiteSpace(dto.Remarks))
            throw new AttendanceInvalidException("Remarks cannot be empty.");
        if (string.IsNullOrWhiteSpace(dto.Therapy))
            throw new AttendanceInvalidException("Therapy cannot be empty.");
    }

    private static bool IsSameDate(DateTime a, DateTime b)
    {
        return Math.Abs((a - b).TotalHours) <= AttendanceDateTolerance.TotalHours;
    }
}