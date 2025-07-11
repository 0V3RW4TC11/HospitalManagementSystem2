using HospitalManagementSystem2.Models.Entities;

namespace HospitalManagementSystem2.Repositories;

public interface IAccountRepository
{
    IQueryable<Account> Accounts { get; }
    
    Task AddAsync(Account account);
    
    void Remove(Account account);
}