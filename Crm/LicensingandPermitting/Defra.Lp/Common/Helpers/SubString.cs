using System;

namespace Defra.Lp.Common.Helpers
{
    /// <summary>
    /// Returns a substring of the parameter passed in
    /// </summary>
    public class StringHelper
    {


        private static string SafeSubstring(string text, int start, int length, bool lefttoright)
        {
            if (length <= 0 || start < 0)
            {
                text = String.Empty;
            }
            else
            {
                if (!lefttoright)
                {
                    start = text.Length - length - start;
                }
                text = text.Substring(start, length);
            }
            return text;
        }
    }
}
