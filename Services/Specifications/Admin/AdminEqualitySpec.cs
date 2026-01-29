using Ardalis.Specification;

namespace Services.Specifications.Admin
{
    internal class AdminEqualitySpec : Specification<Domain.Entities.Admin>
    {
        public AdminEqualitySpec(string email)
        {
            Query.Where(a => a.Email.ToLower() == email.ToLower());
        }
    }
}
