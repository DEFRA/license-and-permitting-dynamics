namespace Defra.Lp.Common.SharePoint
{

    /// <summary>
    /// Model used to communicate with Logic App that updates SharePoint metadata
    /// </summary>
    internal class MetaDataRequest
    {
        public string ApplicationNo { get; internal set; }
        public string Customer { get; internal set; }
        public string EawmlNo { get; internal set; }
        public string ListName { get; internal set; }
        public string PermitDetails { get; internal set; }
        public string PermitNo { get; internal set; }
        public string SiteDetails { get; internal set; }
        public string UpdateType { get; internal set; }
    }
}