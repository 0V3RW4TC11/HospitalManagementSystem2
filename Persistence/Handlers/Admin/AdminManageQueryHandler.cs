using Microsoft.AspNetCore.Identity;
using Persistence.Handlers.Base;
using ViewModels.Admin;

namespace Persistence.Handlers.Admin
{
    public class AdminManageQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) :
        ManageQueryHandlerBase<Entities.Admin, AdminDataViewModel>(context, userManager);
}
