using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal sealed class SpecializationRepository : ISpecializationRepository
{
    private readonly RepositoryDbContext _context;

    public SpecializationRepository(RepositoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Specialization>> GetAllAsync()
    {
        return await _context.Specializations.ToArrayAsync();
    }

    public async Task<IEnumerable<Specialization>> GetFromIdsAsync(IEnumerable<Guid> ids)
    {
        return await _context.Specializations.Where(s => ids.Contains(s.Id)).ToArrayAsync();
    }

    public async Task<Specialization?> FindByIdAsync(Guid id)
    {
        return await _context.Specializations.SingleOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Specializations.AnyAsync(s => s.Id == id);
    }

    public async Task<bool> ExistsAsync(string name)
    {
        return await _context.Specializations.AnyAsync(s => s.Name == name);
    }

    public void Add(Specialization specialization)
    {
        _context.Specializations.Add(specialization);
    }

    public void Remove(Specialization specialization)
    {
        _context.Specializations.Remove(specialization);   
    }
}