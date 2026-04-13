using Mapster;
using Presentation.Models.Helpers;
using Services.Dtos.Attendance;
using Services.Dtos.Patient;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Attendance
{
    public class AttendanceCreateViewModel
    {
        public Guid DoctorId { get; set; }

        public Guid PatientId { get; set; }

        [Display(Name = "Patient")]
        public string PatientName { get; set; }

        public AttendanceViewModel Attendance { get; set; }

        public AttendanceCreateViewModel()
        {
            
        }

        public AttendanceCreateViewModel(Guid doctorId, PatientDto dto)
        {
            DoctorId = doctorId;
            PatientId = dto.Id;
            PatientName = NameFormat.BuildFullName(dto.Title, dto.FirstName, dto.LastName);
            Attendance = new AttendanceViewModel();
        }

        public AttendanceCreateDto Dto
        {
            get
            {
                var dto = Attendance.Adapt<AttendanceCreateDto>();
                dto.DoctorId = DoctorId;
                dto.PatientId = PatientId;
                return dto;
            }
        }
    }
}
