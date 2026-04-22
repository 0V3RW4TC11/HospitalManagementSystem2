using Abstractions;
using Microsoft.AspNetCore.Identity;
using Persistence.Helpers;

namespace Persistence
{
    internal sealed class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task CreateIdentityAsync(Guid hmsUserId, string userName, string password, string role, CancellationToken cancellationToken)
        {
            var user = new IdentityUser { UserName = userName };
            var userResult = await _userManager.CreateAsync(user, password);
            IdentityHelper.ThrowOnIdentityFail(userResult);

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            IdentityHelper.ThrowOnIdentityFail(roleResult);

            var claimResult = await _userManager.AddClaimAsync(user, IdentityHelper.CreateClaim(hmsUserId));
            IdentityHelper.ThrowOnIdentityFail(claimResult);
        }

        public async Task DeleteIdentityAsync(Guid hmsUserId, CancellationToken cancellationToken)
        {
            var claim = IdentityHelper.CreateClaim(hmsUserId);
            var user = await IdentityHelper.GetUserFromHmsIdAsync(_userManager, hmsUserId);

            var claimResult = await _userManager.RemoveClaimAsync(user, claim);
            IdentityHelper.ThrowOnIdentityFail(claimResult);

            var userResult = await _userManager.DeleteAsync(user);
            IdentityHelper.ThrowOnIdentityFail(userResult);
        }
    }
}
