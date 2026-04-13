using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class AttendanceDuplicationException : Exception
    {
        public AttendanceDuplicationException(string? message) : base(message)
        {
        }
    }
}
