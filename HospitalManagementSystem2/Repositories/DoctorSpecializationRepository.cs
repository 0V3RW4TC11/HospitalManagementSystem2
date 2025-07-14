using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Repositories;

public class DoctorSpecializationRepository : IDoctorSpecializationRepository
{
    private readonly DbSet<DoctorSpecialization> _doctorSpecs;

    public DoctorSpecializationRepository(IDbContext context)
    {
        _doctorSpecs = context.DoctorSpecializations;
    }
    
    public IQueryable<DoctorSpecialization> DoctorSpecializations => _doctorSpecs.AsNoTracking();
    
    public async Task AddRangeAsync(IEnumerable<DoctorSpecialization> doctorSpecializations)
    {
        await _doctorSpecs.AddRangeAsync(doctorSpecializations);
    }

    public async Task RemoveRangeAsync(IEnumerable<DoctorSpecialization> doctorSpecializations)
    {
        var toRemove = await _doctorSpecs
            .Where(x => doctorSpecializations.Contains(x))
            .ToListAsync();
        
        _doctorSpecs.RemoveRange(toRemove);
    }
}