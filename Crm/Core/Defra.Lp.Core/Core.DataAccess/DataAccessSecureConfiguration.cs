using System.Collections.Generic;
using Core.Model;
using Core.Model.Entities;

namespace Core.Configuration
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Data access for all Secure Configuration related data access methods
    /// </summary>
    public static class DataAccessSecureConfiguration
    {

        /// <summary>
        /// Returns the Secure Configuration record value matching the key
        /// </summary>
        /// <param name="service">CRM Org Service</param>
        /// <param name="key">Config record Key</param>
        /// <returns>Config value as string</returns>
        public static string GetConfigurationStringValue(this IOrganizationService service, string key)
        {
            // Get the config record matching the key
            QueryExpression priceQuery = new QueryExpression(SecureConfiguration.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(SecureConfiguration.ValueField),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(SecureConfiguration.KeyField, ConditionOperator.Equal, key)
                    }
                }
            };

            EntityCollection results = service.RetrieveMultiple(priceQuery);

            if (results == null || results.TotalRecordCount == 0)
            {
                return null;
            }

            return results[0][SecureConfiguration.ValueField] as string;
        }

        public static IDictionary<string,string> GetConfigurationStringValues(this IOrganizationService service, params string[] keys)
        {

            // Get the config record matching the key
            IDictionary<string, string> retVal = new Dictionary<string, string>();
            FilterExpression filterExpression = new FilterExpression(LogicalOperator.Or);

            foreach (string key in keys)
            {
                filterExpression.Conditions.Add(new ConditionExpression(SecureConfiguration.KeyField, ConditionOperator.Equal, key));
            }

            QueryExpression priceQuery = new QueryExpression(SecureConfiguration.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(SecureConfiguration.KeyField, SecureConfiguration.ValueField),
                Criteria = filterExpression
            };

            EntityCollection results = service.RetrieveMultiple(priceQuery);

            foreach (Entity result in results.Entities)
            {
                string key = result[SecureConfiguration.KeyField] as string;
                if (key == null)
                {
                    continue;
                }
                retVal.Add(key, result[SecureConfiguration.ValueField] as string);
            }

            return retVal;
        }
    }
}