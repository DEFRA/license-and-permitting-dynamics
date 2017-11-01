using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Lp.Common.ProxyClasses
{
    public class MoveSharePointResult
    {
        public string FileName { get; set; }
        public string FileURL { get; set; }
        public string ActivityName { get; set; }
        public string ActivityId { get; set; }
        public string AttachmentId { get; set; }
        public string AttachmentType { get; set; }
        public string DocumentId { get; set; }
        public string SharePointId { get; set; }
    }
}