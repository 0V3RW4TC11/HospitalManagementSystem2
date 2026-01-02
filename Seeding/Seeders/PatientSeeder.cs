using Bogus;
using Domain.Entities;
using static Bogus.DataSets.Name;
using static Seeding.Helpers.FakerHelper;

namespace Seeding.Seeders
{
    internal class PatientSeeder : BaseAccountSeeder<Patient>
    {
        public PatientSeeder(IServiceProvider services, string roleId, string password) : base(
            services,
            roleId,
            password,
            x => x.Email!)
        { }

        protected override Faker<Patient> CreateFaker() => new Faker<Patient>()
            .RuleFor(x => x.Id, f => Guid.NewGuid())
            .RuleFor(x => x.Gender, f => f.PickRandom<Gender>() == Gender.Male ? "Male" : "Female")
            .RuleFor(x => x.Title, (f, x) => f.Name.Prefix(GetGender(x.Gender!)))
            .RuleFor(x => x.FirstName, (f, x) => f.Name.FirstName(GetGender(x.Gender!)))
            .RuleFor(x => x.LastName, (f, x) => f.Name.LastName(GetGender(x.Gender!)))
            .RuleFor(x => x.Address, f => f.Address.FullAddress())
            .RuleFor(x => x.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.DateOfBirth, f => DateOnly.FromDateTime(f.Person.DateOfBirth))
            .RuleFor(x => x.Email, (f, x) => f.Internet.Email(x.FirstName, x.LastName, null, f.UniqueIndex.ToString()))
            .RuleFor(x => x.BloodType, f => f.PickRandom<Constants.BloodType>());
    }
}
