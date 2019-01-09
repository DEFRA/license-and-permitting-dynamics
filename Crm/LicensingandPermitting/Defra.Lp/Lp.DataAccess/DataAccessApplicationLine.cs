namespace Lp.DataAccess
{
    using System;
    using Model.EarlyBound;
    using Microsoft.Xrm.Sdk.Query;
    using Model.Crm;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// Data access layer for Application Lines
    /// </summary>
    public static class DataAccessApplicationLine
    {
        /// <summary>
        /// Returns the application line values, for all active application lines linked to the application
        /// </summary>
        /// <param name="service">CRM service</param>
        /// <param name="applicationId">Linked application</param>
        /// <returns>EntityCollection containing active application lines</returns>
        public static EntityCollection GetApplicationLineValues(IOrganizationService service, Guid applicationId)
        {
            // Query active application lines linked to the given application
            QueryExpression query = new QueryExpression(defra_applicationline.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(ApplicationLine.ApplicationId, ConditionOperator.Equal, applicationId),
                        new ConditionExpression(ApplicationLine.State, ConditionOperator.Equal, (int)defra_applicationlineState.Active)
                    }
                },
                ColumnSet = new ColumnSet(ApplicationLine.Value)
            };

            // Go to CRM
            return service.RetrieveMultiple(query);
        }

        /// <summary>
        /// Returns an application line Entity with the state and applicationdi attributes
        /// </summary>
        /// <param name="_Service">CRM org service</param>
        /// <param name="applicationLineId">Application Line Id to return</param>
        /// <returns>Entity record</returns>
        public static Entity GetApplicationLine(IOrganizationService _Service, Guid applicationLineId)
        {
            return _Service.Retrieve(ApplicationLine.EntityLogicalName, applicationLineId, new ColumnSet(ApplicationLine.State, ApplicationLine.ApplicationId));
        }
    }
}