using Ardalis.Specification;

namespace Specifications.Admin
{
    public class AdminIdByEmailSpec : SingleResultSpecification<Entities.Admin, Guid>
    {
        public AdminIdByEmailSpec(string email)
        {
            var normalizedEmail = email.ToLower();

            Query
                .Where(a => a.Email.ToLower() == normalizedEmail)
                .Select(a => a.Id);
        }
    }
}