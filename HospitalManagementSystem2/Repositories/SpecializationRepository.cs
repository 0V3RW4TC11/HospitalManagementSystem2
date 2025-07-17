using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Repositories;

public class SpecializationRepository : ISpecializationRepository
{
    private readonly DbSet<Specialization> _specializations;

    public SpecializationRepository(IDbContext context)
    {
        _specializations = context.Specializations;
    }
    
    public IQueryable<Specialization> Specializations => _specializations.AsNoTracking();
    
    public async Task AddAsync(Specialization specialization)
    {
        await _specializations.AddAsync(specialization);
    }

    public async Task UpdateAsync(Specialization specialization)
    {
        var entry = await _specializations.FirstAsync(s => s.Id == specialization.Id);
        entry.Name = specialization.Name;
        _specializations.Update(entry);
    }

    public async Task RemoveAsync(Specialization specialization)
    {
        var entry = await _specializations.FirstAsync(s => s.Id == specialization.Id);
        _specializations.Remove(entry);
    }
}