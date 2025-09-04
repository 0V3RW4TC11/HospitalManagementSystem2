using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Admin
{
    public class AdminDetailsViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public bool IsLockedOut { get; set; }

        public string? Title { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        public string Gender { get; set; }

        public string Address { get; set; }

        [DisplayName("Phone Number")]
        public string Phone { get; set; }

        [DisplayName("Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Date Of Birth")]
        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }
    }
}
