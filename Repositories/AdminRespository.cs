using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Repositories
{
    public class AdminRespository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Admin> _admins;

        public AdminRespository(ApplicationDbContext context)
        {
            _context = context;
            _admins = context.Admins;
        }

        public IQueryable<Admin> Admins => _admins.AsNoTracking();

        private async Task<bool> IsExisting(Admin admin) => await _admins.AnyAsync(Staff.Matches(admin));

        public async Task CreateAsync(Admin admin)
        {
            if (await IsExisting(admin))
            {
                throw new Exception("A duplicate record exists");
            }

            await _admins.AddAsync(admin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Admin admin)
        {
            _admins.Remove(admin);
            await _context.SaveChangesAsync();
        }

        public async Task<Admin?> FindByIdAsync(Guid id)
        {
            return await _admins.AsNoTracking().FirstOrDefaultAsync(x => x.AccountId == id);
        }

        public async Task UpdateAsync(Admin admin)
        {
            var entry = _context.Entry(admin);
            entry.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
