using Mapster;
using Services.Dtos.Patient;

namespace Presentation.Models.Patient
{
    public class PatientProfileViewModel
    {
        public string UserName { get; set; }

        public PatientViewModel Patient { get; set; }

        public PatientProfileViewModel() { }

        public PatientProfileViewModel(string userName, PatientDto patient)
        {
            UserName = userName;
            Patient = patient.Adapt<PatientViewModel>();
        }

        public PatientDto Dto(Guid id)
        {
            var dto = Patient.Adapt<PatientDto>();
            dto.Id = id;
            return dto;
        }
    }
}
