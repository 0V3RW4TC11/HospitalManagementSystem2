namespace Presentation.Models.Doctor
{
    public class DoctorEditViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public DoctorDetailsViewModel DetailsViewModel { get; set; }

        public IEnumerable<DoctorEditSpecViewModel> EditSpecViewModels { get; set; }
    }
}
