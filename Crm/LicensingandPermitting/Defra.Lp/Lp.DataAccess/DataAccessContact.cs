// Data access layer in charge of returning and managing contacts and contact details

namespace Lp.DataAccess
{
    using System;
    using System.Linq;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Model.EarlyBound;

    /// <summary>
    /// Main data access layer class for contact and contact detail related CRM transactions
    /// </summary>
    public class DataAccessContact
    {
        /// <summary>
        /// CRM Organisation Service
        /// </summary>
        private IOrganizationService Service { get; }

        /// <summary>
        /// Plugin and Code Activity Tracing service
        /// </summary>
        private ITracingService TracingService { get; }


        /// <summary>
        /// Constructor saves the CRm services
        /// </summary>
        /// <param name="service">CRM Organisation Service</param>
        /// <param name="tracingService">CRM Trancing Service</param>
        public DataAccessContact(IOrganizationService service, ITracingService tracingService)
        {
            this.Service = service;
            this.TracingService = tracingService;
        }
        
        /// <summary>
        /// Queries CRM for all contacts for a given account and contact role
        /// </summary>
        /// <param name="accountId">Linked contact account</param>
        /// <param name="accountRoleCode">The contact role to filter by</param>
        /// <returns>List of contact entity references matching the requirements</returns>
        public EntityReference[] GetAccountContacts(Guid accountId, Contact_AccountRoleCode accountRoleCode)
        {
            // Prepare to query all active contacts of the given role and account
            QueryExpression query = new QueryExpression(Contact.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(Core.Model.Entities.Contact.AccountId, ConditionOperator.Equal, accountId),
                        new ConditionExpression(Core.Model.Entities.Contact.AccountRoleCodeField, ConditionOperator.Equal, (int)accountRoleCode),
                        new ConditionExpression(Core.Model.Entities.Contact.State, ConditionOperator.Equal, (int)ContactState.Active)
                    }
                }
            };

            // Query CRM
            EntityCollection contacts = this.Service.RetrieveMultiple(query);

            // Process Results
            if (contacts?.Entities == null)
            {
                return null;
            }

            return contacts.Entities.Select(e => e.ToEntityReference()).ToArray();
        }
    }
}
