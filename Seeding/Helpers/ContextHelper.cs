using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Seeding.Helpers
{
    internal static class ContextHelper
    {
        public static async Task<bool> HasDataAsync(RepositoryDbContext context)
        {
            if (await context.Accounts.AnyAsync())
                return true;
            if (await context.Admins.AnyAsync())
                return true;
            if (await context.Patients.AnyAsync())
                return true;
            if (await context.Doctors.AnyAsync())
                return true;
            if (await context.DoctorSpecializations.AnyAsync())
                return true;
            if (await context.Specializations.AnyAsync())
                return true;
            if (await context.Attendances.AnyAsync())
                return true;
            if (await context.Users.AnyAsync())
                return true;

            return false;
        }

        public static async Task ResetDatabase(RepositoryDbContext context)
        {
            await Task.WhenAll(
                context.Accounts.ExecuteDeleteAsync(),
                context.Admins.ExecuteDeleteAsync(),
                context.Patients.ExecuteDeleteAsync(),
                context.Doctors.ExecuteDeleteAsync(),
                context.DoctorSpecializations.ExecuteDeleteAsync(),
                context.Specializations.ExecuteDeleteAsync(),
                context.Attendances.ExecuteDeleteAsync(),
                context.Users.ExecuteDeleteAsync()
            );
        }
    }
}
