using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.AppConstants;
using System.Security.Claims;

namespace Persistence.Helpers
{
    internal static class IdentityHelper
    {
        public static async Task<IdentityUser> GetUserFromHmsIdAsync(UserManager<IdentityUser> userManager, Guid hmsUserId)
        {
            var claim = CreateClaim(hmsUserId);
            return (await userManager.GetUsersForClaimAsync(claim)).SingleOrDefault()
                ?? throw new Exception("Identity not found for HMS User with Id " + hmsUserId);
        }

        public static void ThrowOnIdentityFail(IdentityResult result)
        {
            if (result.Succeeded is false)
                throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
        }

        public static Claim CreateClaim(Guid hmsUserId)
        {
            return new Claim(ClaimConstants.HmsUserId, hmsUserId.ToString());
        }
    }
}
