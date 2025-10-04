namespace Presentation.Models.Helpers
{
    internal static class NameFormat
    {
        public static string BuildFullName(string? title, string firstName, string? lastName)
        {
            var titlePart = title != null ? title + " " : string.Empty;
            var lastNamePart = lastName != null ? " " + lastName : string.Empty;
            return titlePart + firstName + lastNamePart;
        }
    }
}
