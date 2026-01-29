using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.Attendance
{
    public class AttendanceNotFoundException() : NotFoundException("Attendance not found.")
    {
    }
}
