using System;

namespace Defra.Lp.Common.SharePoint
{

    /// <summary>
    /// Used when calling the SharePoint document relay logic app
    /// to upload the files and folders in SharePoint
    /// </summary>
    internal class DocumentRelayRequest
    {
        public string ApplicationContentType { get; internal set; }
        public string ApplicationNo { get; internal set; }
        public object FileBody { get; internal set; }
        public string FileDescription { get; internal set; }
        public string FileName { get; internal set; }
        public string ListName { get; internal set; }
        public string PermitContentType { get; internal set; }
        public string PermitNo { get; internal set; }
        public string Customer { get; internal set; }
        public string PermitDetails { get; internal set; }
        public string SiteDetails { get; internal set; }
        public string AttachmentId { get; internal set; }
        public string EmailId { get; internal set; }
        public string EmailRegarding { get; internal set; }
        public string EmailFrom { get; internal set; }
        public string EmailTo { get; internal set; }

    }
}