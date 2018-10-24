using Core.Model.Entities;
namespace Lp.DataAccess
{
    using System;
    using Lp.Model.EarlyBound;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk;
    public static class DataAccessContactDetail
    {
        /// <summary>
        /// Returns a reference to a single Contact Detail record for a given Application
        /// and Type
        /// </summary>
        /// <param name="service">CRM Organisation Service</param>
        /// <param name="applicationId">Application being searched</param>
        /// <param name="contactDetailType">Contact detail type being searched</param>
        /// <returns>Contact Detail Entity Reference</returns>
        public static EntityReference GetContactDetail(IOrganizationService service, Guid? applicationId, int contactDetailType)
        {
            // Set-up Location Query
            QueryExpression qEdefraContactDetail = new QueryExpression(defra_addressdetails.EntityLogicalName) { TopCount = 1 };
            qEdefraContactDetail.Criteria.AddCondition(ContactDetail.State, ConditionOperator.Equal, (int)defra_addressdetailsState.Active);
            // Application Locations?
            qEdefraContactDetail.Criteria.AddCondition(ContactDetail.Application, ConditionOperator.Equal, applicationId);
            qEdefraContactDetail.Criteria.AddCondition(ContactDetail.Type, ConditionOperator.Equal, contactDetailType);
            // Query CRM
            EntityCollection result = service.RetrieveMultiple(qEdefraContactDetail);
            if (result?.Entities != null && result.Entities.Count > 0)
            {
                return result.Entities[0].ToEntityReference();
            }
            return null;
        }
    }
}