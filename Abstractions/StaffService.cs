using System.Text.RegularExpressions;

namespace Abstractions
{
    public abstract class StaffService
    {
        public async Task<string> CreateStaffUsernameAsync(string firstName, string? lastName, CancellationToken ct = default)
        {
            var name = lastName == null ? firstName : $"{firstName}.{lastName}";
            var pattern = CreateRegex(name);
            int count = await CountMatchingUserNamesAsync(pattern, ct);

            if (count == 0)
                return $"{name}@{Constants.DomainNames.Organization}";
            else
            {
                ++count; // pre increment count

                while (await IsExisting($"{name}{count}@{Constants.DomainNames.Organization}"))
                {
                    ++count; // increment count again if name is taken
                }

                return $"{name}{count}@{Constants.DomainNames.Organization}";
            }
        }

        protected abstract Task<int> CountMatchingUserNamesAsync(Regex pattern, CancellationToken ct);

        protected abstract Task<bool> IsExisting(string username);

        private static Regex CreateRegex(string name)
        {
            string escapedName = Regex.Escape(name);
            string escapedDomain = Regex.Escape(Constants.DomainNames.Organization);

            string pattern =
                $@"^{escapedName}(\d+)?@{escapedDomain}$";

            return new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
    }
}
