using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Helpers;

namespace Persistence.Repositories;

internal sealed class DoctorRepository : IDoctorRepository
{
    private readonly RepositoryDbContext _context;

    public DoctorRepository(RepositoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Doctor>> Doctors(int pageNumber, int pageSize)
    {
        return await PagedListHelper.GetPagedList(
            _context.Doctors,
            d => d.Id,
            pageNumber,
            pageSize);
    }

    public async Task<int> GetTotalCount()
    {
        return await _context.Doctors.CountAsync();
    }

    public async Task<Doctor?> FindByIdAsync(Guid id)
    {
        return await _context.Doctors.SingleOrDefaultAsync(d => d.Id == id);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Doctor, bool>> predicate)
    {
        return await _context.Doctors.AnyAsync(predicate);
    }

    public void Add(Doctor doctor)
    {
        _context.Doctors.Add(doctor);
    }

    public void Remove(Doctor doctor)
    {
        _context.Doctors.Remove(doctor);
    }
}