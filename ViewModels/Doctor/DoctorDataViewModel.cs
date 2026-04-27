using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ViewModels.Doctor
{
    public class DoctorDataViewModel
    {
        public string? Title { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string? LastName { get; set; }

        public string? Address { get; set; }

        [DisplayName("Phone Number")]
        public string Phone { get; set; }

        [DisplayName("Email Address")]
        public string Email { get; set; }

        [DisplayName("Date Of Birth")]
        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }
    }
}
