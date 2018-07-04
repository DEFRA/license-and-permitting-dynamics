// ReSharper disable InconsistentNaming
namespace Core.CompaniesHouse.Api.CompaniesHouse
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Model for the Companies House officer identification 
    /// </summary>
    [DataContract]
    public class CompaniesHouseIdentification
    {
        /// <summary>
        /// Company registration number
        /// </summary>
        [DataMember]
        public string registration_number;

        /// <summary>
        /// E.g. UNITED KINGDOM
        /// </summary>
        [DataMember]
        public string place_registered;

        /// <summary>
        /// E.g. eea
        /// </summary>
        [DataMember]
        public string identification_type;
    }
}
 