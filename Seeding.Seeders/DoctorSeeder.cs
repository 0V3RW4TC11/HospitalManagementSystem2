using Bogus;
using Entities;
using Seeding.Seeders.Helpers;

namespace Seeding.Seeders
{
    internal class DoctorSeeder : BaseAccountSeeder<Doctor>
    {
        private readonly DoctorSpecializationsSeeder _docSpecsSeeder;

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

        public override void Dispose()
        {
            _docSpecsSeeder.Dispose();
            base.Dispose();
        }

        protected override List<Doctor> BatchFunc(int batchAmount)
        {
            var doctors = base.BatchFunc(batchAmount);
            _docSpecsSeeder.Create(doctors);
            return doctors;
        }

        protected override Faker<Doctor> CreateFaker() => new Faker<Doctor>()
            .RuleFor(x => x.Id, f => Guid.NewGuid())
            .RuleFor(x => x.Gender, FakerHelper.PickRandomGender)
            .RuleFor(x => x.FirstName, (f, x) => f.Name.FirstName(FakerHelper.GetGender(x.Gender!)))
            .RuleFor(x => x.LastName, (f, x) => f.Name.LastName(FakerHelper.GetGender(x.Gender!)))
            .RuleFor(x => x.Address, f => f.Address.FullAddress())
            .RuleFor(x => x.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.DateOfBirth, f => DateOnly.FromDateTime(f.Person.DateOfBirth))
            .RuleFor(x => x.Email, (f, x) => FakerHelper.GetStaffEmail(x.FirstName, x.LastName));

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
