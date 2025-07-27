using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly RepositoryDbContext _context;

    public AdminRepository(RepositoryDbContext context)
    {
        _context = context;
    }

    public async Task<Admin?> FindByIdAsync(Guid id)
    {
        return await _context.Admins.SingleOrDefaultAsync(a => a.Id == id);
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