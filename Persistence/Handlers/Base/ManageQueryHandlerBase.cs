using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Handlers.Helpers;
using Queries.Shared;
using ViewModels.Shared;

namespace Persistence.Handlers.Base
{
    public class ManageQueryHandlerBase<TEntity, TDataModel>(HmsDbContext context, UserManager<IdentityUser> userManager) :
        IRequestHandler<GetManageUserModel<TDataModel>, ManageUserViewModel<TDataModel>>
        where TEntity : Entities.Entity
        where TDataModel : class
    {
        protected HmsDbContext Context { get; } = context;

        public virtual async Task<ManageUserViewModel<TDataModel>> Handle(GetManageUserModel<TDataModel> request, CancellationToken cancellationToken)
        {
            var (Entity, User) = await UserQueryHelper.GetUserData<TEntity>(
                Context,
                userManager,
                request.Id,
                cancellationToken);

            return new ManageUserViewModel<TDataModel>
            {
                Id = Entity.Id,
                Data = Entity.Adapt<TDataModel>(),
                UserName = User.UserName!,
                IsLockedOut = User.LockoutEnabled
            };
        }
    }
}
