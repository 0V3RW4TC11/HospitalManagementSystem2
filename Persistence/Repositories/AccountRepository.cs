using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal sealed class AccountRepository : IAccountRepository
{
    private readonly RepositoryDbContext _context;

    public AccountRepository(RepositoryDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> FindByUserIdAsync(Guid userId)
    {
        return await _context.Accounts.SingleOrDefaultAsync(a => a.UserId == userId);
    }

    public async Task<Account?> FindByIdentityIdAsync(string identityId)
    {
        return await _context.Accounts.SingleOrDefaultAsync(a => a.IdentityUserId == identityId);
    }

    public void Add(Account account)
    {
        _context.Accounts.Add(account);
    }

    public void Remove(Account account)
    {
        _context.Accounts.Remove(account);
    }
}