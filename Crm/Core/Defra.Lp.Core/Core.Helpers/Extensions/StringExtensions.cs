using Microsoft.Xrm.Sdk;
using System;
using System.IO;
using System.Text;

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
    }
}