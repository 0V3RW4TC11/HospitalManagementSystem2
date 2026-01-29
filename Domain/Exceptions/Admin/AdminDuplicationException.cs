using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.Admin
{
    public class AdminDuplicationException : Exception
    {
        public AdminDuplicationException() : base("Email is in use by another " + Constants.AuthRoles.Admin)
        {
        }
    }
}
