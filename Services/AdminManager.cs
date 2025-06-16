using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using HospitalManagementSystem2.Utility;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem2.Services
{
    public class AdminManager
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAdminRepository _adminRepository;
        private readonly IAccountRepository _accountRepository;

        public AdminManager(ApplicationDbContext context,
                            UserManager<IdentityUser> userManager,
                            IAccountRepository accountRepository,
                            IAdminRepository adminRepository)
        {
            _context = context;
            _userManager = userManager;
            _accountRepository = accountRepository;
            _adminRepository = adminRepository;
        }

        public async Task CreateAsync(Account account)
        {
            await TransactionHelper.ExecuteInTransaction(_context, async () =>
            {
                // Add Role to User
                var roleResult = await _userManager.AddToRoleAsync(account.User, Constants.AuthRoles.Admin);
                ResultHelper.CheckIdentityResult(roleResult);

                // Create Admin
                var admin = new Admin 
                { 
                    AccountId = account.Id, 
                    Account = account
                };
                await _adminRepository.CreateAsync(admin);
            });
        }

        public async Task DeleteAsync(Admin admin)
        {
            await TransactionHelper.ExecuteInTransaction(_context, async () =>
            {
                // Get Account
                var account = await _accountRepository.FindByIdAsync(admin.AccountId)
                    ?? throw new NullReferenceException(NotificationHelper.MissingData(nameof(Account), nameof(Admin), admin.AccountId.ToString()));
                // Get User
                var user = await _userManager.FindByIdAsync(account.UserId)
                    ?? throw new NullReferenceException(NotificationHelper.MissingData("User", nameof(Admin), admin.AccountId.ToString()));
                // Remove User from Admin role
                var result = await _userManager.RemoveFromRoleAsync(user, Constants.AuthRoles.Admin);
                ResultHelper.CheckIdentityResult(result);
                // Delete Admin
                await _adminRepository.DeleteAsync(admin);
            });
        }

        public async Task<Admin?> FindByIdAsync(Guid id) => await _adminRepository.FindByIdAsync(id);
    }
}
