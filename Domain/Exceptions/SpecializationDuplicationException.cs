using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class SpecializationDuplicationException : Exception
    {
        public SpecializationDuplicationException(string? message) : base(message)
        {
        }
    }
}
