using System.Text.RegularExpressions;

namespace Abstractions
{
    public abstract class StaffService
    {
        protected abstract Task<int> CountMatchingUserNamesAsync(string pattern, CancellationToken ct);
        protected abstract Task<bool> IsExisting(string username);

        public async Task<string> CreateStaffUsernameAsync(string firstName, string? lastName, CancellationToken ct = default)
        {
            var name = lastName == null ? firstName.ToLower() : $"{firstName.ToLower()}.{lastName.ToLower()}";
            var pattern = BuildPattern(name);
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

        private static string BuildPattern(string name)
        {
            string escapedName = Regex.Escape(name);
            string escapedDomain = Regex.Escape(Constants.DomainNames.Organization);

            return $@"^{escapedName}(\d+)?@{escapedDomain}$";
        }
    }
}
