using Domain.Exceptions;
using Domain.Repositories;
using Services.Abstractions;

namespace Services
{
    internal sealed class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<string> GetIdentityIdFromUserId(Guid userId)
        {
            var account = await _accountRepository.FindByUserIdAsync(userId)
                ?? throw new AccountNotFoundException("Account not found for User Id: " + userId);

            return account.IdentityUserId;
        }

        public async Task<Guid> GetUserIdFromIdentityId(string identityId)
        {
            var account = await _accountRepository.FindByIdentityIdAsync(identityId)
                ?? throw new AccountNotFoundException("Account not found for Identity Id: " + identityId);

            return account.UserId;
        }
    }
}
