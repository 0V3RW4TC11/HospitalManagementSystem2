using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class AdminDuplicationException : Exception
    {
        public AdminDuplicationException(string? message) : base(message)
        {
        }
    }
}
