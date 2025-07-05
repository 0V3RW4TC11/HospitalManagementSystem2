using HospitalManagementSystem2.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem2.Utility
{
    public class AccountHelper
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AccountHelper(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task CreateAsync(Account account, string role, string username, string password)
        {
            // Set User in Account
            account.User = new IdentityUser { UserName = username };

            // Create the User
            var result = await _userManager.CreateAsync(account.User, password);
            CheckIdentityResult(result);

            // Set User's role
            result = await _userManager.AddToRoleAsync(account.User, role);
            CheckIdentityResult(result);

            // Set UserId in Account
            account.UserId = account.User.Id;
        }

        public async Task UpdateAsync(Account account)
        {
            // Get the User
            IdentityUser user = account.User ??
                throw new Exception(NotificationHelper.MissingData(nameof(Account.User), nameof(Account), account.Id.ToString()));

            await _userManager.UpdateAsync(user);
        }

        public async Task DeleteAsync(Account account)
        {
            // Delete the IdentityUser
            var result = await _userManager.DeleteAsync(account.User);
            CheckIdentityResult(result);

            // Remove the UserId
            account.UserId = string.Empty;
        }

        public static void CheckIdentityResult(IdentityResult result)
        {
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
        }
    }
}
