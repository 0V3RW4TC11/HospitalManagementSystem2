using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Attendance
{
    public class AttendanceViewModel
    {
        [DisplayName("Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime? DateTime { get; set; } = System.DateTime.Now;

        public string Diagnosis { get; set; }

        public string Remarks { get; set; }

        public string Therapy { get; set; }
    }
}
