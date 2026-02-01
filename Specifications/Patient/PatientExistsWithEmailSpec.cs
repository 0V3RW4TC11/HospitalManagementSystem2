using Ardalis.Specification;

namespace Specifications.Patient
{
    public class PatientExistsWithEmailSpec : Specification<Domain.Entities.Patient>
    {
        public PatientExistsWithEmailSpec(string email)
        {
            var normalizedEmail = email.ToLower();
            Query.Where(p => p.Email.ToLower() == normalizedEmail);
        }
    }
}
