using Services.Dtos.Attendance;

namespace Services.Abstractions;

public interface IAttendanceService
{
    Task CreateAsync(AttendanceCreateDto attendanceCreateDto);

    Task<(AttendanceDto[] List, int TotalCount)> AttendancesByPatientIdAsync(Guid patientId, int pageNumber, int pageSize);
    
    Task<(AttendanceDto[] List, int TotalCount)> AttendancesByDoctorIdAsync(Guid doctorId, int pageNumber, int pageSize);
    
    Task<AttendanceDto> GetByIdAsync(Guid id);
    
    Task UpdateAsync(AttendanceDto attendanceDto);
    
    Task DeleteAsync(Guid id);
}