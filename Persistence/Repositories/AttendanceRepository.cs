using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly RepositoryDbContext _context;

    public AttendanceRepository(RepositoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Attendance>> GetAllByPatientIdAsync(Guid id)
    {
        return await _context.Attendances.Where(a => a.PatientId == id).ToArrayAsync();
    }

    public async Task<IEnumerable<Attendance>> GetAllByDoctorIdAsync(Guid id)
    {
        return await _context.Attendances.Where(a => a.DoctorId == id).ToArrayAsync();   
    }

    public async Task<Attendance?> FindByIdAsync(Guid id)
    {
        return await _context.Attendances.SingleOrDefaultAsync(a => a.Id == id);
    }

    public void Add(Attendance attendance)
    {
        _context.Attendances.Add(attendance);
    }

    public void Remove(Attendance attendance)
    {
        _context.Attendances.Remove(attendance);
    }
}