namespace Defra.Lp.Core.CompaniesHouse
{
    using System.Runtime.Serialization;

    [DataContract]
    public class CompaniesHouseResultsDirectors
    {
        [DataMember]
        public CompaniesHouseContact[] items { get; set; }
    }
}
