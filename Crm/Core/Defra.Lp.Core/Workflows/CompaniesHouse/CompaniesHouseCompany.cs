using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Lp.Core.Workflows.CompaniesHouse
{
    using System.Runtime.Serialization;

    [DataContract]
    public class CompaniesHouseCompany
    {
        [DataMember]
        public string company_name;

        [DataMember]
        public CompaniesHouseAddress registered_office_address;
    }
}
