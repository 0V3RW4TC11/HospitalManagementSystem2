using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal sealed class DoctorSpecializationRepository : IDoctorSpecializationRepository
{
    private readonly RepositoryDbContext _context;

    public DoctorSpecializationRepository(RepositoryDbContext context)
    {
        _context = context;
    }

    public void AddRange(IEnumerable<DoctorSpecialization> doctorSpecializations)
    {
        _context.DoctorSpecializations.AddRange(doctorSpecializations);
    }

    public async Task<IEnumerable<Guid>> GetSpecIdsByDoctorIdAsync(Guid doctorId)
    {
        return await _context.DoctorSpecializations
            .Where(ds => ds.DoctorId == doctorId)
            .Select(ds => ds.SpecializationId)
            .ToArrayAsync();
    }

    public void RemoveRange(IEnumerable<DoctorSpecialization> doctorSpecializations)
    {
        _context.DoctorSpecializations.RemoveRange(doctorSpecializations);
    }
}