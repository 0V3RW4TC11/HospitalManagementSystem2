namespace Domain.Exceptions;

public class AttendanceNotFoundForPatientException : NotFoundException
{
    public AttendanceNotFoundForPatientException(string id) : base($"Attendance not found for Patient Id: {id}.")
    {
    }
}