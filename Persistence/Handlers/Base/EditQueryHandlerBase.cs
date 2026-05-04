using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Handlers.Helpers;
using Queries.Shared;
using ViewModels.Shared;

namespace Persistence.Handlers.Base
{
    public class EditQueryHandlerBase<TEntity, TDataModel>(HmsDbContext context, UserManager<IdentityUser> userManager) :
        IRequestHandler<GetEditUserModel<TDataModel>, EditUserViewModel<TDataModel>>
        where TEntity : Entities.Entity
        where TDataModel : class
    {
        protected HmsDbContext Context { get; } = context;

        public virtual async Task<EditUserViewModel<TDataModel>> Handle(GetEditUserModel<TDataModel> request, CancellationToken cancellationToken)
        {
            var (Entity, User) = await UserQueryHelper.GetUserData<TEntity>(
                Context,
                userManager,
                request.Id, 
                cancellationToken);

            return new EditUserViewModel<TDataModel>
            {
                Id = Entity.Id,
                Data = Entity.Adapt<TDataModel>(),
                UserName = User.UserName!,
            };
        }
    }
}
