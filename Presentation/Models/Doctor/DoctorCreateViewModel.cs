namespace Presentation.Models.Doctor
{
    public class DoctorCreateViewModel
    {
        public DoctorDetailsViewModel DetailsViewModel { get; set; }

        public PasswordCreateViewModel PasswordViewModel { get; set; }

        public DoctorEditSpecViewModel[] EditSpecViewModels { get; set; }
    }
}
