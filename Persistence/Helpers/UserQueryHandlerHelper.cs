using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ViewModels.User;
using X.PagedList;
using X.PagedList.EF;

namespace Persistence.Helpers
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // TODO: Make into generic handler
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    internal class UserQueryHandlerHelper(HmsDbContext context, UserManager<IdentityUser> userManager)
    {
        private async Task<TEntity> GetEntityAsync<TEntity>(Guid id, CancellationToken ct)
            where TEntity : Entities.Entity
        {
            return await context.Set<TEntity>().SingleAsync(e => e.Id == id, ct);
        }

        private async Task<IdentityUser> GetUserAsync(Guid id)
        {
            return await IdentityHelper.GetUserFromHmsIdAsync(userManager, id);
        }

        public async Task<EditViewModel<TResult>> CreateEditViewModelAsync<TEntity, TResult>(Guid id, CancellationToken ct)
            where TEntity : Entities.Entity
            where TResult : class
        {
            var entity = await GetEntityAsync<TEntity>(id, ct);
            var user = await GetUserAsync(id);

            return new EditViewModel<TResult>
            {
                Id = entity.Id,
                UserName = user.UserName ?? throw new ArgumentNullException(),
                Data = entity.Adapt<TResult>()
            };
        }

        public async Task<ManageViewModel<TResult>> CreateManageViewModelAsync<TEntity, TResult>(Guid id, CancellationToken ct) 
            where TEntity : Entities.Entity
            where TResult : class
        {
            var entity = await GetEntityAsync<TEntity>(id, ct);
            var user = await GetUserAsync(id);

            return new ManageViewModel<TResult>
            {
                Id = entity.Id,
                UserName = user.UserName ?? throw new ArgumentNullException(),
                IsLockedOut = user.LockoutEnabled,
                Data = entity.Adapt<TResult>()
            };
        }

        public async Task<IPagedList<TResult>> CreatePagedModelsAsync<TEntity, TResult>(
            Expression<Func<TEntity, string>> keySelector,
            int pageNumber,
            int pageSize,
            CancellationToken ct)
            where TEntity : Entities.Entity
            where TResult : class
        {
            var totalCount = await context.Set<TEntity>().CountAsync(ct);
            return await context.Set<TEntity>()
                .OrderBy(keySelector)
                .ProjectToType<TResult>()
                .ToPagedListAsync(pageNumber, pageSize, totalCount);
        }
    }
}
