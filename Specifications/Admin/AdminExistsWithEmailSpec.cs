using Ardalis.Specification;

namespace Specifications.Admin
{
    public class AdminExistsWithEmailSpec : Specification<Domain.Entities.Admin>
    {
        public AdminExistsWithEmailSpec(string email)
        {
            var normalizedEmail = email.ToLower();
            Query.Where(a => a.Email.ToLower() == normalizedEmail);
        }
    }
}
