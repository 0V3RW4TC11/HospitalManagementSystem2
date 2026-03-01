using Commands.Admin;
using Mapster;
using Presentation.ViewModels.Components;

namespace Presentation.ViewModels.Admin
{
    internal class CreateAdminViewModel : AdminViewModel
    {
        public CreatePasswordComponent CreatePassword { get; set; }

        public CreateAdminCommand Command => this.Adapt<CreateAdminCommand>();
    }
}
