using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Admin
{
    public class AdminDetailsViewModel
    {
        public string? Title { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string? LastName { get; set; }

        public string? Gender { get; set; }

        public string? Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        [DisplayName("Date Of Birth")]
        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }
    }
}
