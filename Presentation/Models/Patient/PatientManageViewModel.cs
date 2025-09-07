namespace Presentation.Models.Patient
{
    public class PatientManageViewModel : PatientBaseViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public bool IsLockedOut { get; set; }
    }
}
