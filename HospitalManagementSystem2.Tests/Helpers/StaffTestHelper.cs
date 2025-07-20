using HospitalManagementSystem2.Data;

namespace HospitalManagementSystem2.Tests.Helpers;

public static class StaffTestHelper
{
    public static string ExpectedOrgEmail(string firstName, string lastName) =>
        $"{firstName.ToLower()}.{lastName.ToLower()}@{Constants.StaffEmailDomain}";
}