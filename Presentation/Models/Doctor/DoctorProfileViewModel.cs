using Services.Dtos.Doctor;
using Services.Dtos.Specialization;

namespace Presentation.Models.Doctor
{
    public class DoctorProfileViewModel
    {
        public string UserName { get; set; }

        public DoctorAndSpecsViewModel DoctorAndSpecs { get; set; }

        public DoctorProfileViewModel()
        {
            
        }

        public DoctorProfileViewModel(string userName, DoctorDto doctor, IEnumerable<SpecializationDto> specDtos)
        {
            UserName = userName;

            DoctorAndSpecs = new DoctorAndSpecsViewModel(doctor, specDtos);
        }
    }
}
