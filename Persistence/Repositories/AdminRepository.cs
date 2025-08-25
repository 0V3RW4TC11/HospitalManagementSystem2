using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal sealed class AdminRepository : IAdminRepository
{
    private readonly RepositoryDbContext _context;

    public AdminRepository(RepositoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<(Guid Id, string FirstName, string? LastName, string Email)>> GetAdminListAsync()
    {
        return await _context.Admins
            .Select(a => new ValueTuple<Guid, string, string?, string>(a.Id, a.FirstName, a.LastName, a.Email))
            .ToArrayAsync();
    }

    public async Task<Admin?> FindByIdAsync(Guid id)
    {
        return await _context.Admins.SingleOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Admin, bool>> predicate)
    {
        return await _context.Admins.AnyAsync(predicate);
    }

    public void Add(Admin admin)
    {
        _context.Admins.Add(admin);
    }

    public void Remove(Admin admin)
    {
        _context.Admins.Remove(admin);
    }
}