using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.ViewModels.Account
{
    public class AccountResetPasswordViewModel
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        [DisplayName("New Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
