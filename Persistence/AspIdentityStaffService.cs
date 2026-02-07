using Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Persistence
{
    internal sealed class AspIdentityStaffService : StaffService
    {
        private readonly RepositoryDbContext _context;

        public AspIdentityStaffService(RepositoryDbContext context)
        {
            _context = context;
        }

        protected override async Task<int> CountMatchingUserNamesAsync(Regex pattern, CancellationToken ct)
        {
            return await _context.Users.CountAsync(u => pattern.IsMatch(u.UserName!), ct);
        }
    }
}
