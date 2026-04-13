using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Account
{
    public class AccountLoginViewModel
    {
        [Required]
        [EmailAddress]
        [DisplayName("Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Remember me")]
        public bool IsPersistant { get; set; }
    }
}
