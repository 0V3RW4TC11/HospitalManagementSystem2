using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.Doctor
{
    public class DoctorNotFoundException() : NotFoundException("Doctor not found.")
    {
    }
}
