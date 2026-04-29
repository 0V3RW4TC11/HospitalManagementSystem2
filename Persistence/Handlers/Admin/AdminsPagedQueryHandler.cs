using Persistence.Handlers.Base;
using ViewModels.Admin;

namespace Persistence.Handlers.Admin
{
    public class AdminsPagedQueryHandler(HmsDbContext context) :
        PagedQueryHandlerBase<Entities.Admin, AdminIndexViewModel>(context, x => x.FirstName);
}
