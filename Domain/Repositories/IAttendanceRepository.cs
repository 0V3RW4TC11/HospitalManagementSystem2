using System.Linq.Expressions;
using System.Runtime.InteropServices;
using DataTransfer.Attendance;
using Domain.Entities;

namespace Domain.Repositories;

public interface IAttendanceRepository
{
    Task<Attendance?> FindByIdAsync(Guid id);
    
    Task<IEnumerable<AttendanceShortDto>> FindShortAttendancesByPatientIdAsync(Guid id);
    
    Task<IEnumerable<AttendanceShortDto>> FindShortAttendancesByDoctorIdAsync(Guid id);
    
    Task<bool> ExistsAsync(Expression<Func<Attendance, bool>> predicate);
    
    void Add(Attendance attendance);
    
    void Remove(Attendance attendance);
}