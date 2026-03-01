using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.ViewModels.Account
{
    public class AccountChangePasswordViewModel
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        [DisplayName("Current Password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [DisplayName("New Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "New/Confirm passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
