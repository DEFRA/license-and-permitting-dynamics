namespace Defra.Lp.Core.CompaniesHouse
{
    using System.Runtime.Serialization;

    [DataContract]
    public class CompaniesHouseMembersResult
    {
        [DataMember]
        public CompaniesHouseMember[] items { get; set; }
    }
}
