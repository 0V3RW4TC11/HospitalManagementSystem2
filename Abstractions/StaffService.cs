using System.Text.RegularExpressions;

namespace Abstractions
{
    public abstract class StaffService
    {
        public async Task<string> CreateStaffUsernameAsync(string firstName, string? lastName, CancellationToken ct)
        {
            var name = lastName == null ? firstName : $"{firstName}.{lastName}";
            var pattern = GenerateRegexPattern(name);
            int count = await CountMatchingEmailsAsync(pattern, ct);

            return count == 0 ?
                $"{name}@{Constants.DomainNames.Organization}" :
                $"{name}{count + 1}@{Constants.DomainNames.Organization}";
        }

        protected abstract Task<int> CountMatchingEmailsAsync(Regex pattern, CancellationToken ct);

        private static Regex GenerateRegexPattern(string name)
        {
            string escapedName = Regex.Escape(name);
            string escapedDomain = Regex.Escape(Constants.DomainNames.Organization);

            string pattern =
                $@"^{escapedName}(\d+)?@{escapedDomain}$";

            return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
    }
}
