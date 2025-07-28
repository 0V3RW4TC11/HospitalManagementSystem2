using Domain.Entities;

namespace Domain.Repositories;

public interface IAttendanceRepository
{
    Task<IEnumerable<Attendance>> GetAllByPatientIdAsync(Guid id);
    
    Task<IEnumerable<Attendance>> GetAllByDoctorIdAsync(Guid id);
    
    Task<Attendance?> FindByIdAsync(Guid id);
    
    void Add(Attendance attendance);
    
    void Remove(Attendance attendance);
}