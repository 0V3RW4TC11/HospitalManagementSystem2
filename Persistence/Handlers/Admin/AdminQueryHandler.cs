using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Helpers;
using Queries.Admin;
using ViewModels.Admin;
using X.PagedList;
using X.PagedList.Extensions;

namespace Persistence.Handlers.Admin
{
    public class AdminQueryHandler : 
        IRequestHandler<GetPagedAdminsQuery, IPagedList<IndexViewModel>>,
        IRequestHandler<GetManageAdminModel, ManageViewModel>,
        IRequestHandler<GetEditAdminModel, EditViewModel>
    {
        private readonly HmsDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IPagedList<IndexViewModel>> Handle(GetPagedAdminsQuery request, CancellationToken cancellationToken)
        {
            var adminViewModels = await _context.Admins
                .ProjectToType<IndexViewModel>()
                .ToListAsync(cancellationToken);

            return adminViewModels.ToPagedList(request.PageNumber, request.PageSize);
        }

        public async Task<ManageViewModel> Handle(GetManageAdminModel request, CancellationToken cancellationToken)
        {
            var admin = await _context.Admins.SingleAsync(a => a.Id == request.Id, cancellationToken);
            var user = await IdentityHelper.GetUserFromHmsIdAsync(_userManager, request.Id);

            return new ManageViewModel
            {
                Id = admin.Id,
                UserName = user.UserName ?? throw new ArgumentNullException(),
                IsLockedOut = user.LockoutEnabled,
                Data = admin.Adapt<DataViewModel>()
            };
        }

        public async Task<EditViewModel> Handle(GetEditAdminModel request, CancellationToken cancellationToken)
        {
            var admin = await _context.Admins.SingleAsync(a => a.Id == request.Id, cancellationToken);
            var user = await IdentityHelper.GetUserFromHmsIdAsync(_userManager, request.Id);

            return new EditViewModel
            {
                Id = admin.Id,
                UserName = user.UserName ?? throw new ArgumentNullException(),
                Data = admin.Adapt<DataViewModel>()
            };
        }
    }
}
