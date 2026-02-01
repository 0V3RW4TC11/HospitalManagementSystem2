using Ardalis.Specification;

namespace Specifications.Patient
{
    public class GetPatientIdByEmailSpec : SingleResultSpecification<Domain.Entities.Patient, Guid>
    {
        public GetPatientIdByEmailSpec(string email)
        {
            var normalizedEmail = email.ToLower();

            Query
                .Where(p => p.Email.ToLower() == normalizedEmail)
                .Select(p => p.Id);
        }
    }
}
