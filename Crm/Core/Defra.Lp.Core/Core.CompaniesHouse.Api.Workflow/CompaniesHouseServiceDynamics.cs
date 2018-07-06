namespace Defra.Lp.Core.CompaniesHouse.Workflow
{
    using System;
    using global::Core.CompaniesHouse.Api.Model;
    using global::Core.Model.Entities;
    using global::Core.CompaniesHouse.Api.Mappings;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System.Globalization;

    public class CompaniesHouseServiceDynamics : CompaniesHouseService
    {
        private IOrganizationService _crmService { get; set; }

        private ITracingService _crmTracing { get; set; }

        public CompaniesHouseServiceDynamics(string TargetURL, string APIKey, string CompanyRegistrationNumber, IOrganizationService crmService, ITracingService crmTracing)
            : base(TargetURL, APIKey, CompanyRegistrationNumber)
        {
            this._crmService = crmService;
            this._crmTracing = crmTracing;
        }

        public void ValidateCustomer(Entity Account)
        {
            if (Account.Id != Guid.Empty && !string.IsNullOrEmpty(this.CompanyRegistrationNumber))
            {
                _crmTracing.Trace(string.Format("Validate Customer parentAccount {0} with Company Registration Number {1}", Account.Id.ToString(), this.CompanyRegistrationNumber));

                //Retrieve the account
                Account = _crmService.Retrieve(Account.LogicalName, Account.Id, new ColumnSet("name", "defra_validatedwithcompanyhouse", "defra_companyhousestatus"));

                //Get the company details
                this.GetCompany();

                if (this.Company != null && !string.IsNullOrEmpty(this.Company.company_name))
                {
                    //Create or fetch the Contact Details
                    Entity contactDetails = this.GetCustomerRegAddressContactDetails(Account);

                    //Get the company registered address from CH
                    this.GetRegisteredAddress();

                    //Try match the registered address with OS Places
                    if (this.Company.registered_office_address != null)
                        //Create or update the contact details address
                        this.CreateOrUpdateRegisteredAddress(contactDetails);

                    _crmTracing.Trace(string.Format("Company Status = {0}", this.Company.company_status.ToString()));

                    //Update the customer
                    bool accountToBeUpdated = false;
                    if (!Account.Attributes.Contains("name") || (string)Account["name"] != this.Company.company_name)
                    {
                        Account["name"] = this.Company.company_name;
                        accountToBeUpdated = true;
                    }
                    if (!Account.Attributes.Contains("defra_companyhousestatus")
                        || (CompaniesHouseCompanyStatus)((OptionSetValue)Account["defra_companyhousestatus"]).Value != this.Company.company_status)
                    {
                        Account["defra_companyhousestatus"] = new OptionSetValue((int)this.Company.company_status);
                        accountToBeUpdated = true;
                    }
                    if (!(bool)Account["defra_validatedwithcompanyhouse"])
                    {
                        Account["defra_validatedwithcompanyhouse"] = true;
                        accountToBeUpdated = true;
                    }

                    if (accountToBeUpdated)
                        _crmService.Update(Account);

                    if (this.Company.type == CompanyTypes.LimitedCompany)
                    {
                        //Get the company directors
                        this.GetCompanyMembers(true);
                    }
                    else
                    {
                        //Get all company members, for LLP we need designated memebers and members
                        this.GetCompanyMembers(false);
                    }

                    // Create or update the Company Contact Members
                    this.CreateOrUpdateAccountContacts(Account);

                    // Create or update the Company Corporate Members
                    this.CreateOrUpdateMemberAccount(Account);
                }
                else
                {
                    throw new InvalidPluginExecutionException(string.Format("Company with Company Registration Number {0} has not been found", this.CompanyRegistrationNumber));
                }
            }
        }

        private Entity GetCustomerRegAddressContactDetails(Entity Account)
        {
            QueryExpression query = new QueryExpression("defra_addressdetails")
            {
                ColumnSet = new ColumnSet("defra_address"),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression("defra_customer", ConditionOperator.Equal, Account.Id),
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                        //new ConditionExpression("defra_applicationid", ConditionOperator.Null),
                        new ConditionExpression("defra_addresstype", ConditionOperator.Equal, 910400005)
                    }
                }
            };

            EntityCollection results = _crmService.RetrieveMultiple(query);

            if (results.Entities.Count > 0)
                return results.Entities[0];

            Entity newContactDetails = new Entity("defra_addressdetails");
            newContactDetails["defra_addresstype"] = new OptionSetValue(910400005);
            newContactDetails["defra_customer"] = Account.ToEntityReference();
            newContactDetails["defra_name"] = string.Format("Company Registered Address", this.Company.company_name);

            newContactDetails.Id = _crmService.Create(newContactDetails);

            return newContactDetails;
        }

        private void CreateOrUpdateRegisteredAddress(Entity contactDetails)
        {
            /* Example mapping of address CH - OS Places
                "postal_code": "OX26 2QB" - defra_postcode
                "address_line_1": "3d Minton Place" - defra_premises
                "locality": "Bicester" - defra_towntext 
                "address_line_2": "Victoria Road" - defra_street
                "region": "Oxfordshire"
                */

            //The contact details lkp to address is populated. Retrieve, check the address record and create new one if different
            if (contactDetails.Attributes.Contains("defra_address") && contactDetails["defra_address"] != null && contactDetails["defra_address"] is EntityReference && ((EntityReference)contactDetails["defra_address"]).Id != Guid.Empty)
            {
                //Retrieve and check the address
                Entity address = _crmService.Retrieve(((EntityReference)contactDetails["defra_address"]).LogicalName, ((EntityReference)contactDetails["defra_address"]).Id, new ColumnSet("defra_name", "defra_locality", "defra_postcode", "defra_premises", "defra_street", "defra_towntext", "defra_uprn"));

                bool isDifferent = false;
                if (address.Attributes.Contains("defra_postcode") && (string)address["defra_postcode"] != this.Company.registered_office_address.postal_code)
                    isDifferent = true;
                if (address.Attributes.Contains("defra_premises") && (string)address["defra_premises"] != this.Company.registered_office_address.premises)
                    isDifferent = true;
                if (address.Attributes.Contains("defra_street") && (string)address["defra_street"] != this.Company.registered_office_address.address_line_1)
                    isDifferent = true;
                if (address.Attributes.Contains("defra_towntext") && (string)address["defra_towntext"] != this.Company.registered_office_address.locality)
                    isDifferent = true;
                if (address.Attributes.Contains("defra_locality") && (string)address["defra_locality"] != this.Company.registered_office_address.address_line_2)
                    isDifferent = true;

                if (isDifferent)
                {
                    //Update the contact details address
                    UpdateContactDetailsAddress(contactDetails);
                }
            }
            else //The address lkp is empty on contact details
            {
                //Update the contact details address
                UpdateContactDetailsAddress(contactDetails);
            }
        }

        private void UpdateContactDetailsAddress(Entity contactDetails)
        {
            //Only for the address lkp update
            Entity updatedContactDetails = new Entity(contactDetails.LogicalName) { Id = contactDetails.Id };

            //Check for existing address by the companies house registered address
            Entity retrievedCHRegAddress = this.GetDynamicsAddressByTheCompanyHouseRegAddress();

            //If exists use it and update the contact details
            if (retrievedCHRegAddress.Id != Guid.Empty)
                updatedContactDetails["defra_address"] = retrievedCHRegAddress.ToEntityReference();
            else
                //Create new one and update the lkp on contact details
                updatedContactDetails["defra_address"] = CreateNewAddress().ToEntityReference();

            //Update the contact details
            _crmService.Update(updatedContactDetails);
        }

        private Entity GetDynamicsAddressByTheCompanyHouseRegAddress()
        {
            //check for existing address
            QueryExpression query = new QueryExpression("defra_address")
            {
                ColumnSet = new ColumnSet("defra_addressid"),
                Criteria = new FilterExpression()
                {
                    Conditions =
                        {
                            new ConditionExpression("defra_postcode", ConditionOperator.Equal, this.Company.registered_office_address.postal_code),
                            new ConditionExpression("defra_street", ConditionOperator.Equal, this.Company.registered_office_address.address_line_1),
                            new ConditionExpression("defra_towntext", ConditionOperator.Equal, this.Company.registered_office_address.locality),
                            new ConditionExpression("defra_locality", ConditionOperator.Equal, this.Company.registered_office_address.address_line_2)
                        }
                }
            };

            EntityCollection results = _crmService.RetrieveMultiple(query);

            if (results.Entities.Count > 0)
                return results.Entities[0];

            return new Entity("defra_address");
        }

        private Entity CreateNewAddress()
        {
            Entity newAddress = new Entity("defra_address");
            newAddress["defra_fromcompanieshouse"] = true;

            // The premises doesn't come back separately from Companies. We might want to look at parsing to pull out the number
            // but there is also things like house and building name so this not particularly reliable ...
            newAddress["defra_premises"] = this.Company.registered_office_address.premises;
            newAddress["defra_street"] = this.Company.registered_office_address.address_line_1;
            newAddress["defra_locality"] = this.Company.registered_office_address.address_line_2;
            newAddress["defra_towntext"] = this.Company.registered_office_address.locality;
            newAddress["defra_postcode"] = this.Company.registered_office_address.postal_code;
            newAddress["defra_name"] = string.Format("{0}, {1}, {2}", this.Company.registered_office_address.address_line_1, this.Company.registered_office_address.locality, this.Company.registered_office_address.postal_code);

            newAddress.Id = _crmService.Create(newAddress);

            return newAddress;
        }

        /// <summary>
        /// Creates contacts linked to the given account as indicated by Companies House.
        /// Also updates the resigned on date if appropriate
        /// </summary>
        /// <param name="account"></param>
        private void CreateOrUpdateAccountContacts(Entity account)
        {
            _crmTracing.Trace("Creating directors");

            // Validation
            if (this.CompanyMembers == null || this.CompanyMembers.items == null)
            {
                return;
            }

            // Get the existing crm contacts linked to the account, we may need to update them
            var existingCompanyContacts = GetContactMembers(account);

            // iterate through each companies house contact
            foreach (CompaniesHouseMember companiesHouseMember in this.CompanyMembers.items)
            {
                _crmTracing.Trace($"Role is {companiesHouseMember.officer_role}");

                // Validate company house member name is set
                if (string.IsNullOrEmpty(companiesHouseMember.name))
                {
                    _crmTracing.Trace($"Member does not have a name {companiesHouseMember.name}");
                    continue;
                }

                // Validate member is of a type we can process
                if (companiesHouseMember.officer_role != OfficerRoles.Director
                    && companiesHouseMember.officer_role != OfficerRoles.LimitedLiabilityPartnershipdMember
                    && companiesHouseMember.officer_role != OfficerRoles.LimitedLiabilityPartnershipDesignatedMember)
                {
                    _crmTracing.Trace($"Member does not a valid role {companiesHouseMember.name}");
                    continue;
                }

                // Check if the company house member already exists in CRM
                Entity updateExistingContact = null;
                foreach (Entity existingContact in existingCompanyContacts.Entities)
                {
                    if (DoesContactExist(existingContact, companiesHouseMember))
                    {
                        // Existing CRM contact needs to be updated
                        updateExistingContact = existingContact;
                    }
                }

                // Create member accounts that have not resigned
                if (updateExistingContact == null && string.IsNullOrEmpty(companiesHouseMember.resigned_on))
                {
                    // Member has not resigned
                    CreateContactMember(account, companiesHouseMember);
                }

                // Update the resigned date of any existing member
                if (updateExistingContact != null && !string.IsNullOrEmpty(companiesHouseMember.resigned_on))
                {
                    UpdateAccountContact(updateExistingContact, companiesHouseMember);
                }
            }
        }

        /// <summary>
        /// Creates or updates child acocunts 
        /// </summary>
        /// <param name="account"></param>
        private void CreateOrUpdateMemberAccount(Entity account)
        {
            _crmTracing.Trace("Creating member accounts");

            // Validation
            if (this.CompanyMembers == null || this.CompanyMembers.items == null || account == null)
            {
                return;
            }

            // Get the existing crm members linked to the account, we may need to unlink them
            var existingLinks = GetAccountMembers(account.Id);
            _crmTracing.Trace($"GetAccountMembers {account.Id} found {existingLinks.Entities.Count} linked records");

            // iterate through each companies house contact
            foreach (CompaniesHouseMember companiesHouseMember in this.CompanyMembers.items)
            {
                _crmTracing.Trace($"Role is {companiesHouseMember.officer_role}");

                // Validate company house member name is set
                if (string.IsNullOrEmpty(companiesHouseMember.name))
                {
                    continue;
                }

                // Validate member is of a type we can process
                if (companiesHouseMember.officer_role != OfficerRoles.LimitedLiabilityPartnershipCorporateDesignatedMember
                    && companiesHouseMember.officer_role != OfficerRoles.LimitedLiabilityPartnershipdCorporateMember)
                {
                    continue;
                }

                // Check if the company house member already linked to the parent account
                Entity existingRelationship = null;
                foreach (Entity existingLink in existingLinks.Entities)
                {
                    if (IsCorporateMemberAlreadyLinked(existingLink, companiesHouseMember))
                    {
                        // Existing CRM account needs to be unlinked
                        existingRelationship = existingLink;
                    }
                }

                Entity existingAccount = null;
                // Check if the the child account member already exists in CRM at all
                if (existingRelationship == null
                    && companiesHouseMember.identification != null
                    && string.IsNullOrEmpty(companiesHouseMember.resigned_on))
                {
                    existingAccount = this.GetAccount(companiesHouseMember.identification?.registration_number);
                }

                // Create member accounts that have not resigned
                if (existingRelationship == null && string.IsNullOrEmpty(companiesHouseMember.resigned_on))
                {
                    // Member is active, create it, but only if not already exists
                    if (existingAccount == null)
                    {
                        _crmTracing.Trace($"CreateOrUpdateMemberAccount creating account {companiesHouseMember.identification?.registration_number}");
                        Entity memberAccount = this.CreateAccountMember(companiesHouseMember);

                        // And link it to the parent
                        _crmTracing.Trace($"CreateOrUpdateMemberAccount linking new account {companiesHouseMember.identification?.registration_number}");
                        this.LinkAccountMember(account, memberAccount);
                    }
                    else
                    {
                        // And link it to the parent
                        _crmTracing.Trace($"CreateOrUpdateMemberAccount linking existing account {companiesHouseMember.identification?.registration_number}");
                        this.LinkAccountMember(account, existingAccount);
                    }
                }

                // Unlink the resigned date of any existing member
                if (existingRelationship != null && !string.IsNullOrEmpty(companiesHouseMember.resigned_on))
                {
                    // TODO - Unlink resigned corporate members
                }
            }
        }

        /// <summary>
        /// Updates the contact resigned date
        /// </summary>
        /// <param name="updateExistingContact"></param>
        /// <param name="companiesHouseMember"></param>
        private void UpdateAccountContact(Entity updateExistingContact, CompaniesHouseMember companiesHouseMember)
        {
            updateExistingContact["defra_fromcompanieshouse"] = true;
            try
            {
                updateExistingContact["defra_resignedoncompanieshouse"] = DateTime.ParseExact(companiesHouseMember.resigned_on,
                    "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                _crmTracing.Trace($"Resigned Date {companiesHouseMember.resigned_on} is not in the correct format.");
            }
            _crmService.Update(updateExistingContact);
        }

        /// <summary>
        /// Creates a new companies house contact linked to the account given in the parameters
        /// </summary>
        /// <param name="account">Parent account</param>
        /// <param name="companiesHouseMember"></param>
        private void CreateContactMember(Entity account, CompaniesHouseMember companiesHouseMember)
        {
            // Map Companies House member to CRM
            Entity newContact = CompaniesHouseMemberMapping.MapToContact(companiesHouseMember);

            // Link to the parent account
            newContact[Contact.ParentCustomerIdField] = account.ToEntityReference();

            // Create in CRM
            _crmService.Create(newContact);
        }

        /// <summary>
        /// Creates a new companies house acccount from a mcompanies house member
        /// </summary>
        /// <param name="parentAccount"></param>
        /// <param name="companiesHouseMember"></param>
        private Entity CreateAccountMember(CompaniesHouseMember companiesHouseMember)
        {
            // Map Companies house meber to an account and create it in CRM
            Entity newAccount = CompaniesHouseMemberMapping.MapToAccount(companiesHouseMember);
            newAccount.Id = _crmService.Create(newAccount);
            return newAccount;
        }

        /// <summary>
        /// Creates an N:N relationship between parent and member account
        /// </summary>
        /// <param name="parentAccount"></param>
        /// <param name="memberAccount"></param>
        private void LinkAccountMember(Entity parentAccount, Entity memberAccount)
        {
            Relationship relation = new Relationship(Account.ParentChildAccountManyToManyRelationship)
            {
                PrimaryEntityRole = EntityRole.Referencing
            };

            _crmService.Associate(
                    Account.EntityLogicalName,
                    parentAccount.Id,
                    relation,
                    new EntityReferenceCollection { memberAccount.ToEntityReference() });
        }

        // Checks if a companies house member already exists
        private static bool DoesContactExist(Entity existingContact, CompaniesHouseMember companiesHouseMember)
        {
            string dynStr = string.Empty;
            if (existingContact.Attributes.Contains("firstname") && existingContact.Attributes["firstname"] != null)
                dynStr += (string)existingContact["firstname"];
            if (existingContact.Attributes.Contains("lastname") && existingContact.Attributes["lastname"] != null)
                dynStr += (string)existingContact["lastname"];
            if (existingContact.Attributes.Contains("defra_dobmonthcompanieshouse") &&
                existingContact.Attributes["defra_dobmonthcompanieshouse"] != null)
                dynStr += ((int)existingContact["defra_dobmonthcompanieshouse"]).ToString();
            if (existingContact.Attributes.Contains("defra_dobyearcompanieshouse") &&
                existingContact.Attributes["defra_dobyearcompanieshouse"] != null)
                dynStr += ((int)existingContact["defra_dobyearcompanieshouse"]).ToString();

            if (string.Equals(dynStr,
                (companiesHouseMember.firstname + companiesHouseMember.lastname + companiesHouseMember.date_of_birth.month +
                 companiesHouseMember.date_of_birth.year), StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }

        // Checks if a companies house member already exists
        private bool IsCorporateMemberAlreadyLinked(Entity existingLink, CompaniesHouseMember companiesHouseMember)
        {
            _crmTracing.Trace($"IsCorporateMemberAlreadyLinked {companiesHouseMember}");
            if (existingLink.Attributes.Contains(Account.ParentChildAccountManyToManyRelationshipAlias + "." + Account.CompanyNumberField)
                && companiesHouseMember.identification != null
                && existingLink.Attributes[Account.ParentChildAccountManyToManyRelationshipAlias + "." + Account.CompanyNumberField].ToString() == companiesHouseMember.identification.registration_number)
            {
                // Matching account with the same company reg number exists
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves all contacts linked to the account given
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        private EntityCollection GetContactMembers(Entity Account)
        {
            //Retrieve the account directors from Dynamics
            QueryExpression query = new QueryExpression("contact")
            {
                ColumnSet = new ColumnSet("fullname", "firstname", "lastname",
                    "defra_dobmonthcompanieshouse", "defra_dobyearcompanieshouse",
                    "defra_resignedoncompanieshouse"),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                        new ConditionExpression("parentcustomerid", ConditionOperator.Equal, Account.Id),
                        new ConditionExpression("fullname", ConditionOperator.NotNull)
                    }
                }
            };
            return _crmService.RetrieveMultiple(query);
        }

        /// <summary>
        /// Retrieves all contacts linked to the account given
        /// </summary>
        /// <param name="parentAccountId"></param>
        /// <returns></returns>
        private EntityCollection GetAccountMembers(Guid parentAccountId)
        {
            // Instantiate QueryExpression QEdefra_parent_child_account_relationship
            var query = new QueryExpression(Account.ParentChildAccountManyToManyRelationship) {TopCount = 50};

            // Define filter QEdefra_parent_child_account_relationship.Criteria
            query.Criteria.AddCondition(Account.ParentChildAccountManyToManyRelationshipFromField, ConditionOperator.Equal, parentAccountId);

            // Add link-entity QEdefra_parent_child_account_relationship_account
            var link = query.AddLink(Account.EntityLogicalName, Account.ParentChildAccountManyToManyRelationshipToField, Account.AccountIdField);
            link.EntityAlias = Account.ParentChildAccountManyToManyRelationshipAlias;

            // Add columns to QEdefra_parent_child_account_relationship_account.Columns
            link.Columns.AddColumns(Account.AccountIdField, Account.NameField, Account.CompanyNumberField);
            _crmTracing.Trace($"GetAccountMembers query {query} for parent {parentAccountId}");
            return _crmService.RetrieveMultiple(query);
        }


        /// <summary>
        /// Gets the first contact matching the given companies house number
        /// </summary>
        /// <param name="companyHouseNumber">The companies house number to look up in CRM</param>
        /// <returns>Matching account or null</returns>
        private Entity GetAccount(string companyHouseNumber)
        {
            //Prepare the query
            QueryExpression query = new QueryExpression(Account.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(Account.NameField),
                TopCount = 1,
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(Account.StateField, ConditionOperator.Equal, 0),
                        new ConditionExpression(Account.CompanyNumberField, ConditionOperator.Equal, companyHouseNumber)
                    }
                }
            };

            // Get the first record found 
            EntityCollection accounts = _crmService.RetrieveMultiple(query);

            if (accounts?.Entities != null && accounts.Entities.Count > 0)
            {
                _crmTracing.Trace($"GetAccount {companyHouseNumber} found {accounts.Entities.Count} matching records");
                return accounts.Entities[0];
            }

            _crmTracing.Trace($"GetAccount {companyHouseNumber} found 0 matching records");
            // No account found
            return null;
        }
    }
}