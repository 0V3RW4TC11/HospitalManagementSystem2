using Bogus;
using Domain.Entities;
using Seeders.Helpers;
using static Bogus.DataSets.Name;
using static Seeders.Helpers.FakerHelper;

namespace Seeders
{
    internal class DoctorSeeder : BaseAccountSeeder<Doctor>
    {
        private readonly DoctorSpecializationsSeeder _docSpecsSeeder;
        private readonly UniqueEmailsHelper _emailHelper = new(Constants.DomainNames.Organization);

        public DoctorSeeder(
            IServiceProvider services, 
            IEnumerable<Guid> specializationIds,
            string roleId, 
            string password) : base(
            services,
            roleId,
            password,
            x => x.Email!)
        {
            _docSpecsSeeder = new DoctorSpecializationsSeeder(specializationIds);
        }

        protected override List<Doctor> BatchFunc(int batchAmount)
        {
            var doctors = base.BatchFunc(batchAmount);
            _docSpecsSeeder.Create(doctors);
            return doctors;
        }

        protected override Faker<Doctor> CreateFaker() => new Faker<Doctor>()
            .RuleFor(x => x.Id, f => Guid.NewGuid())
            .RuleFor(x => x.Gender, f => f.PickRandom<Gender>() == Gender.Male ? "Male" : "Female")
            .RuleFor(x => x.FirstName, (f, x) => f.Name.FirstName(GetGender(x.Gender!)))
            .RuleFor(x => x.LastName, (f, x) => f.Name.LastName(GetGender(x.Gender!)))
            .RuleFor(x => x.Address, f => f.Address.FullAddress())
            .RuleFor(x => x.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.DateOfBirth, f => DateOnly.FromDateTime(f.Person.DateOfBirth))
            .RuleFor(x => x.Email, (f, x) => _emailHelper.Create(x.FirstName, x.LastName!));

        protected override List<Task> CreateInsertTasks()
        {
            var tasks = base.CreateInsertTasks();
            tasks.Add(
                AsyncHelper.InvokeAsync(async () =>
                {
                    await using var context = await _contextFactory.CreateDbContextAsync();
                    await _docSpecsSeeder.BulkInsertAsync(context);
                }));
            return tasks;
        }
    }
}
