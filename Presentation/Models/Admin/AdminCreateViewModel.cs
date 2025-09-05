using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Admin
{
    public class AdminCreateViewModel
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
        public DateOnly? DateOfBirth { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
