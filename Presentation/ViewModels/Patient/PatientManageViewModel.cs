using Mapster;
using Services.Dtos.Patient;

namespace Presentation.ViewModels.Patient
{
    public class PatientManageViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public bool IsLockedOut { get; set; }

        public PatientViewModel Patient { get; set; }

        public PatientManageViewModel() { }

        public PatientManageViewModel(string userName, bool isLockedOut, PatientDto patientDto)
        {
            Id = patientDto.Id;
            UserName = userName;
            IsLockedOut = isLockedOut;
            Patient = patientDto.Adapt<PatientViewModel>();
        }
    }
}
