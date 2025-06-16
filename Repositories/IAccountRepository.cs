using HospitalManagementSystem2.Models.Entities;

namespace HospitalManagementSystem2.Repositories
{
    public interface IAccountRepository
    {
        Task CreateAsync(Account account);

        Task<Account?> FindByIdAsync(Guid id);

        Task UpdateAsync(Account account);

        Task DeleteAsync(Account account);

        IQueryable<Account> Accounts { get; }
    }
}
