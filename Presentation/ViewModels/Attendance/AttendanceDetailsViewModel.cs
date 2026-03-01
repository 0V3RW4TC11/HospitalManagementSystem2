using Mapster;
using Presentation.ViewModels.Helpers;
using Services.Dtos.Attendance;
using Services.Dtos.Doctor;
using Services.Dtos.Patient;
using System.ComponentModel.DataAnnotations;

namespace Presentation.ViewModels.Attendance
{
    public class AttendanceDetailsViewModel
    {
        public string ReturnUrl { get; set; }

        [Display(Name = "Doctor")]
        public string DoctorName { get; set; }

        [Display(Name = "Patient")]
        public string PatientName { get; set; }

        [DisplayFormat(DataFormatString = "{0:d/M/yyyy h:mm tt}")]
        public DateTime Date { get; set; }

        public AttendanceViewModel Attendance { get; set; }

        public AttendanceDetailsViewModel()
        {

        }

        public AttendanceDetailsViewModel(
            AttendanceDto attendanceDto, 
            DoctorDto doctorDto, 
            PatientDto patientDto,
            string returnUrl)
        {
            Attendance = attendanceDto.Adapt<AttendanceViewModel>();
            DoctorName = NameFormat.BuildFullName(null, doctorDto.FirstName, doctorDto.LastName);
            PatientName = NameFormat.BuildFullName(patientDto.Title, patientDto.FirstName, patientDto.LastName);
            Date = attendanceDto.DateTime;
            ReturnUrl = returnUrl;
        }
    }
}
