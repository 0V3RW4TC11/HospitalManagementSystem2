using DataTransfer.Attendance;

namespace Services.Abstractions;

public interface IAttendanceService
{
    Task CreateAsync(AttendanceCreateDto attendanceCreateDto);
    
    Task<IEnumerable<AttendanceDto>> GetAllByPatientIdAsync(Guid id);
    
    Task<IEnumerable<AttendanceDto>> GetAllByDoctorIdAsync(Guid id);
    
    Task UpdateAsync(AttendanceDto attendanceDto);
    
    Task DeleteAsync(Guid id);
}