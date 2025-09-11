using Domain.Repositories;
using Services.Abstractions;

namespace Services
{
    internal class IdentityService : IIdentityService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly AccountService _accountService;

        public IdentityService(IIdentityProvider identityProvider, AccountService accountService)
        {
            _identityProvider = identityProvider;
            _accountService = accountService;
        }

        public async Task LoginAsync(
            string username, 
            string password, 
            bool isPersistent, 
            bool enableLockoutOnFail)
        {
            await _identityProvider.LoginAsync(
                username,
                password, 
                isPersistent, 
                enableLockoutOnFail);
        }

        public async Task LogoutAsync()
        {
            await _identityProvider.LogoutAsync();
        }

        public async Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            await _identityProvider.ChangePasswordAsync(
                await _accountService.GetIdentityIdFromUserId(userId),
                oldPassword,
                newPassword);
        }

        public async Task ResetPasswordAsync(Guid userId, string newPassword)
        {
            await _identityProvider.ResetPasswordAsync(
                await _accountService.GetIdentityIdFromUserId(userId),
                newPassword);
        }

        public async Task SetLockoutAsync(Guid userId, bool enabled)
        {
            await _identityProvider.SetLockoutAsync(
                await _accountService.GetIdentityIdFromUserId(userId),
                enabled);
        }

        public async Task<bool> IsLockedOutAsync(Guid userId)
        {
            return await _identityProvider.IsLockedOut(
                await _accountService.GetIdentityIdFromUserId(userId));
        }

        public async Task<string> GetUserNameAsync(Guid userId)
        {
            return await _identityProvider.GetUserNameAsync(
                await _accountService.GetIdentityIdFromUserId(userId));
        }

        public async Task<Guid> GetLoggedInUserId()
        {
            string identityId = _identityProvider.GetLoggedInUserIdentityId();
            return await _accountService.GetUserIdFromIdentityId(identityId);
        }
    }
}
