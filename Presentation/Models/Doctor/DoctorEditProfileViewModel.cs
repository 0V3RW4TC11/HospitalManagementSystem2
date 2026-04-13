using Mapster;
using Services.Dtos.Doctor;
using Services.Dtos.Specialization;

namespace Presentation.Models.Doctor
{
    public class DoctorEditProfileViewModel
    {
        public DoctorEditViewModel Edit { get; set; }

        public string UserName { get; set; }

        public DoctorEditProfileViewModel()
        {
            
        }

        public DoctorEditProfileViewModel(string userName, DoctorDto doctor, IEnumerable<SpecializationDto> allSpecs)
        {
            UserName = userName;
            Edit = new DoctorEditViewModel(doctor, allSpecs);
        }

        public DoctorDto Dto(Guid id)
        {
            var dto = Edit.Doctor.Adapt<DoctorDto>();
            dto.Id = id;
            dto.SpecializationIds = Edit.SelectedSpecs;
            return dto;
        }
    }
}
