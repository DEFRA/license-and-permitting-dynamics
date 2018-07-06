using Microsoft.Xrm.Sdk;

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
    }
}
