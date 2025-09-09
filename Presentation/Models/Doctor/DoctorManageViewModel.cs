namespace Presentation.Models.Doctor
{
    public class DoctorManageViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public bool IsLockedOut { get; set; }

        public DoctorDetailsViewModel DetailsViewModel { get; set; }

        public DoctorSpecsViewModel SpecsViewModel { get; set; }
    }
}
