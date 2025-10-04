using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Attendance
{
    public class AttendanceViewModel
    {
        public string Diagnosis { get; set; }

        public string Remarks { get; set; }

        public string Therapy { get; set; }
    }
}
