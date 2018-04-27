namespace Defra.Lp.Core.CompaniesHouse
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