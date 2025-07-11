using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly DbSet<Account> _accounts;

    public AccountRepository(IDbContext context)
    {
        _accounts = context.Accounts;
    }
    
    public IQueryable<Account> Accounts => _accounts.AsNoTracking();
    
    public async Task AddAsync(Account account) => await _accounts.AddAsync(account);

    public void Remove(Account account) => _accounts.Remove(account);
}