using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Lp.Core.Workflows.CompaniesHouse
{
    using System.Runtime.Serialization;

    [DataContract]
    public class CompaniesHouseAddress
    {
        [DataMember]
        public string postal_code;

        [DataMember]
        public string address_line_1;

        [DataMember]
        public string locality;

        [DataMember]
        public string address_line_2;

        [DataMember]
        public string region;

        [DataMember]
        public string country;

        [DataMember]
        public string etag;

        [DataMember]
        public string po_box;

        [DataMember]
        public string premises;
    }
}