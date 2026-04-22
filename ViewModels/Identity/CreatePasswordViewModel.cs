using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ViewModels.Identity
{
    public class CreatePasswordViewModel
    {
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
