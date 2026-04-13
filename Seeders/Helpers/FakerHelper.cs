using Bogus;
using static Bogus.DataSets.Name;

namespace Seeders.Helpers
{
    internal static class FakerHelper
    {
        public static Gender GetGender(string gender) =>
            gender == "Male" ? Gender.Male : Gender.Female;

        public static string GetStaffEmail(string firstName, string lastName) =>
            $"{firstName}.{lastName}{Faker.GlobalUniqueIndex}@{Constants.DomainNames.Organization}".ToLower();

        public static string PickRandomGender(Faker faker) =>
            faker.PickRandom<Gender>() == Gender.Male ? "Male" : "Female";
    }
}
