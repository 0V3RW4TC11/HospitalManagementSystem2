namespace HospitalManagementSystem2.Data;

public class Constants
{
    // TODO: move to appsettings.json
    public const string StaffEmailDomain = "sjog.org.au";

    public static class AuthRoles
    {
        public const string Admin = "Admin";
        public const string Doctor = "Doctor";
        public const string Patient = "Patient";
        public static string[] List => [Admin, Doctor, Patient];
    }
}