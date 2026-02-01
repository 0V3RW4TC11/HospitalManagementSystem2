using Ardalis.Specification;

namespace Specifications.Admin
{
    public class GetAdminIdByEmailSpec : SingleResultSpecification<Domain.Entities.Admin, Guid>
    {
        public GetAdminIdByEmailSpec(string email)
        {
            var normalizedEmail = email.ToLower();

            Query
                .Where(a => a.Email.ToLower() == normalizedEmail)
                .Select(a => a.Id);
        }
    }
}
