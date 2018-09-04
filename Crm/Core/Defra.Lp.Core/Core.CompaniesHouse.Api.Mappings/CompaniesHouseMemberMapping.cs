using Core.Helpers.Extensions;

namespace Core.CompaniesHouse.Api.Mappings
{
    using System;
    using System.Globalization;
    using Core.Model.Entities;
    using Defra.Lp.Core.CompaniesHouse;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// Class is responsible for mapping Companies House Officer Role codes to CRM equivalents for accounts and contacts
    /// </summary>
    public static class CompaniesHouseMemberMapping
    {

        /// <summary>
        /// Maps a Companies member to a CRM contact, truncating field as required.
        /// </summary>
        public static Entity MapToContact(CompaniesHouseMember companiesHouseMember, Entity crmContact = null)
        {
            if (crmContact == null)
            {
                crmContact = new Entity(Contact.EntityLogicalName);
            }

            ContactAccountRoleCode crmContactRole = OfficerRoleMapping.MapToCrmContactRoleCode(companiesHouseMember.officer_role);
            crmContact[Contact.FromCompaniesHouseField] = true;
            crmContact[Contact.AccountRoleCodeField] = new OptionSetValue((int)crmContactRole);
            crmContact[Contact.FirstNameField] = companiesHouseMember.firstname.TruncateIfNeeded(Contact.FirstNameFieldLength);
            crmContact[Contact.LastNameField] = companiesHouseMember.lastname.TruncateIfNeeded(Contact.LastNameFieldLength);
            crmContact[Contact.JobTitleField] = companiesHouseMember.occupation.TruncateIfNeeded(Contact.JobTitleFieldLength);

            if (companiesHouseMember.date_of_birth != null)
            {
                crmContact[Contact.DobMonthCompaniesHouseField] = companiesHouseMember.date_of_birth.month;
                crmContact[Contact.DobYearCompaniesHouseField] = companiesHouseMember.date_of_birth.year;
            }

            if (!string.IsNullOrEmpty(companiesHouseMember.resigned_on))
            {
                try
                {
                    crmContact[Contact.ResignedOnCompaniesHouseField] = DateTime.ParseExact(companiesHouseMember.resigned_on,
                        "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    
                }
            }

            return crmContact;
        }

        /// <summary>
        /// Maps a Companies member to a CRM account, truncating field as required.
        /// </summary>
        public static Entity MapToAccount(CompaniesHouseMember companiesHouseMember, Entity crmAccount = null)
        {
            if (crmAccount == null)
            {
                crmAccount = new Entity(Account.EntityLogicalName);
            }

            crmAccount[Account.NameField] = companiesHouseMember.name.TruncateIfNeeded(Account.NameFieldLength);
            crmAccount[Account.CompanyNumberField] = companiesHouseMember.identification?.registration_number.TruncateIfNeeded(Account.CompanyNumberFieldLength);
            return crmAccount;
        }
    }
}
