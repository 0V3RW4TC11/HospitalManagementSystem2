using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications.Patient
{
    internal class PatientEqualitySpec : Specification<Domain.Entities.Patient>
    {
        public PatientEqualitySpec(string email)
        {
            Query.Where(p => p.Email.ToLower() == email.ToLower());
        }
    }
}
