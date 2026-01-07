using Bogus;
using Domain.Entities;
using Seeders.Helpers;
using static Bogus.DataSets.Name;
using static Seeders.Helpers.FakerHelper;

namespace Seeders
{
    internal class AdminSeeder : BaseAccountSeeder<Admin>
    {
        private readonly UniqueEmailsHelper _emailHelper = new(Constants.DomainNames.Organization);

        public AdminSeeder(IServiceProvider services, string roleId, string password) : base(
            services,
            roleId,
            password,
            x => x.Email!)
        { }

        protected override Faker<Admin> CreateFaker() => new Faker<Admin>()
            .RuleFor(x => x.Id, f => Guid.NewGuid())
            .RuleFor(x => x.Gender, f => f.PickRandom<Gender>() == Gender.Male ? "Male" : "Female")
            .RuleFor(x => x.Title, (f, x) => f.Name.Prefix(GetGender(x.Gender!)))
            .RuleFor(x => x.FirstName, (f, x) => f.Name.FirstName(GetGender(x.Gender!)))
            .RuleFor(x => x.LastName, (f, x) => f.Name.LastName(GetGender(x.Gender!)))
            .RuleFor(x => x.Address, f => f.Address.FullAddress())
            .RuleFor(x => x.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.DateOfBirth, f => DateOnly.FromDateTime(f.Person.DateOfBirth))
            .RuleFor(x => x.Email, (f, x) => _emailHelper.Create(x.FirstName, x.LastName!));
    }
}