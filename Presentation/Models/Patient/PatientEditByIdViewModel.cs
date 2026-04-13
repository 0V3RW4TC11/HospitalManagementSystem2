using Mapster;
using Services.Dtos.Patient;

namespace Presentation.Models.Patient
{
    public class PatientEditByIdViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public PatientViewModel Patient { get; set; }

        public PatientEditByIdViewModel() { }

        public PatientEditByIdViewModel(string userName, PatientDto patientDto)
        {
            Id = patientDto.Id;
            UserName = userName;
            Patient = patientDto.Adapt<PatientViewModel>();
        }

        public PatientDto Dto
        {
            get
            {
                var dto = Patient.Adapt<PatientDto>();
                dto.Id = Id;
                return dto;
            }
        }
    }
}
