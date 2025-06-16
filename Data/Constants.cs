namespace HospitalManagementSystem2.Data
{
    public class Constants
    {
        public static class AuthRoles
        {
            public const string Admin = "Admin";
            public const string Doctor = "Doctor";
            public const string Patient = "Patient";
            public static string[] List => [Admin, Doctor, Patient];
        }
    }
}
