using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Helpers;
using Queries.Admin;
using ViewModels.Admin;
using ViewModels.User;
using X.PagedList;
using X.PagedList.EF;

namespace Persistence.Handlers.Admin
{
    public class AdminQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) : 
        IRequestHandler<GetPagedAdminsQuery, IPagedList<IndexViewModel>>,
        IRequestHandler<GetManageAdminModel, ManageViewModel<DataViewModel>>,
        IRequestHandler<GetEditAdminModel, EditViewModel<DataViewModel>>
    {
        public async Task<IPagedList<IndexViewModel>> Handle(GetPagedAdminsQuery request, CancellationToken cancellationToken)
        {
            var totalCount = await context.Admins.CountAsync(cancellationToken);
            return await context.Admins
                .OrderBy(a => a.FirstName)
                .ProjectToType<IndexViewModel>()
                .ToPagedListAsync(request.PageNumber, request.PageSize, totalCount);
        }

        public async Task<ManageViewModel<DataViewModel>> Handle(GetManageAdminModel request, CancellationToken cancellationToken)
        {
            var admin = await context.Admins.SingleAsync(a => a.Id == request.Id, cancellationToken);
            var user = await IdentityHelper.GetUserFromHmsIdAsync(userManager, request.Id);

            return new ManageViewModel<DataViewModel>
            {
                Id = admin.Id,
                UserName = user.UserName ?? throw new ArgumentNullException(),
                IsLockedOut = user.LockoutEnabled,
                Data = admin.Adapt<DataViewModel>()
            };
        }

        public async Task<EditViewModel<DataViewModel>> Handle(GetEditAdminModel request, CancellationToken cancellationToken)
        {
            var admin = await context.Admins.SingleAsync(a => a.Id == request.Id, cancellationToken);
            var user = await IdentityHelper.GetUserFromHmsIdAsync(userManager, request.Id);

            return new EditViewModel<DataViewModel>
            {
                Id = admin.Id,
                UserName = user.UserName ?? throw new ArgumentNullException(),
                Data = admin.Adapt<DataViewModel>()
            };
        }
    }
}
