using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Services;

namespace HospitalManagementSystem2.Tests
{
    public class AdminTests
    {
        private readonly AccountManager _accountManager;
        private readonly AdminManager _adminManager;

        private Account _account = new()
        {
            Person = new Person
            {
                Title = "TestTitle",
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                Gender = "Test",
                Address = "1 Test Test, Test 0000",
                Phone = "0-0-0000-0000",
                Email = "test@example.com",
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now)
            }
        };

        private readonly string _adminPassword = "Password123!";

        public AdminTests(AccountManager accountManager,
                          AdminManager adminManager)
        {
            _accountManager = accountManager;
            _adminManager = adminManager;
        }

        public async Task CreateTest()
        {
            await Create();
            _ = await _adminManager.FindByIdAsync(_account.Id)
                ?? throw new Exception("Admin not found");
        }

        public async Task DeleteTest()
        {
            var admin = await _adminManager.FindByIdAsync(_account.Id)
                ?? throw new Exception("Admin not found");
            await _adminManager.DeleteAsync(admin);

            await _accountManager.DeleteAsync(_account);
            admin = await _adminManager.FindByIdAsync(_account.Id);
            if (admin != null)
                throw new Exception("Admin found");
        }

        public async Task DuplicationTest()
        {
            // create
            // create
            // confirm
            throw new NotImplementedException();
        }

        public async Task UpdateTest()
        {
            // create
            // update
            // confirm
            throw new NotImplementedException();
        }

        private async Task Create()
        {
            await _accountManager.CreateAsync(_account, _account.Person.Email
                ?? throw new Exception("Email not set"), _adminPassword);

            await _adminManager.CreateAsync(_account);
        }
    }
}
