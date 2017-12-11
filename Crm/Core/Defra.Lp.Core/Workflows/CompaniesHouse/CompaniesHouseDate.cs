namespace Defra.Lp.Core.Workflows.CompaniesHouse
{
    using System.Runtime.Serialization;

    [DataContract]
    public class CompaniesHouseDate
    {
        [DataMember]
        public int year;

        [DataMember]
        public int month;

    }
}