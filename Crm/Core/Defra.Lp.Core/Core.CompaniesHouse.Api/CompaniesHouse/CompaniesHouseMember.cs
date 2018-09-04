// ReSharper disable InconsistentNaming

using System.Text;
using Core.CompaniesHouse.Api.CompaniesHouse;

namespace Defra.Lp.Core.CompaniesHouse
{
    using System.Runtime.Serialization;

    [DataContract]
    public class CompaniesHouseMember
    {
        [DataMember]
        public string country_of_residence;

        [DataMember]
        public string name;

        [DataMember]
        public string officer_role;

        [DataMember]
        public string appointed_on;

        [DataMember]
        public string resigned_on;

        [DataMember]
        public CompaniesHouseDate date_of_birth;

        [DataMember]
        public string occupation;

        [DataMember]
        public string nationality;

        public string firstname
        {
            get
            {
                string[] names = this.name.Split(',');

                if (names.Length == 2)
                {
                    return names[1].Trim();
                }

                if (names.Length == 3)
                {
                    return $"{names[2].Trim()} {names[1].Trim()}";
                }

                if (names.Length > 3)
                {
                    return $"{names[names.Length-1].Trim()} {names[names.Length - 2].Trim()} {names[names.Length - 3].Trim()}";
                }
                return string.Empty;
            }
        }

        public string lastname
        {
            get
            {
                // Return the last name
                string[] names = this.name.Split(',');
                return names[0];
            }
        }

        /// <summary>
        /// Contains the corporate member company number
        /// </summary>
        [DataMember]
        public CompaniesHouseIdentification identification;
    }
}
 