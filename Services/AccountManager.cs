using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using HospitalManagementSystem2.Utility;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem2.Services
{
    public class AccountManager
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAccountRepository _accountRepository;
        private readonly IPersonRepository _personRepository;

        public AccountManager(ApplicationDbContext context,
                              UserManager<IdentityUser> userManager,
                              IAccountRepository accountRepository,
                              IPersonRepository personRepository)
        {
            _context = context;
            _userManager = userManager;
            _accountRepository = accountRepository;
            _personRepository = personRepository;
        }

        public async Task CreateAsync(Account account, string username, string password)
        {
            await TransactionHelper.ExecuteInTransaction(_context, async () =>
            {
                // Create Person
                await _personRepository.CreateAsync(account.Person);
                // Set PersonId
                account.PersonId = account.Person.Id;

                // Set User
                account.User = new IdentityUser { UserName = username };
                // Create User
                var result = await _userManager.CreateAsync(account.User, password);
                ResultHelper.CheckIdentityResult(result);
                // Set UserId
                account.UserId = account.User.Id;

                // Create Account
                await _accountRepository.CreateAsync(account);
            });
        }

        public async Task DeleteAsync(Account account)
        {
            await TransactionHelper.ExecuteInTransaction(_context, async () =>
            {
                // Extract User
                var user = account.User;
                // Extract Person
                var person = account.Person;
                // Delete Account
                await _accountRepository.DeleteAsync(account);
                // Delete User
                var result = await _userManager.DeleteAsync(user);
                ResultHelper.CheckIdentityResult(result);
                // Delete Person
                await _personRepository.DeleteAsync(person);
            });
        }

        public async Task<Account?> FindByIdAsync(Guid id)
        {
            // Get Account
            var account = await _accountRepository.FindByIdAsync(id);
            if (account != null)
            {
                // Set User
                account.User = await _userManager.FindByIdAsync(account.UserId)
                    ?? throw new NullReferenceException(NotificationHelper.MissingData("User", nameof(Account), id.ToString()));

                // Set Person
                account.Person = await _personRepository.FindByIdAsync(account.PersonId)
                    ?? throw new NullReferenceException(NotificationHelper.MissingData(nameof(Person), nameof(Account), id.ToString()));

                return account;
            }

            return null;
        }
    }
}
