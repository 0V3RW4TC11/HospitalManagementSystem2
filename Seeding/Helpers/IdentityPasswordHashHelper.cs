using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Seeding.Helpers
{
    internal static class IdentityPasswordHashHelper
    {
        public static string HashPassword(IServiceProvider services, string password)
        {
            var passwordHasher = services.GetRequiredService<UserManager<IdentityUser>>().PasswordHasher;
            return passwordHasher.HashPassword(null!, password);
        }
    }
}
