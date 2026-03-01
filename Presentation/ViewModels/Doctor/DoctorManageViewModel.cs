using Services.Dtos.Doctor;
using Services.Dtos.Specialization;

namespace Presentation.ViewModels.Doctor
{
    public class DoctorManageViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public bool IsLockedOut { get; set; }

        public DoctorAndSpecsViewModel DoctorAndSpecs { get; set; }

        public DoctorManageViewModel()
        {
            
        }

        public DoctorManageViewModel(
            string userName, 
            bool isLockedOut,
            DoctorDto doctor,
            IEnumerable<SpecializationDto> specializations)
        {
            Id = doctor.Id;
            Username = userName;
            IsLockedOut = isLockedOut;
            DoctorAndSpecs = new DoctorAndSpecsViewModel(doctor, specializations);
        }
    }
}
