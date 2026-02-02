using Ardalis.Specification;

namespace Specifications.Doctor
{
    public class DoctorByEmailSpec : Specification<Domain.Entities.Doctor>
    {
        public DoctorByEmailSpec(string email)
        {
            var normalizedEmail = email.ToLower();
            Query.Where(d => d.Email.ToLower() == normalizedEmail);
        }
    }
}