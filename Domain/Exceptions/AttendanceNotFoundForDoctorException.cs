namespace Domain.Exceptions;

public class AttendanceNotFoundForDoctorException : NotFoundException
{
    public AttendanceNotFoundForDoctorException(string id) : base($"Attendance not found for Doctor Id: {id}.")
    {
    }
}