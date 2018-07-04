namespace Core.CompaniesHouse.Api.Mappings
{
    using System;
    using Model;
    using Core.Model.Entities;

    /// <summary>
    /// Class is responsible for mapping Companies House Officer Role codes to CRM equivalents for accounts and contacts
    /// </summary>
    public static class OfficerRoleMapping
    {

        /// <summary>
        /// Maps a Companies house officer_role to a CRM contact accountrolecode optionset value
        /// </summary>
        /// <param name="companiesHouseOfficerRole">Companies house officer_role</param>
        /// <returns>CRM contact accountrolecode optionset value, throws ArgumentException if invalid mapping</returns>
        public static  ContactAccountRoleCode MapToCrmContactRoleCode(string companiesHouseOfficerRole)
        {
            switch (companiesHouseOfficerRole)
            {
                case OfficerRoles.Director:
                    return ContactAccountRoleCode.Director;

                case OfficerRoles.LimitedLiabilityPartnershipDesignatedMember:
                    return ContactAccountRoleCode.LlpDesignatedMember;

                case OfficerRoles.LimitedLiabilityPartnershipdMember:
                    return ContactAccountRoleCode.LlpMember;

                default:
                    throw new ArgumentException(@"Unable to map Companies House Officer Role '{0}' to CRM Contact Role Code.", companiesHouseOfficerRole);
            }
        }
    }
}
