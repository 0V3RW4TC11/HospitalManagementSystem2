using Microsoft.AspNetCore.Identity;
using Persistence.Handlers.Base;
using ViewModels.Admin;

namespace Persistence.Handlers.Admin
{
    public class AdminUserQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) :
        UserQueryHandlerBase<Entities.Admin, AdminDataViewModel>(context, userManager);
}
