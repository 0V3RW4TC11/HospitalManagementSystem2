using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Patient
{
    public class PatientCreateViewModel : PasswordCreateViewModel
    {
        public string? Title { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string? LastName { get; set; }

        public string Gender { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string Email { get; set; }

        [DisplayName("Blood Type")]
        public Constants.BloodType BloodType { get; set; }

        [DisplayName("Date of Birth")]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }
    }
}
