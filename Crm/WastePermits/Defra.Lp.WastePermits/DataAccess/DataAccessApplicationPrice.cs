using Model.Lp.Crm;

namespace DAL
{
    using System;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Model.Waste.Crm;

    public static class DataAccessApplicationPrice
    {

        /// <summary>
        /// Get Price for the given application type and standard rule combination
        /// </summary>
        /// <param name="service"></param>
        /// <param name="applicationType"></param>
        /// <param name="standardRule"></param>
        /// <returns></returns>
        public static Money RetrieveApplicationPrice(this IOrganizationService service, OptionSetValue applicationType, EntityReference standardRule)
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
                        new ConditionExpression(ApplicationPrice.ApplicationType, ConditionOperator.Equal, applicationType),
                        new ConditionExpression(ApplicationPriceWaste.StandardRule, ConditionOperator.Equal, standardRule),
                        new ConditionExpression(ApplicationPrice.State, ConditionOperator.Equal, (int)ApplicationLineStates.Active)
                    }
                }
            };

            EntityCollection results =  service.RetrieveMultiple(priceQuery);

            if (results == null || results.TotalRecordCount == 0)
            {
                return null;
            }

            if (results.TotalRecordCount > 1)
            {
                throw new ApplicationException(string.Format(Messages.MultiplePricesFound, results.TotalRecordCount, applicationType.Value, standardRule.Id));
            }

            Entity priceEntity = results[0];

            Money price = priceEntity[ApplicationPrice.Price] as Money;

            return price;
        }
    }
}
