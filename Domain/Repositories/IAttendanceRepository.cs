using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Repositories;

public interface IAttendanceRepository
{
    Task<Attendance?> FindByIdAsync(Guid id);
    
    Task<IEnumerable<(Guid Id, Guid UserId, DateTime DateTime)>> FindAttendanceInfoByPatientIdAsync(Guid id);
    
    Task<IEnumerable<(Guid Id, Guid UserId, DateTime DateTime)>> FindAttendanceInfoByDoctorIdAsync(Guid id);
    
    Task<bool> ExistsAsync(Expression<Func<Attendance, bool>> predicate);
    
    void Add(Attendance attendance);
    
    void Remove(Attendance attendance);
}