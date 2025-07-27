using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class DoctorSpecializationRepository : IDoctorSpecializationRepository
{
    private readonly RepositoryDbContext _context;

    public DoctorSpecializationRepository(RepositoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Guid>> GetSpecIdsByDoctorIdAsync(Guid doctorId)
    {
        return await _context.DoctorSpecializations
            .Where(ds => ds.DoctorId == doctorId)
            .Select(ds => ds.SpecializationId)
            .ToArrayAsync();
    }

    public async Task UpdateAsync(Guid doctorId, IEnumerable<Guid> specializationIds)
    {
        var currentSpecs = (await GetSpecIdsByDoctorIdAsync(doctorId)).ToArray();
        
        var newSpecs = specializationIds.ToArray();
        
        var intersect = newSpecs.Intersect(currentSpecs).ToArray();
        
        var toAdd = newSpecs
            .Except(intersect)
            .Select(id => new DoctorSpecialization { DoctorId = doctorId, SpecializationId = id });
        var toRemove = currentSpecs
            .Except(intersect)
            .Select(id => new DoctorSpecialization { DoctorId = doctorId, SpecializationId = id });
        
        _context.DoctorSpecializations.AddRange(toAdd);
        _context.DoctorSpecializations.RemoveRange(toRemove);
    }
}