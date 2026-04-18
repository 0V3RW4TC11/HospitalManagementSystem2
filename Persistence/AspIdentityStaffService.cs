using Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Persistence
{
    internal sealed class AspIdentityStaffService : StaffService
    {
        private readonly HmsDbContext _context;

        public AspIdentityStaffService(HmsDbContext context)
        {
            _context = context;
        }

        protected override async Task<int> CountMatchingUserNamesAsync(string pattern, CancellationToken ct)
        {
            return await _context.Users.CountAsync(u => Regex.IsMatch(u.UserName!, pattern), ct);
        }

        protected override async Task<bool> IsExisting(string username)
        {
            string normalizedUsername = username.ToLower();
            return await _context.Users.AnyAsync(u => u.UserName!.ToLower() == normalizedUsername);
        }
    }
}
