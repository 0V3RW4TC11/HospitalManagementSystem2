using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Queries.Admin;
using ViewModels.Admin;
using X.PagedList;
using X.PagedList.Extensions;

namespace Persistence.Handlers.Admin
{
    public class AdminQueryHandler : 
        IRequestHandler<GetPagedAdminsQuery, IPagedList<IndexViewModel>>,
        IRequestHandler<GetManageAdminQuery, ManageViewModel>
    {
        private readonly HmsDbContext _context;

        public AdminQueryHandler(HmsDbContext context) => _context = context;

        public async Task<IPagedList<IndexViewModel>> Handle(GetPagedAdminsQuery request, CancellationToken cancellationToken)
        {
            var adminViewModels = await _context.Admins
                .ProjectToType<IndexViewModel>()
                .ToListAsync(cancellationToken);

            return adminViewModels.ToPagedList(request.PageNumber, request.PageSize);
        }

        public async Task<ManageViewModel> Handle(GetManageAdminQuery request, CancellationToken cancellationToken)
        {
            var admin = await _context.Admins.SingleAsync(a => a.Id == request.Id, cancellationToken);
            
            var hmsuserid = request.Id.ToString();
            var claim = await _context.UserClaims.SingleAsync(c => 
                c.ClaimType == AppConstants.ClaimConstants.HmsUserId &&
                c.ClaimValue == hmsuserid, 
                cancellationToken);

            var userData = await _context.Users
                .Where(u => u.Id == claim.UserId)
                .Select(u => new { u.UserName, u.LockoutEnabled})
                .SingleAsync(cancellationToken);

            return new ManageViewModel
            {
                Id = admin.Id,
                UserName = userData.UserName ?? throw new ArgumentNullException(),
                IsLockedOut = userData.LockoutEnabled,
                Data = admin.Adapt<DataViewModel>()
            };
        }
    }
}
