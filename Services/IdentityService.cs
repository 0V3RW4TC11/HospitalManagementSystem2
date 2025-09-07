using Domain.Repositories;
using Services.Abstractions;

namespace Services
{
    internal class IdentityService : IIdentityService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly AccountService _accountService;

        public IdentityService(IRepositoryManager repositoryManager, AccountService accountService)
        {
            _repositoryManager = repositoryManager;
            _accountService = accountService;
        }

        public async Task LoginAsync(
            string username, 
            string password, 
            bool isPersistent, 
            bool enableLockoutOnFail)
        {
            await _repositoryManager.IdentityProvider.LoginAsync(
                username,
                password, 
                isPersistent, 
                enableLockoutOnFail);
        }

        public async Task LogoutAsync()
        {
            await _repositoryManager.IdentityProvider.LogoutAsync();
        }

        public async Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            await _repositoryManager.IdentityProvider.ChangePasswordAsync(
                await _accountService.GetIdentityIdFromUserId(userId),
                oldPassword,
                newPassword);
        }

        public async Task ResetPasswordAsync(Guid userId, string newPassword)
        {
            await _repositoryManager.IdentityProvider.ResetPasswordAsync(
                await _accountService.GetIdentityIdFromUserId(userId),
                newPassword);
        }

        public async Task SetLockoutAsync(Guid userId, bool enabled)
        {
            await _repositoryManager.IdentityProvider.SetLockoutAsync(
                await _accountService.GetIdentityIdFromUserId(userId),
                enabled);
        }

        public async Task<bool> IsLockedOut(Guid userId)
        {
            return await _repositoryManager.IdentityProvider.IsLockedOut(
                await _accountService.GetIdentityIdFromUserId(userId));
        }

        public async Task<string> GetUserNameAsync(Guid userId)
        {
            return await _repositoryManager.IdentityProvider.GetUserNameAsync(
                await _accountService.GetIdentityIdFromUserId(userId));
        }

        public async Task<Guid> GetLoggedInUserId()
        {
            string identityId = _repositoryManager.IdentityProvider.GetLoggedInUserIdentityId();
            return await _accountService.GetUserIdFromIdentityId(identityId);
        }
    }
}
