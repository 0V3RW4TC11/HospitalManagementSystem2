namespace Presentation.Models.Doctor
{
    public class DoctorProfileViewModel
    {
        public string Username { get; set; }

        public DoctorDetailsViewModel DetailsViewModel { get; set; }

        public DoctorSpecsViewModel SpecsViewModel { get; set; }
    }
}
