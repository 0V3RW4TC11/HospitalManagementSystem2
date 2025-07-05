using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Utility;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Services
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Admin> _admins;
        private readonly AccountHelper _accountHelper;
        private readonly IStaffEmailGenerator _staffEmailGenerator;

        public AdminRepository(ApplicationDbContext context, 
                                AccountHelper accountHelper,
                                IStaffEmailGenerator staffEmailGenerator)
        {
            _context = context;
            _admins = context.Admins;
            _accountHelper = accountHelper;
            _staffEmailGenerator = staffEmailGenerator;
        }

        public IQueryable<Admin> Admins => _admins.AsNoTracking();

        public async Task CreateAsync(Admin admin, string password)
        {
            if (await IsExisting(admin))
            {
                throw new Exception("A duplicate record exists");
            }

            await TransactionHelper.ExecuteInTransaction(_context, async () =>
            {
                // Create Username
                var username = await _staffEmailGenerator.GenerateEmailAsync(admin, Constants.StaffEmailDomain);

                // Create Account
                await _accountHelper.CreateAsync(admin, Constants.AuthRoles.Admin, username, password);
                
                // Create Admin
                await _admins.AddAsync(admin);
                await _context.SaveChangesAsync();
            });
        }

        public async Task DeleteAsync(Admin admin)
        {
            await TransactionHelper.ExecuteInTransaction(_context, async () =>
            {
                // Delete Admin
                _admins.Remove(admin);
                await _context.SaveChangesAsync();

                // Delete Admin Account
                await _accountHelper.DeleteAsync(admin);
            });
        }

        public async Task<Admin?> FindByIdAsync(Guid id)
        {
            return await Admins.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(Admin admin)
        {
            await TransactionHelper.ExecuteInTransaction(_context, async () =>
            {
                // Update the Admin
                var entry = _context.Entry(admin);
                entry.State = EntityState.Modified;

                // Save changes
                await _context.SaveChangesAsync();

                // Update Admin Account
                await _accountHelper.UpdateAsync(admin);
            });
        }

        private async Task<bool> IsExisting(Admin admin) => await _admins.AnyAsync(Staff.Matches(admin));
    }
}
