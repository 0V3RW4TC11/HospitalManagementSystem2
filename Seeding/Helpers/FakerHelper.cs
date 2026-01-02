using static Bogus.DataSets.Name;

namespace Seeding.Helpers
{
    internal static class FakerHelper
    {
        public static Gender GetGender(string gender) =>
            gender == "Male" ? Gender.Male : Gender.Female;
    }
}
