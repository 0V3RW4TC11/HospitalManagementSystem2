using Domain.Entities;
using EFCore.BulkExtensions;
using Persistence;
using System.Collections.Concurrent;
using Bogus;

namespace Seeders
{
    internal class DoctorSpecializationsSeeder : IDisposable
    {
        private readonly ConcurrentBag<DoctorSpecialization> _doctorSpecializationsBag;
        private readonly Guid[] _specializationIds;

        public DoctorSpecializationsSeeder(IEnumerable<Guid> specializationIds)
        {
            _doctorSpecializationsBag = new();
            _specializationIds = specializationIds.ToArray();
        }

        public void Create(IEnumerable<Doctor> doctors)
        {
            var doctorSpecializations = new List<DoctorSpecialization>();
            
            // Create a thread-local Faker to avoid memory accumulation
            var faker = new Faker();

            foreach (var doctor in doctors)
            {
                // Pick 1 to 5 random specializations for each doctor
                var selectedIds = faker.PickRandom(_specializationIds, faker.Random.Int(1, 5));
                
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

        public void Dispose()
        {
            _doctorSpecializationsBag.Clear();
        }
    }
}
