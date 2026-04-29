using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Helpers;
using Queries.Shared;
using ViewModels.User;

namespace Persistence.Handlers.Base
{
    public class UserQueryHandlerBase<TEntity, TDataModel>(
        HmsDbContext context, 
        UserManager<IdentityUser> userManager) :
        IRequestHandler<GetEditModel<TDataModel>, EditViewModel<TDataModel>>,
        IRequestHandler<GetManageModel<TDataModel>, ManageViewModel<TDataModel>>
        where TEntity : Entities.Entity
        where TDataModel : class
    {
        public async Task<EditViewModel<TDataModel>> Handle(GetEditModel<TDataModel> request, CancellationToken cancellationToken)
        {
            var (Entity, User) = await GetUserData(request.Id, cancellationToken);

            return new EditViewModel<TDataModel>
            {
                Id = Entity.Id,
                Data = Entity.Adapt<TDataModel>(),
                UserName = User.UserName!,
            };
        }

        public async Task<ManageViewModel<TDataModel>> Handle(GetManageModel<TDataModel> request, CancellationToken cancellationToken)
        {
            var (Entity, User) = await GetUserData(request.Id, cancellationToken);

            return new ManageViewModel<TDataModel>
            {
                Id = Entity.Id,
                Data = Entity.Adapt<TDataModel>(),
                UserName = User.UserName!,
                IsLockedOut = User.LockoutEnabled
            };
        }

        private async Task<(TEntity Entity, IdentityUser User)> GetUserData(Guid id, CancellationToken ct)
        {
            var entity = await context.Set<TEntity>().SingleAsync(e => e.Id == id, ct);
            var user = await IdentityHelper.GetUserFromHmsIdAsync(userManager, id);

            if (user.UserName == null)
                throw new Exception("Identity missing username");

            return (entity, user);
        }
    }
}
