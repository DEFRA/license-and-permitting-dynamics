using System;

namespace Defra.Lp.Core.Workflows.CompaniesHouse
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using OSPlaces;
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

        public void ValidateCustomer(Entity Account, OSPlacesService osService)
        {
            if (Account.Id != Guid.Empty && !string.IsNullOrEmpty(this.CompanyRegistrationNumber))
            {
                _crmTracing.Trace(string.Format("Validate Customer Account {0} with Company Registration Number {1}", Account.Id.ToString(), this.CompanyRegistrationNumber));

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

                    //Try match address with OS Places
                    //OSPlacesAddress osAddress = osService.MatchCompaniesHouseAddress(this.Company.registered_office_address);

                    //Get the company directords
                    this.GetCompanyDirectors();

                    //Create the Company Directors
                    this.CreateDirectors(Account);
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
                if (address.Attributes.Contains("defra_premises") && (string)address["defra_premises"] != this.Company.registered_office_address.address_line_1)
                    isDifferent = true;
                if (address.Attributes.Contains("defra_towntext") && (string)address["defra_towntext"] != this.Company.registered_office_address.locality)
                    isDifferent = true;
                if (address.Attributes.Contains("defra_street") && (string)address["defra_street"] != this.Company.registered_office_address.address_line_2)
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
                            new ConditionExpression("defra_premises", ConditionOperator.Equal, this.Company.registered_office_address.address_line_1),
                            new ConditionExpression("defra_towntext", ConditionOperator.Equal, this.Company.registered_office_address.locality),
                            new ConditionExpression("defra_street", ConditionOperator.Equal, this.Company.registered_office_address.address_line_2)
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
            newAddress["defra_postcode"] = this.Company.registered_office_address.postal_code;
            newAddress["defra_premises"] = this.Company.registered_office_address.address_line_1;
            newAddress["defra_towntext"] = this.Company.registered_office_address.locality;
            newAddress["defra_street"] = this.Company.registered_office_address.address_line_2;
            newAddress["defra_name"] = string.Format("{0}, {1}, {2}, {3}", this.Company.registered_office_address.address_line_1, this.Company.registered_office_address.address_line_2, this.Company.registered_office_address.locality, this.Company.registered_office_address.postal_code);

            newAddress.Id = _crmService.Create(newAddress);

            return newAddress;
        }

        private void CreateDirectors(Entity Account)
        {
            if (this.Directors != null && this.Directors.items != null)
            {
                _crmTracing.Trace("Creating directors");

                //Retrieve the account directors from Dynamics
                QueryExpression query = new QueryExpression("contact")
                {
                    ColumnSet = new ColumnSet("fullname", "firstname", "lastname", "defra_dobmonthcompanieshouse", "defra_dobyearcompanieshouse"),
                    Criteria = new FilterExpression()
                    {
                        Conditions =
                        {
                            new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                            new ConditionExpression("parentcustomerid", ConditionOperator.Equal, Account.Id),
                            new ConditionExpression("accountrolecode", ConditionOperator.Equal, 910400000),
                            new ConditionExpression("fullname", ConditionOperator.NotNull)
                        }
                    }
                };

                EntityCollection results = _crmService.RetrieveMultiple(query);

                //Create the non existing once
                foreach (CompaniesHouseContact director in this.Directors.items)
                {
                    // Only interested in directors. Filtering here because the filter on the Companys House API
                    // doesn't seem to work and returned all officers.
                    _crmTracing.Trace(string.Format("Role is {0}", director.officer_role));

                    if (!string.IsNullOrEmpty(director.name) && director.officer_role == "director")
                    {
                        bool exists = false;

                        foreach (Entity dynamicsDirector in results.Entities)
                        {
                            string dynStr = string.Empty;
                            if (dynamicsDirector.Attributes.Contains("firstname") && dynamicsDirector.Attributes["firstname"] != null)
                                dynStr += (string)dynamicsDirector["firstname"];
                            if (dynamicsDirector.Attributes.Contains("lastname") && dynamicsDirector.Attributes["lastname"] != null)
                                dynStr += (string)dynamicsDirector["lastname"];
                            if (dynamicsDirector.Attributes.Contains("defra_dobmonthcompanieshouse") && dynamicsDirector.Attributes["defra_dobmonthcompanieshouse"] != null)
                                dynStr += ((int)dynamicsDirector["defra_dobmonthcompanieshouse"]).ToString();
                            if (dynamicsDirector.Attributes.Contains("defra_dobyearcompanieshouse") && dynamicsDirector.Attributes["defra_dobyearcompanieshouse"] != null)
                                dynStr += ((int)dynamicsDirector["defra_dobyearcompanieshouse"]).ToString();

                            if (dynStr.ToLower() == (director.firstname + director.lastname + director.date_of_birth.month.ToString() + director.date_of_birth.year.ToString()).ToLower())
                                exists = true;
                        }

                        if (!exists)
                        {
                            Entity newDirector = new Entity("contact");
                            newDirector["defra_fromcompanieshouse"] = true;
                            newDirector["parentcustomerid"] = Account.ToEntityReference();
                            newDirector["accountrolecode"] = new OptionSetValue(910400000);
                            newDirector["firstname"] = director.firstname;
                            newDirector["lastname"] = director.lastname;
                            //newDirector["fullname"] = director.name;
                            newDirector["jobtitle"] = director.occupation;
                            if (director.date_of_birth != null)
                            {
                                newDirector["defra_dobmonthcompanieshouse"] = director.date_of_birth.month;
                                newDirector["defra_dobyearcompanieshouse"] = director.date_of_birth.year;
                            }
                            if (!string.IsNullOrEmpty(director.resigned_on))
                            {
                                try
                                {
                                    newDirector["defra_resignedoncompanieshouse"] = DateTime.ParseExact(director.resigned_on, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                }
                                catch (FormatException)
                                {
                                    _crmTracing.Trace(string.Format("Resigned Date {0} is not in the correct format.", director.resigned_on));
                                }
                            }
                            newDirector.Id = _crmService.Create(newDirector);
                        }
                    }
                }
            }
        }
    }
}