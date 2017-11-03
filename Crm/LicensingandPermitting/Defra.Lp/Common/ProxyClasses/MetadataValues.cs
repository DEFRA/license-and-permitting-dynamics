using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Lp.Common.ProxyClasses
{
    public class MetadataValues
    {
        public MetadataValues()
        {
            this.Fields = new List<FieldMetadata>();
        }

        public List<FieldMetadata> Fields { get; internal set; }
        public string ListName { get; internal set; }
    }
}
