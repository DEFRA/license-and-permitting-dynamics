// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <summary>Class is responsible for relinking and copying entities from a source parent to a target parent</summary>

using System;
using System.Linq;
using Lp.Model.EarlyBound;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Lp.DataAccess
{
    /// <summary>
    /// Class is in charge of linking/copying child entities from a source parent to a target parent
    /// </summary>
    public class DataAccessContact
    {
        private IOrganizationService Service { get; }

        private ITracingService TracingService { get; }


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
                        new ConditionExpression("accountid", ConditionOperator.Equal, accountId),
                        new ConditionExpression("accountrolecode", ConditionOperator.Equal, (int)accountRoleCode),
                        new ConditionExpression("statecode", ConditionOperator.Equal, (int)ContactState.Active)
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
