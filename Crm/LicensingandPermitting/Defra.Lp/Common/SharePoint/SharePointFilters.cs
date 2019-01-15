namespace Defra.Lp.Common.SharePoint
{
    /// <summary>
    /// SharePoint filter functions
    /// </summary>
    class SharePointFilters
    {
        /// <summary>
        /// Filters a path string so that it is safe to use in SharePoint
        /// </summary>
        /// <param name="path">SharePoint path string to be filtered</param>
        /// <returns>Filtered string</returns>
        public static string FilterPath(string path)
        {
            return path?.Replace('/', '_');
        }
    }
}
