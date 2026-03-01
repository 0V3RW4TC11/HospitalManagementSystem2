using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Queries.Dtos.Admin
{
    public class AdminIndexItemDto
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
