using Services.Abstractions;
using System.Security.Claims;

namespace Presentation.Helpers
{
    internal static class IdentityHelper
    {
        // TODO: Move into IdentityService later
        public static async Task<Guid> GetUserIdFromSignedInUser(ClaimsPrincipal user, IAccountService accountService)
        {
            if (user.Identity == null || !user.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated.");
            
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found in claims.");

            return await accountService.GetUserIdFromIdentityId(userId);
        }
    }
}
