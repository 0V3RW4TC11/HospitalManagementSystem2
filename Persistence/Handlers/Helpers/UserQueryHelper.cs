using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Helpers;

namespace Persistence.Handlers.Helpers
{
    internal static class UserQueryHelper
    {
        public static async Task<(TEntity Entity, IdentityUser User)> GetUserData<TEntity>(
            HmsDbContext context, 
            UserManager<IdentityUser> userManager, 
            Guid id, 
            CancellationToken ct)
            where TEntity : Entity
        {
            var entity = await context.Set<TEntity>().SingleAsync(e => e.Id == id, ct);
            var user = await IdentityHelper.GetUserFromHmsIdAsync(userManager, id);

            if (user.UserName == null)
                throw new Exception("Identity missing username");

            return (entity, user);
        }
    }
}
