using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Admin
{
    public class AdminCreateViewModel : AdminBaseViewModel
    {
        public PasswordCreateViewModel PasswordViewModel { get; set; }
    }
}
