using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.AppConstants;
using Queries.Identity;

namespace Persistence.Handlers.Identity
{
    internal sealed class IdentityQueryHandler : IRequestHandler<GetHmsUserIdQuery, Guid>
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityQueryHandler(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<Guid> Handle(GetHmsUserIdQuery request, CancellationToken cancellationToken)
        {
            var user = _signInManager.Context.User;
            
            if (!_signInManager.IsSignedIn(user))
            {
                throw new Exception("No user is currently signed in.");
            }
            
            var identityId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? throw new Exception("Signed in user has no Asp Identity claim");

            var identityUser = await _userManager.FindByIdAsync(identityId)
                ?? throw new Exception("No Asp Identity found for ID " + identityId);

            var claims = await _userManager.GetClaimsAsync(identityUser);

            var hmsUserIdClaim = claims.FirstOrDefault(c => c.Type == ClaimConstants.HmsUserId)
                ?? throw new Exception("User has no associated HMS User Id");

            return Guid.Parse(hmsUserIdClaim.Value);
        }
    }
}
