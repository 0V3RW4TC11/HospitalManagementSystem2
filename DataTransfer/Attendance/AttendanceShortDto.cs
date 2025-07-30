namespace DataTransfer.Attendance;

public class AttendanceShortDto
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }

    public DateTime DateTime { get; set; }
}