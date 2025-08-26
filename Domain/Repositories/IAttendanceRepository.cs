using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories;

public interface IAttendanceRepository
{
    Task<(IEnumerable<Attendance> List, int TotalCount)> AttendancesByPatientIdAsync(Guid patientId, int pageNumber, int pageSize);
    
    Task<(IEnumerable<Attendance> List, int TotalCount)> AttendancesByDoctorIdAsync(Guid doctorId, int pageNumber, int pageSize);

    Task<Attendance?> FindByIdAsync(Guid id);

    Task<bool> ExistsAsync(Expression<Func<Attendance, bool>> predicate);
    
    void Add(Attendance attendance);
    
    void Remove(Attendance attendance);
}