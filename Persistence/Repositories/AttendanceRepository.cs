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

    public async Task<(IEnumerable<Attendance> List, int TotalCount)> AttendancesByPatientIdAsync(Guid patientId, int pageNumber, int pageSize)
    {
        return await Attendances(
            a => a.PatientId == patientId,
            pageNumber,
            pageSize);
    }

    public async Task<(IEnumerable<Attendance> List, int TotalCount)> AttendancesByDoctorIdAsync(Guid doctorId, int pageNumber, int pageSize)
    {
        return await Attendances(
            a => a.DoctorId == doctorId,
            pageNumber,
            pageSize);
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

    private async Task<(IEnumerable<Attendance> List, int TotalCount)> Attendances(
        Expression<Func<Attendance, bool>> predicate,
        int pageNumber,
        int pageSize)
    {
        // Ensure page number is at least 1
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 0 ? 10 : pageSize;

        // Fetch only the rows for the current page
        var attendances = await _context.Attendances
            .Where(predicate)
            .OrderBy(a => a.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync();

        var totalCount = await _context.Attendances
            .Where(predicate)
            .CountAsync();

        return (List: attendances, TotalCount: totalCount);
    }
}