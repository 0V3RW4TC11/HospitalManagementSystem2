using Abstractions;
using Microsoft.AspNetCore.Identity;
using Persistence.Constants;
using System.Security.Claims;

namespace Persistence
{
    internal sealed class IdentityProvider : IIdentityProvider
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityProvider(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task CreateIdentityAsync(Guid hmsUserId, string userName, string password, string role, CancellationToken cancellationToken)
        {
            var user = new IdentityUser { UserName = userName };
            var userResult = await _userManager.CreateAsync(user, password);
            ResultThrowOnFail(userResult);

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            ResultThrowOnFail(roleResult);

            var claimResult = await _userManager.AddClaimAsync(user, CreateClaim(hmsUserId));
            ResultThrowOnFail(claimResult);
        }

        public async Task DeleteIdentityAsync(Guid hmsUserId, CancellationToken cancellationToken)
        {
            var claim = CreateClaim(hmsUserId);
            var user = (await _userManager.GetUsersForClaimAsync(claim)).SingleOrDefault()
                ?? throw new Exception("Identity not found for HMS User with Id " + hmsUserId);

            var claimResult = await _userManager.RemoveClaimAsync(user, claim);
            ResultThrowOnFail(claimResult);

            var userResult = await _userManager.DeleteAsync(user);
            ResultThrowOnFail(userResult);
        }

        private static Claim CreateClaim(Guid hmsUserId)
        {
            return new Claim(ClaimConstants.ClaimType, hmsUserId.ToString());
        }

        private static void ResultThrowOnFail(IdentityResult result)
        {
            if (result.Succeeded is false)
                throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
        }
    }
}
