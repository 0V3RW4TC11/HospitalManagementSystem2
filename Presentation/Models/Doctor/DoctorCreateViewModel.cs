using Mapster;
using Services.Dtos.Doctor;
using Services.Dtos.Specialization;

namespace Presentation.Models.Doctor
{
    public class DoctorCreateViewModel
    {
        public DoctorEditViewModel Edit { get; set; }

        public PasswordCreateViewModel PasswordCreate { get; set; }

        public DoctorCreateDto Dto
        {
            get
            {
                var dto = Edit.Doctor.Adapt<DoctorCreateDto>();
                dto.SpecializationIds = Edit.SelectedSpecs;
                dto.Password = PasswordCreate.Password;
                return dto;
            }
        }

        public DoctorCreateViewModel()
        {
            
        }

        public DoctorCreateViewModel(IEnumerable<SpecializationDto> specializations)
        {
            Edit = new DoctorEditViewModel(specializations);
        }
    }
}
