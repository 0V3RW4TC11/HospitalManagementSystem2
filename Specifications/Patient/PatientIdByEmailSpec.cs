using Ardalis.Specification;

namespace Specifications.Patient
{
    public class PatientIdByEmailSpec : SingleResultSpecification<Entities.Patient, Guid>
    {
        public PatientIdByEmailSpec(string email)
        {
            var normalizedEmail = email.ToLower();

            Query
                .Where(p => p.Email.ToLower() == normalizedEmail)
                .Select(p => p.Id);
        }
    }
}