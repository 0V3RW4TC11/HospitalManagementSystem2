using DataTransfer.Admin;
using Domain.Constants;

namespace TestData;

public static class AdminData
{
    public static readonly AdminCreateDto CreateDto = new()
    {
        FirstName = "TestAdminFirstName",
        LastName = "TestAdminLastName",
        Gender = "Male",
        Address = "123 Main St",
        Phone = "123-456-7890",
        Email = "testAdmin@example.com",
        DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
        Password = "Password123!"
    };

    public static readonly string ExpectedUsername =
        $"{CreateDto.FirstName.ToLower()}.{CreateDto.LastName!.ToLower()}@{DomainNames.Organization}";
}