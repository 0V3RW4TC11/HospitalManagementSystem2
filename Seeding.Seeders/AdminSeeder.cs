using Bogus;
using Entities;
using Seeding.Seeders.Helpers;

namespace Seeding.Seeders
{
    internal class AdminSeeder : BaseAccountSeeder<Admin>
    {
        public AdminSeeder(IServiceProvider services, string roleId, string password) : base(
            services,
            roleId,
            password,
            x => x.Email!)
        { }

        protected override Faker<Admin> CreateFaker() => new Faker<Admin>()
            .RuleFor(x => x.Id, f => Guid.NewGuid())
            .RuleFor(x => x.Gender, FakerHelper.PickRandomGender)
            .RuleFor(x => x.Title, (f, x) => f.Name.Prefix(FakerHelper.GetGender(x.Gender!)))
            .RuleFor(x => x.FirstName, (f, x) => f.Name.FirstName(FakerHelper.GetGender(x.Gender!)))
            .RuleFor(x => x.LastName, (f, x) => f.Name.LastName(FakerHelper.GetGender(x.Gender!)))
            .RuleFor(x => x.Address, f => f.Address.FullAddress())
            .RuleFor(x => x.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.DateOfBirth, f => DateOnly.FromDateTime(f.Person.DateOfBirth))
            .RuleFor(x => x.Email, (f, x) => FakerHelper.GetStaffEmail(x.FirstName, x.LastName!));
    }
}