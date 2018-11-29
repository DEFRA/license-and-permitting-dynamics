namespace WastePermits.DataAccess
{
    using System;
    using System.Linq;
    using Lp.Model.Crm;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using WastePermits.Model.Crm;

    public static class DataAccessApplicationPrice
    {
        /// <summary>
        /// Get Price for the given application type and standard rule combination
        /// </summary>
        /// <param name="service"></param>
        /// <param name="applicationType"></param>
        /// <param name="standardRule"></param>
        /// <param name="applicationSubType"></param>
        /// <returns></returns>
        public static Money RetrieveApplicationPrice(this IOrganizationService service, OptionSetValue applicationType, EntityReference standardRule, EntityReference item, EntityReference applicationSubType)
        {
            // Get all the other application lines linked to the same application
            QueryExpression priceQuery = new QueryExpression(ApplicationPrice.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ApplicationPrice.Price),
                Criteria = new FilterExpression()
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression(ApplicationPrice.ApplicationType, ConditionOperator.Equal, applicationType.Value),
                        new ConditionExpression(ApplicationPrice.State, ConditionOperator.Equal, (int)ApplicationLineStates.Active)
                    }
                }
            };

            // Add standard rule filter if available
            if (standardRule != null && standardRule.Id != Guid.Empty)
            {
                priceQuery.Criteria.Conditions.Add(new ConditionExpression(ApplicationPriceWaste.StandardRule, ConditionOperator.Equal, standardRule.Id));
            }

            // Add item filter if available
            if (item != null && item.Id != Guid.Empty)
            {
                priceQuery.Criteria.Conditions.Add(new ConditionExpression(ApplicationPriceWaste.ItemId, ConditionOperator.Equal, item.Id));
            }

            // Add application sub type filter if available
            if (applicationSubType != null && applicationSubType.Id != Guid.Empty)
            {
                priceQuery.Criteria.Conditions.Add(new ConditionExpression(ApplicationPrice.ApplicationSubType, ConditionOperator.Equal, applicationSubType.Id));
            }

            EntityCollection results =  service.RetrieveMultiple(priceQuery);

            if (results == null || results.Entities == null || results.Entities.Count == 0)
            {
                return null;
            }

            if (results.Entities.Count > 1)
            {
               throw new InvalidPluginExecutionException(
                   $"There were {results.TotalRecordCount} Application Price records found for ApplicationType {applicationType.Value} and Standard Rule {standardRule?.Id ?? Guid.Empty}. Expecting One Price only.");
            }


            Entity priceEntity = results.Entities.First();
            if (priceEntity.Contains(ApplicationPrice.Price))
            {
                return priceEntity[ApplicationPrice.Price] as Money;
            }

            return null;
        }
    }
}
