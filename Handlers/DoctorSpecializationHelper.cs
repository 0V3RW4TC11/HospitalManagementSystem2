using Abstractions;
using Domain.Entities;
using Specifications.Doctor;

namespace Handlers
{
    internal class DoctorSpecializationHelper
    {
        private readonly IRepository<DoctorSpecialization> _repository;

        public DoctorSpecializationHelper(IRepository<DoctorSpecialization> repository)
        {
            _repository = repository;
        }

        public async Task UpdateAsync(Guid doctorId, IEnumerable<Guid> newSpecIds, CancellationToken ct = default)
        {
            var currentSpecIds = await _repository.ListAsync(new SpecializationIdsByDoctorIdSpec(doctorId), ct)
                ?? throw new Exception("No Specializations found for Doctor.");

            var intersect = newSpecIds.Intersect(currentSpecIds).ToList();

            var toAdd = newSpecIds
                .Except(intersect)
                .Select(id => new DoctorSpecialization { DoctorId = doctorId, SpecializationId = id });
            var toRemove = currentSpecIds
                .Except(intersect)
                .Select(id => new DoctorSpecialization { DoctorId = doctorId, SpecializationId = id });

            await _repository.AddRangeAsync(toAdd, ct);
            await _repository.DeleteRangeAsync(toRemove, ct);
        }
    }
}