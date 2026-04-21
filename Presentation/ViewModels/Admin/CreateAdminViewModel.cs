using Commands.Admin;
using Mapster;
using Presentation.ViewModels.Components;

namespace Presentation.ViewModels.Admin
{
    public class CreateAdminViewModel : AdminViewModel
    {
        public CreateAdminCommand Command
        {
            get
            {
                var data = this.Adapt<AdminData>();
                return new(data, CreatePassword.Password);
            }
        }

        public CreatePasswordComponent CreatePassword { get; set; }
    }
}
