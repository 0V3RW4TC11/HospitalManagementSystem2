using Domain.Entities;
using EFCore.BulkExtensions;
using Persistence;
using System.Collections.Concurrent;
using Bogus;

namespace Seeding.Seeders
{
    internal class DoctorSpecializationsSeeder
    {
        private readonly ConcurrentBag<DoctorSpecialization> _doctorSpecializationsBag;
        private readonly Faker _faker;
        private readonly Guid[] _specializationIds;

        public DoctorSpecializationsSeeder(IEnumerable<Guid> specializationIds)
        {
            _doctorSpecializationsBag = new();
            _faker = new Faker();
            _specializationIds = specializationIds.ToArray();
        }

        public void Create(IEnumerable<Doctor> doctors)
        {
            var doctorSpecializations = new List<DoctorSpecialization>();

            foreach (var doctor in doctors)
            {
                // Pick 1 to 5 random specializations for each doctor
                var selectedIds = _faker.PickRandom(_specializationIds, _faker.Random.Int(1, 5));
                
                foreach (var id in selectedIds)
                {
                    doctorSpecializations.Add(new DoctorSpecialization
                    {
                        DoctorId = doctor.Id,
                        SpecializationId = id
                    });
                }
            }

            doctorSpecializations.ForEach(_doctorSpecializationsBag.Add);
        }

        public async Task BulkInsertAsync(RepositoryDbContext context)
        {
            await context.BulkInsertAsync(
                _doctorSpecializationsBag,
                new BulkConfig
                {
                    PreserveInsertOrder = false
                });
        }
    }
}
