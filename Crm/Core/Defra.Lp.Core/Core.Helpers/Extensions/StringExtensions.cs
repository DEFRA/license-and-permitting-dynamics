// -----------------------------------------------
// Helper extensions for string processing
// -----------------------------------------------
namespace Core.Helpers.Extensions
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

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
		
		/// <summary>
        /// Base64 encodes a string. 
        /// </summary>
        /// <param name="originalText"></param>
        /// <returns>Base64 encoded string </returns>
		public static string Base64Encode(this string originalText) {
		    var plainTextBytes = Encoding.UTF8.GetBytes(originalText);
		    return Convert.ToBase64String(plainTextBytes);
		}

        /// <summary>
        /// Adds timestamp to the filename
        /// </summary>
        /// <param name="fileName">Original filename</param>
        /// <param name="date">Date to be added to file name</param>
        /// <returns>Orignal filename + timestamp + ext</returns>
        public static string AppendTimeStamp(this string fileName, DateTime date)
        {
            return string.Concat(
                Path.GetFileNameWithoutExtension(fileName),
                "_",
                date.ToString("yyyyMMddHHmmssfff"),
                Path.GetExtension(fileName)
                );
        }

        /// <summary>
        /// Removes characters that are not allowed to be used in filenames in 
        /// SharePoint. Also restricts length to 75 characters.
        /// </summary>
        /// <param name="fileName">Original filename</param>
        /// <returns>Filename without illegal chars</returns>
        public static string SpRemoveIllegalChars(this string fileName)
        {
            fileName = new Regex(@"\.(?!(\w{3,4}$))").Replace(fileName, "");
            var forbiddenChars = @"#%&*:<>?/{|}~".ToCharArray();
            fileName = new string(fileName.Where(c => !forbiddenChars.Contains(c)).ToArray());
            if (fileName.Length >= 76)
            {
                fileName = fileName.Remove(75);
            }
            return fileName.Trim();
        }
    }
}