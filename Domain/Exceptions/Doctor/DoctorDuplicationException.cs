using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.Doctor
{
    public class DoctorDuplicationException : Exception
    {
        public DoctorDuplicationException() : base("Email is in use by another " + Constants.AuthRoles.Doctor)
        {
        }
    }
}
