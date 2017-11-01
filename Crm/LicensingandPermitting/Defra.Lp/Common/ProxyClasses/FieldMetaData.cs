using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Lp.Common.ProxyClasses
{
    public class FieldMetadata
    {
        public bool ManagedMetadataType { get; internal set; }
        public string Name { get; internal set; }
        public object Value { get; internal set; }
    }
}
