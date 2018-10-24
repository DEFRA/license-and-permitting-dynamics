using System.CodeDom.Compiler;
using Core.Model.Entities;
namespace Lp.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Lp.Model.EarlyBound;
    using Microsoft.Crm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Lp.Model.Crm;
    using Core.Helpers.Extensions;
    using Microsoft.Xrm.Sdk;
    public static class DataAccessContactDetail
    {
        /// <summary>
        /// Returns all the locations and location details linked to the given application
        /// </summary>
        /// <param name="service">CRM Organisation Service</param>
        /// <param name="applicationId">Application being searched</param>
        /// <param name="contactDetailType">Contact detail type being searched</param>
        /// <returns>List of Locations and location details</returns>
        public static EntityReference GetContactDetail(IOrganizationService service, Guid? applicationId, int contactDetailType)
        {
            // Set-up Location Query
            QueryExpression qEdefraLocation = new QueryExpression(defra_addressdetails.EntityLogicalName) { TopCount = 1 };
            qEdefraLocation.Criteria.AddCondition(ContactDetail.State, ConditionOperator.Equal, (int)defra_addressdetailsState.Active);
            // Application Locations?
            qEdefraLocation.Criteria.AddCondition(ContactDetail.Application, ConditionOperator.Equal, applicationId);
            qEdefraLocation.Criteria.AddCondition(ContactDetail.Type, ConditionOperator.Equal, contactDetailType);
            // Query CRM
            EntityCollection result = service.RetrieveMultiple(qEdefraLocation);
            if (result?.Entities != null && result.Entities.Count > 0)
            {
                return result.Entities[0].ToEntityReference();
            }
            return null;
        }
    }
}