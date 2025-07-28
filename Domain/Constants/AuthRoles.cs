namespace Domain.Constants;

public static class AuthRoles
{
    public const string Admin = "Admin";
    
    public const string Doctor = "Doctor";
    
    public const string Patient = "Patient";
    
    public static string[] AsList() => [Admin, Doctor, Patient];
}