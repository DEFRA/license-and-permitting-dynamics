// -----------------------------------------------
// Helper extensions for string processing
// -----------------------------------------------
namespace Core.Helpers.Extensions
{
    /// <summary>
    /// General string extensions
    /// </summary>
    public static class StringExtensions
    {

        /// <summary>
        /// Truncates a string if required, used to ensure the resulting string
        /// does not exceed a specific lenght.
        /// </summary>
        /// <param name="originalText"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string TruncateIfNeeded(this string originalText, int maxLength)
        {
            // Validate
            if (originalText == null || maxLength < 0 || originalText.Length < maxLength)
            {
                return originalText;
            }

            // Truncate
            return originalText.Substring(0, maxLength);
        }

        /// <summary>
        /// Safely performs a susbtring, prevents exceptions when parameters exceed the text length
        /// </summary>
        /// <param name="text">Text to be processed</param>
        /// <param name="start">Start index - 0 based</param>
        /// <param name="length">Length of the substring</param>
        /// <param name="lefttoright">True if left to right substring, otherwise false</param>
        /// <returns>Processed string result</returns>
        public static string SafeSubstring(this string text, int start, int length, bool lefttoright = true)
        {

            // Length or start are invalid or text is null
            if (length <= 0 || start < 0 || text == null)
            {
                return string.Empty;
            }

            // Right to left?
            if (!lefttoright)
            {
                start = text.Length - length - start;
            }

            // Start is greater than text length?
            if (start >= text.Length)
            {
                return string.Empty;
            }

            // Length beyond text end?
            if (start + length >= text.Length)
            {
                length = text.Length - start;
            }

            return text.Substring(start, length);
        }
    }
}
