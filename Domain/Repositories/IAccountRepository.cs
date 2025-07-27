using Domain.Entities;

namespace Domain.Repositories;

public interface IAccountRepository
{
    Task<Account?> FindByUserIdAsync(Guid userId);
    
    Task<Account?> FindByIdentityIdAsync(string identityId);

    void Add(Account account);
    
    void Remove(Account account);
}