using System.Text.RegularExpressions;

namespace Persistence
{
    internal static class RegexToSqlLikeConverter
    {
        public static string Convert(string regexPattern)
        {
            // Remove regex anchors
            string pattern = regexPattern.Replace("^", "").Replace("$", "");

            // Convert escaped dots back to literal dots
            pattern = pattern.Replace(@"\.", ".");

            // Convert regex \d+ (one or more digits) to SQL % (any characters)
            pattern = Regex.Replace(pattern, @"\\d\+", "%");

            // Convert regex \d (single digit) to SQL _ (single character)
            pattern = Regex.Replace(pattern, @"\\d(?!\+)", "_");

            // Convert regex groups (...) and optional modifier (?) together
            // Pattern like (\d+)? becomes just % (zero or more of anything)
            pattern = Regex.Replace(pattern, @"\(%\)\?", "%");

            // Remove remaining parentheses that weren't part of optional groups
            pattern = pattern.Replace("(", "").Replace(")", "");

            return pattern;
        }
    }
}
