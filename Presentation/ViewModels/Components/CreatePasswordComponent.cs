using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.ViewModels.Components
{
    internal class CreatePasswordComponent
    {
        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
