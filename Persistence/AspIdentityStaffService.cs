using Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public sealed class AspIdentityStaffService : StaffService
    {
        private readonly HmsDbContext _context;

        public AspIdentityStaffService(HmsDbContext context)
        {
            _context = context;
        }

        protected override async Task<int> CountMatchingUserNamesAsync(string pattern, CancellationToken ct)
        {
            var sqlLikePattern = RegexToSqlLikeConverter.Convert(pattern);
            return await _context.Users.CountAsync(u => EF.Functions.Like(u.UserName!, sqlLikePattern), ct);
        }

        protected override async Task<bool> IsExisting(string username)
        {
            string normalizedUsername = username.ToLower();
            return await _context.Users.AnyAsync(u => u.UserName!.ToLower() == normalizedUsername);
        }
    }
}
