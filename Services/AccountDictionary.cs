using Domain.Exceptions;
using Domain.Repositories;
using Services.Abstractions;

namespace Services
{
    internal class AccountDictionary : IAccountDictionary
    {
        private readonly IAccountRepository _repository;

        public AccountDictionary(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> GetIdentityIdByUserId(Guid userId)
        {
            var account = await _repository.FindByUserIdAsync(userId)
                ?? throw new AccountNotFoundException("Account not found for User Id: " + userId);

            return account.IdentityUserId;
        }

        public async Task<Guid> GetUserIdByIdentityId(string identityId)
        {
            var account = await _repository.FindByIdentityIdAsync(identityId)
                ?? throw new AccountNotFoundException("Account not found for Identity Id: " + identityId);

            return account.UserId;
        }
    }
}
