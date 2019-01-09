namespace WastePermits.DataAccess
{
    using System;
    using System.Linq;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Model.EarlyBound;

    /// <summary>
    /// Data access layer for CRM queries relating to Pricing
    /// </summary>
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
            QueryExpression priceQuery = new QueryExpression(defra_applicationprice.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(defra_applicationprice.Fields.defra_price),
                Criteria = new FilterExpression()
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression(defra_applicationprice.Fields.defra_applicationtype, ConditionOperator.Equal, applicationType.Value),
                        new ConditionExpression(defra_applicationprice.Fields.StateCode, ConditionOperator.Equal, (int)defra_applicationpriceState.Active)
                    }
                }
            };

            // Add standard rule filter if available
            if (standardRule != null && standardRule.Id != Guid.Empty)
            {
                priceQuery.Criteria.Conditions.Add(new ConditionExpression(defra_applicationprice.Fields.defra_standardruleid, ConditionOperator.Equal, standardRule.Id));
            }

            // Add item filter if available
            if (item != null && item.Id != Guid.Empty)
            {
                priceQuery.Criteria.Conditions.Add(new ConditionExpression(defra_applicationprice.Fields.defra_itemid, ConditionOperator.Equal, item.Id));
            }

            // Add application sub type filter if available
            if (applicationSubType != null && applicationSubType.Id != Guid.Empty)
            {
                priceQuery.Criteria.Conditions.Add(new ConditionExpression(defra_applicationprice.Fields.defra_application_subtype, ConditionOperator.Equal, applicationSubType.Id));
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
            if (priceEntity.Contains(defra_applicationprice.Fields.defra_price))
            {
                return priceEntity[defra_applicationprice.Fields.defra_price] as Money;
            }

            return null;
        }
    }
}
