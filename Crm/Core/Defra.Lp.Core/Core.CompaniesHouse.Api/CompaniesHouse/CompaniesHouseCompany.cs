using System;

namespace Defra.Lp.Core.CompaniesHouse
{
    using System.Runtime.Serialization;

    [DataContract]
    public class CompaniesHouseCompany
    {
        [DataMember]
        public string company_name;

        [DataMember]
        public string type;


        [DataMember]
        public CompaniesHouseAddress registered_office_address;

        public CompaniesHouseCompanyStatus company_status;

        [DataMember(Name = "company_status")]
        public string CompanyStatusString
        {
            get { return Enum.GetName(typeof(CompaniesHouseCompanyStatus), this.company_status); }
            // Removes hyphen and coverts string to enum which we can then map to a dynamics option set value
            set { this.company_status = (CompaniesHouseCompanyStatus)Enum.Parse(typeof(CompaniesHouseCompanyStatus), value.Replace("-", string.Empty), true); }
        }
    }
}
