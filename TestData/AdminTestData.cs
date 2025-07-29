using DataTransfer.Admin;
using Domain.Constants;

namespace TestData;

public static class AdminTestData
{
    private const string FirstName = "TestAdminFirstName";
    private const string LastName = "TestAdminLastName";
    private const string Gender = "Male";
    private const string Address = "123 Main St";
    private const string Phone = "123-456-7890";
    private const string Email = "testAdmin@example.com";
    private const string Password = "Password123!";
    private static readonly DateOnly DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1));

    public static AdminCreateDto CreateDto() => new()
    {
        FirstName = FirstName,
        LastName = LastName,
        Gender = Gender,
        Address = Address,
        Phone = Phone,
        Email = Email,
        DateOfBirth = DateOfBirth,
        Password = Password
    };

    public static readonly string ExpectedUsername =
        $"{FirstName.ToLower()}.{LastName.ToLower()}@{DomainNames.Organization}";
}