using Microsoft.AspNetCore.Identity;
using Persistence.Handlers.Base;
using ViewModels.Patient;

namespace Persistence.Handlers.Patient
{
    public class PatientManageQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) :
        ManageQueryHandlerBase<Entities.Patient, PatientDataViewModel>(context, userManager);
}
