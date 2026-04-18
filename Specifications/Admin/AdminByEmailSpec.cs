using Ardalis.Specification;

namespace Specifications.Admin
{
    public class AdminByEmailSpec : Specification<Entities.Admin>
    {
        public AdminByEmailSpec(string email)
        {
            var normalizedEmail = email.ToLower();
            Query.Where(a => a.Email.ToLower() == normalizedEmail);
        }
    }
}