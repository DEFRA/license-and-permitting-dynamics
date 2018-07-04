namespace Core.CompaniesHouse.Api.Model
{
    /// <summary>
    ///  Companies House officer roles used when filtering members of a company
    /// </summary>
    public class OfficerRoles
    {

        /// <summary>
        /// Company Director
        /// </summary>
        public const string Director = "director";

        /// <summary>
        /// LLP Member
        /// </summary>
        public const string LimitedLiabilityPartnershipDesignatedMember = "llp-designated-member";
        
        /// <summary>
        /// LLP Member
        /// </summary>
        public const string LimitedLiabilityPartnershipdMember = "llp-member";

        /// <summary>
        /// Corporate LLP Member 
        /// </summary>
        public const string LimitedLiabilityPartnershipdCorporateMember = "corporate-llp-member";

        /// <summary>
        ///  Corporate LLP Designated Member
        /// </summary>
        public const string LimitedLiabilityPartnershipdCorporateDesignatedMember = "corporate-llp-designated-member";
        
            
    }
}
