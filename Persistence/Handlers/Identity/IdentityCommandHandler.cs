using Commands.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Helpers;

namespace Persistence.Handlers.Identity
{
    public sealed class IdentityCommandHandler(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) :
        IRequestHandler<ChangePasswordCommand>,
        IRequestHandler<LoginCommand>,
        IRequestHandler<LogoutCommand>,
        IRequestHandler<ResetPasswordCommand>,
        IRequestHandler<SetLockOutCommand>
    {
        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await IdentityHelper.GetUserFromHmsIdAsync(userManager, request.HmsUserId);
            var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            IdentityHelper.ThrowOnIdentityFail(result);
        }

        public async Task Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await signInManager.PasswordSignInAsync(
                request.UserName,
                request.Password,
                request.IsPersistent,
                request.EnableLockoutOnFail);
            ThrowOnSignInFail(result);
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await signInManager.SignOutAsync();
        }

        public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await IdentityHelper.GetUserFromHmsIdAsync(userManager, request.HmsUserId);
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, request.NewPassword);
            IdentityHelper.ThrowOnIdentityFail(result);
        }

        public async Task Handle(SetLockOutCommand request, CancellationToken cancellationToken)
        {
            var user = await IdentityHelper.GetUserFromHmsIdAsync(userManager, request.HmsUserId);
            await userManager.SetLockoutEnabledAsync(user, request.Enabled);
            await userManager.SetLockoutEndDateAsync(user, request.Enabled ? DateTimeOffset.MaxValue : null);
        }

        private static void ThrowOnSignInFail(SignInResult result)
        {
            if (result.Succeeded is false)
                throw new Exception(result.ToString());
        }
    }
}
