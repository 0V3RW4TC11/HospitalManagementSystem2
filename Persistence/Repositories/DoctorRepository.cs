using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal sealed class DoctorRepository : IDoctorRepository
{
    private readonly RepositoryDbContext _context;

    public DoctorRepository(RepositoryDbContext context)
    {
        _context = context;
    }

    public async Task<Doctor?> FindByIdAsync(Guid id)
    {
        return await _context.Doctors.SingleOrDefaultAsync(d => d.Id == id);
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