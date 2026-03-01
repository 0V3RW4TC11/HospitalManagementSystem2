using Commands.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Helpers;

namespace Persistence.Handlers.Identity
{
    public sealed class IdentityCommandHandler :
        IRequestHandler<ChangePasswordCommand>,
        IRequestHandler<LoginCommand>,
        IRequestHandler<LogoutCommand>,
        IRequestHandler<ResetPasswordCommand>,
        IRequestHandler<SetLockOutCommand>
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityCommandHandler(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await IdentityHelper.GetUserFromHmsUserIdAsync(_userManager, request.HmsUserId);
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            IdentityHelper.ThrowOnIdentityFail(result);
        }

        public async Task Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await _signInManager.PasswordSignInAsync(
                request.UserName,
                request.Password,
                request.IsPersistent,
                request.EnableLockoutOnFail);
            ThrowOnSignInFail(result);
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();
        }

        public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await IdentityHelper.GetUserFromHmsUserIdAsync(_userManager, request.HmsUserId);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
            IdentityHelper.ThrowOnIdentityFail(result);
        }

        public async Task Handle(SetLockOutCommand request, CancellationToken cancellationToken)
        {
            var user = await IdentityHelper.GetUserFromHmsUserIdAsync(_userManager, request.HmsUserId);
            user.LockoutEnabled = request.Enabled;
            user.LockoutEnd = request.Enabled ? DateTimeOffset.MaxValue : null;
        }

        private static void ThrowOnSignInFail(SignInResult result)
        {
            if (result.Succeeded is false)
                throw new Exception(result.ToString());
        }
    }
}
