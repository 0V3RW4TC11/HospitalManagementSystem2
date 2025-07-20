namespace HospitalManagementSystem2.Tests.TestData;

public static class PersonTestData
{
    public const string Title = "TestTitle";
    public const string FirstName = "TestFirstName";
    public const string LastName = "TestLastName";
    public const string Gender = "TestGender";
    public const string Address = "TestAddress";
    public const string Phone = "TestPhoneNumber";
    public const string Email = "TestEmail";
    public const string TestPassword = "TestPassword123!";
    public static readonly DateOnly DateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch);
}