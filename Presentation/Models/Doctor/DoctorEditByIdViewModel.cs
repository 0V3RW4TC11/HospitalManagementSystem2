using Mapster;
using Services.Dtos.Doctor;
using Services.Dtos.Specialization;

namespace Presentation.Models.Doctor
{
    public class DoctorEditByIdViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public DoctorEditViewModel Edit { get; set; }

        public DoctorDto Dto
        {
            get
            {
                var dto = Edit.Doctor.Adapt<DoctorDto>();
                dto.Id = Id;
                dto.SpecializationIds = Edit.SelectedSpecs;
                return dto;
            }
        }

        public DoctorEditByIdViewModel()
        {
            
        }

        public DoctorEditByIdViewModel(
            string userName,
            DoctorDto doctor,
            IEnumerable<SpecializationDto> specializations)
        {
            Id = doctor.Id;
            UserName = userName;
            Edit = new DoctorEditViewModel(doctor, specializations);
        }
    }
}
