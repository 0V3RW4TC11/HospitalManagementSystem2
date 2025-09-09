using Domain.Entities;
using Domain.Repositories;

namespace Services
{
    internal sealed class DoctorSpecializationService
    {
        private readonly IDoctorSpecializationRepository _doctorSpecRepository;

        public DoctorSpecializationService(IDoctorSpecializationRepository doctorSpecRepository)
        {
            _doctorSpecRepository = doctorSpecRepository;
        }

        public async Task UpdateAsync(Guid doctorId, IEnumerable<Guid>? specIds)
        {
            if (specIds == null || !specIds.Any())
                return;

            var currentSpecs = (await _doctorSpecRepository.GetSpecIdsByDoctorIdAsync(doctorId)).ToArray();

            var newSpecs = specIds.ToArray();

            var intersect = newSpecs.Intersect(currentSpecs).ToArray();

            var toAdd = newSpecs
                .Except(intersect)
                .Select(id => new DoctorSpecialization { DoctorId = doctorId, SpecializationId = id });
            var toRemove = currentSpecs
                .Except(intersect)
                .Select(id => new DoctorSpecialization { DoctorId = doctorId, SpecializationId = id });

            _doctorSpecRepository.AddRange(toAdd);
            _doctorSpecRepository.RemoveRange(toRemove);
        }
    }
}
