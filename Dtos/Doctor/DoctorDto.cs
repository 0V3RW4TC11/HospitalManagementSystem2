using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dtos.Doctor
{
    public record DoctorDto
    {
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        public string Gender { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        [DisplayName("Date of Birth")]
        [DataType(DataType.Date)]
        [Required]
        public DateOnly? DateOfBirth { get; set; }
    }
}