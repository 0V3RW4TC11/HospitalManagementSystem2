namespace Presentation.Models.Patient
{
    public class PatientEditViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public PatientDetailsViewModel DetailsViewModel { get; set; }
    }
}
