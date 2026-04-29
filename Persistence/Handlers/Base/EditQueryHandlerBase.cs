using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Handlers.Helpers;
using Queries.Shared;
using ViewModels.User;

namespace Persistence.Handlers.Base
{
    public class EditQueryHandlerBase<TEntity, TDataModel>(HmsDbContext context, UserManager<IdentityUser> userManager) :
        IRequestHandler<GetEditModel<TDataModel>, EditViewModel<TDataModel>>
        where TEntity : Entities.Entity
        where TDataModel : class
    {
        protected HmsDbContext Context { get; } = context;
        protected UserManager<IdentityUser> UserManager { get; } = userManager;

        public virtual async Task<EditViewModel<TDataModel>> Handle(GetEditModel<TDataModel> request, CancellationToken cancellationToken)
        {
            var (Entity, User) = await UserQueryHelper.GetUserData<TEntity>(
                Context,
                UserManager,
                request.Id, 
                cancellationToken);

            return new EditViewModel<TDataModel>
            {
                Id = Entity.Id,
                Data = Entity.Adapt<TDataModel>(),
                UserName = User.UserName!,
            };
        }
    }
}
