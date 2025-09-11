using Mapster;
using Services.Dtos.Doctor;
using Services.Dtos.Specialization;

namespace Presentation.Models.Doctor
{
    public class DoctorAndSpecsViewModel
    {
        public DoctorViewModel Doctor { get; set; }

        public IEnumerable<string> Specializations { get; set; }

        public DoctorAndSpecsViewModel()
        {
            
        }

        public DoctorAndSpecsViewModel(DoctorDto doctor, IEnumerable<SpecializationDto> specDtos)
        {
            Doctor = doctor.Adapt<DoctorViewModel>();
            Specializations = specDtos
                .Where(s => doctor.SpecializationIds.Contains(s.Id))
                .Select(s => s.Name);
        }
    }
}
