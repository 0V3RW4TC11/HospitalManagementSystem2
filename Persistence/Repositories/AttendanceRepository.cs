using System.Linq.Expressions;
using DataTransfer.Attendance;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal sealed class AttendanceRepository : IAttendanceRepository
{
    private readonly RepositoryDbContext _context;

    public AttendanceRepository(RepositoryDbContext context)
    {
        _context = context;
    }

    public async Task<Attendance?> FindByIdAsync(Guid id)
    {
        return await _context.Attendances.SingleOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<AttendanceShortDto>> FindShortAttendancesByPatientIdAsync(Guid id)
    {
        return await _context.Attendances
            .Where(a => a.PatientId == id)
            .Select(a => new AttendanceShortDto{ Id = a.Id, UserId = a.DoctorId, DateTime = a.DateTime })
            .ToArrayAsync();
    }

    public async Task<IEnumerable<AttendanceShortDto>> FindShortAttendancesByDoctorIdAsync(Guid id)
    {
        return await _context.Attendances
            .Where(a => a.DoctorId == id)
            .Select(a => new AttendanceShortDto{ Id = a.Id, UserId = a.PatientId, DateTime = a.DateTime })
            .ToArrayAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<Attendance, bool>> predicate)
    {
        return await _context.Attendances.AnyAsync(predicate);
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