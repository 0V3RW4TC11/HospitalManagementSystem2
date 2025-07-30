using System.Linq.Expressions;
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

    public async Task<IEnumerable<(Guid Id, Guid UserId, DateTime DateTime)>> FindAttendanceInfoByPatientIdAsync(Guid id)
    {
        return await _context.Attendances
            .Where(a => a.PatientId == id)
            .Select(a => new ValueTuple<Guid, Guid, DateTime>(a.Id, a.DoctorId, a.DateTime))
            .ToArrayAsync();
    }

    public async Task<IEnumerable<(Guid Id, Guid UserId, DateTime DateTime)>> FindAttendanceInfoByDoctorIdAsync(Guid id)
    {
        return await _context.Attendances
            .Where(a => a.DoctorId == id)
            .Select(a => new ValueTuple<Guid, Guid, DateTime>(a.Id, a.PatientId, a.DateTime))
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