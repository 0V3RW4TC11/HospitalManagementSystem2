using Domain.Entities;
using System.Text.RegularExpressions;

namespace Abstractions
{
    public abstract class IdentityManager
    {
        public async Task CreateAsync(Admin admin, string password, CancellationToken cancellationToken)
        {
            var userName = await CreateStaffUsername(admin.FirstName, admin.LastName, cancellationToken);
            await CreateIdentityAsync(admin.Id, userName, password, Constants.AuthRoles.Admin, cancellationToken);
        }

        public async Task CreateAsync(Doctor doctor, string password, CancellationToken cancellationToken)
        {
            var userName = await CreateStaffUsername(doctor.FirstName, doctor.LastName, cancellationToken);
            await CreateIdentityAsync(doctor.Id, userName, password, Constants.AuthRoles.Doctor, cancellationToken);
        }

        public async Task CreateAsync(Patient patient, string password, CancellationToken cancellationToken)
        {
            await CreateIdentityAsync(patient.Id, patient.Email, password, Constants.AuthRoles.Patient, cancellationToken);
        }

        public abstract Task DeleteAsync(Guid id, CancellationToken cancellationToken);

        protected abstract Task<int> CountMatchingEmails(Regex pattern, CancellationToken ct);

        protected abstract Task CreateIdentityAsync(Guid id, string userName, string password, string role, CancellationToken ct);

        private async Task<string> CreateStaffUsername(string firstName, string? lastName, CancellationToken ct)
        {
            var name = lastName == null ? firstName : $"{firstName}.{lastName}";
            var pattern = GenerateRegexPattern(name);
            int count = await CountMatchingEmails(pattern, ct);

            return count == 0 ?
                $"{name}@{Constants.DomainNames.Organization}" :
                $"{name}{count + 1}@{Constants.DomainNames.Organization}";
        }

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
