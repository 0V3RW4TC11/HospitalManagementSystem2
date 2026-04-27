using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ViewModels.Doctor
{
    public class DoctorIndexViewModel
    {
        public Guid Id { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string? LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
