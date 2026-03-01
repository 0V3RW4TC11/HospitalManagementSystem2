using Mapster;
using Services.Dtos.Patient;

namespace Presentation.ViewModels.Patient
{
    public class PatientCreateViewModel
    {
        public PatientViewModel Patient { get; set; }

        public PasswordCreateViewModel PasswordCreate { get; set; }

        public PatientCreateDto Dto
        {
            get
            {
                var dto = Patient.Adapt<PatientCreateDto>();
                dto.Password = PasswordCreate.Password;
                return dto;
            }
        }
    }
}
