using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Attendance
{
    public class AttendanceViewModel
    {
        [Display(Name = "Date")]
        public DateTime DateTime { get; set; }

        public string Diagnosis { get; set; }

        public string Remarks { get; set; }

        public string Therapy { get; set; }
    }
}
