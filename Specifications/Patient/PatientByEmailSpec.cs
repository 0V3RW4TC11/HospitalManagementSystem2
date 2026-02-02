using Ardalis.Specification;

namespace Specifications.Patient
{
    public class PatientByEmailSpec : Specification<Domain.Entities.Patient>
    {
        public PatientByEmailSpec(string email)
        {
            var normalizedEmail = email.ToLower();
            Query.Where(p => p.Email.ToLower() == normalizedEmail);
        }
    }
}