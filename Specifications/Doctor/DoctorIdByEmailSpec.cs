using Ardalis.Specification;

namespace Specifications.Doctor
{
    public class DoctorIdByEmailSpec : SingleResultSpecification<Entities.Doctor, Guid>
    {
        public DoctorIdByEmailSpec(string email)
        {
            var normalizedEmail = email.ToLower();

            Query
                .Where(d => d.Email == normalizedEmail)
                .Select(d => d.Id);
        }
    }
}