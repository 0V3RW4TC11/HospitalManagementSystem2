using System.Collections.Concurrent;

namespace Seeders.Helpers
{
    internal class UniqueEmailsHelper
    {
        private readonly ConcurrentDictionary<string, int> _usedEmailCounts = new(StringComparer.OrdinalIgnoreCase);
        private readonly string _domain;

        public UniqueEmailsHelper(string domain)
        {
            _domain = domain;
        }

        public string Create(string firstName, string lastName)
        {
            var baseName = $"{firstName.ToLower()}.{lastName.ToLower()}";
            var key = $"{baseName}@{_domain}";
            int next = _usedEmailCounts.GetValueOrDefault(key, 0) + 1;

            _usedEmailCounts[key] = next;

            return (next == 1)
                ? key
                : $"{baseName}{next}@{_domain}";
        }
    }
}
