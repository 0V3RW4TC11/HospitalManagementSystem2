using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal sealed class PatientRepository : IPatientRepository
{
    private readonly RepositoryDbContext _context;

    public PatientRepository(RepositoryDbContext context)
    {
        _context = context;
    }

    public async Task<Patient?> FindByIdAsync(Guid id)
    {
        return await _context.Patients.SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Patient, bool>> predicate)
    {
        return await _context.Patients.AnyAsync(predicate);
    }

    public void Add(Patient patient)
    {
        _context.Patients.Add(patient);
    }

    public void Remove(Patient patient)
    {
        _context.Patients.Remove(patient);
    }
}