using Abstractions;
using System.Text.RegularExpressions;

namespace Persistence
{
    internal sealed class AspIdentityStaffService : StaffService
    {
        protected override Task<int> CountMatchingEmailsAsync(Regex pattern, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
