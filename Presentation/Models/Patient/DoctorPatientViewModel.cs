using Mapster;
using Services.Dtos.Patient;

namespace Presentation.Models.Patient
{
    public class DoctorPatientViewModel
    {
        public Guid PatientId { get; set; }

        public string UserName { get; set; }

        public PatientViewModel Patient { get; set; }

        public DoctorPatientViewModel()
        {
            
        }

        public DoctorPatientViewModel(PatientDto dto, string userName)
        {
            PatientId = dto.Id;
            Patient = dto.Adapt<PatientViewModel>();
            UserName = userName;
        }
    }
}
