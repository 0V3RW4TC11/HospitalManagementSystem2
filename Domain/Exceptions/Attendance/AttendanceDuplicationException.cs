using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.Attendance
{
    public class AttendanceDuplicationException : Exception
    {
        public AttendanceDuplicationException() : base("An similar attendance already exists.")
        {
        }

        public AttendanceDuplicationException(string? message) : base(message)
        {
        }
    }
}
