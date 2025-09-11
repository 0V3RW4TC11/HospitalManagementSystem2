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

        public DoctorProfileViewModel(DoctorDto doctor, IEnumerable<SpecializationDto> specializations, string userName)
        {
            DoctorAndSpecs = new DoctorAndSpecsViewModel(doctor, specializations);

            UserName = userName;
        }
    }
}
