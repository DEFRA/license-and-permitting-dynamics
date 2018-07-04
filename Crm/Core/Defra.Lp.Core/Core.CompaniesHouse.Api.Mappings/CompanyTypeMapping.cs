namespace Core.CompaniesHouse.Api.Mappings
{
    using System;
    using Model;
    using Core.Model.Entities;

    /// <summary>
    /// Class is responsible for mapping Companies House Comapny Types to CRM equivalents for accounts
    /// </summary>
    public static class CompanyTypeMapping
    {

        /// <summary>
        /// Maps a Companies house company type to a CRM account organisation type optionset value
        /// </summary>
        /// <param name="companiesHouseCompanyType">Companies house company type</param>
        /// <returns>CRM account organisation type optionset value, throws ArgumentException if invalid mapping</returns>
        public static AccountOrganisationTypes MapToCrmOrganisationType(string companiesHouseCompanyType)
        {
            switch (companiesHouseCompanyType)
            {
                case CompanyTypes.LimitedCompany:
                    return AccountOrganisationTypes.LimitedCompany;

                case CompanyTypes.LimitedLiabilityPartnership:
                    return AccountOrganisationTypes.LimitedLiabilityPartnership;

                default:
                    throw new ArgumentException(@"Unable to map Companies House Company Type '{0}' to CRM Organisation Type Code.", companiesHouseCompanyType);
            }
        }
    }
}
