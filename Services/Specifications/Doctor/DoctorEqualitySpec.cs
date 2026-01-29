using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications.Doctor
{
    internal class DoctorEqualitySpec : Specification<Domain.Entities.Doctor>
    {
        public DoctorEqualitySpec(string email)
        {
            Query.Where(d => d.Email.ToLower() == email.ToLower());
        }
    }
}
