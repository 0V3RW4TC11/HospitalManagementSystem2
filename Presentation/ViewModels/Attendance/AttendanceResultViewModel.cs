using Presentation.ViewModels.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Presentation.ViewModels.Attendance
{
    public class AttendanceResultViewModel
    {
        public Guid AttendanceId { get; set; }

        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:d/M/yyyy h:mm tt}")]
        public DateTime Date { get; set; }

        public AttendanceResultViewModel(
            Guid attendanceId,
            DateTime date,
            string nameTitle,
            string nameFirst,
            string nameLast)
        {
            AttendanceId = attendanceId;
            Name = NameFormat.BuildFullName(nameTitle, nameFirst, nameLast);
            Date = date;
        }
    }
}
