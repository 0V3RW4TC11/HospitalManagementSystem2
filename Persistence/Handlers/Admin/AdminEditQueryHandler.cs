using Microsoft.AspNetCore.Identity;
using Persistence.Handlers.Base;
using ViewModels.Admin;

namespace Persistence.Handlers.Admin
{
    public class AdminEditQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) :
        EditQueryHandlerBase<Entities.Admin, AdminDataViewModel>(context, userManager);
}
