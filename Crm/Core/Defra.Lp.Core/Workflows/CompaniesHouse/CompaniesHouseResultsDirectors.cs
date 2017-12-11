namespace Defra.Lp.Core.Workflows.CompaniesHouse
{
    using System.Runtime.Serialization;

    [DataContract]
    public class CompaniesHouseResultsDirectors
    {
        [DataMember]
        public CompaniesHouseContact[] items { get; set; }
    }
}
