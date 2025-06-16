using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Account> _accounts;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
            _accounts = context.Accounts;
        }

        public IQueryable<Account> Accounts => _accounts.AsNoTracking();

        public async Task CreateAsync(Account account)
        {
            await _accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Account account)
        {
            _accounts.Remove(account);
            await _context.SaveChangesAsync();
        }

        public async Task<Account?> FindByIdAsync(Guid id)
        {
            return await _accounts.AsNoTracking().FirstOrDefaultAsync(x => x.PersonId == id);
        }

        public async Task UpdateAsync(Account account)
        {
            var entry = _context.Entry(account);
            entry.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
