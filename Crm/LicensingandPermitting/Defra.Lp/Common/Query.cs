using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Defra.Lp.Common
{
    public static class Query
    {
        public static string GetFieldValueAsString(Entity entity, string attributeName)
        {
            string sendValue = string.Empty;

            if (entity.Attributes.Contains(attributeName))
            {
                object value = entity.Attributes[attributeName];

                if (value != null)
                {
                    if (value.GetType() == typeof(AliasedValue))
                    {
                        AliasedValue aliasedValue = value as AliasedValue;

                        if (aliasedValue.Value != null)
                        {
                            sendValue = Convert.ToString(aliasedValue.Value);
                        }
                    }
                    else
                    {
                        sendValue = Convert.ToString(value);
                    }
                }
            }
            return sendValue;
        }

        public static Entity QueryCRMForSingleEntity(IOrganizationService service, string fetchXml)
        {
            EntityCollection results = QueryCRMForMultipleRecords(service, fetchXml);

            if (results != null && results.Entities.Count >= 1)
            {
                return results.Entities[0];
            }

            return null;
        }

        public static EntityCollection QueryCRMForMultipleRecords(IOrganizationService service, string fetchXml)
        {
            RetrieveMultipleRequest fetchRequest = new RetrieveMultipleRequest
            {
                Query = new FetchExpression(fetchXml)
            };

            return ((RetrieveMultipleResponse)service.Execute(fetchRequest)).EntityCollection;
        }

        public static string GetConfigurationValue(IOrganizationService adminService, string name)
        {
            string query = string.Format(@"<fetch>
                                 <entity name='defra_configuration'>
                                     <attribute name='defra_value' />
                                     <filter>
                                        <condition attribute='defra_name' operator= 'eq' value='{0}' />
                                    </filter>
                                 </entity>
                            </fetch>", name);

            Entity configRecord = Query.QueryCRMForSingleEntity(adminService, query);

            if (configRecord == null || !configRecord.Attributes.Contains("defra_value"))
            {
                throw new InvalidPluginExecutionException(string.Format("Configuration value {0} not found or has not been set", name));
            }
            else
            {
                return (string)configRecord["defra_value"];
            }
        }

        public static Entity GetConfigurationEntity(IOrganizationService adminService, string name)
        {
            string query = string.Format(@"<fetch>
                                 <entity name='defra_configuration'>
                                     <attribute name='defra_addressbasefacadeurl' />
                                     <attribute name='defra_sharepointlogicappurl' />
                                     <attribute name='defra_sharepointpermitlist' />
                                     <attribute name='defra_sharepointfoldercontenttype' />
                                     <filter>
                                        <condition attribute='defra_name' operator= 'eq' value='{0}' />
                                    </filter>
                                 </entity>
                            </fetch>", name);

            Entity configRecord = Query.QueryCRMForSingleEntity(adminService, query);

            if (configRecord == null || configRecord.Attributes.Count == 0)
            {
                throw new InvalidPluginExecutionException(string.Format("Configuration record {0} not found or has not been set", name));
            }
            else
            {
                return configRecord;
            }
        }

        public static string GetCRMOptionsetText(IOrganizationService service, string entityName, string attributeName, int optionSetValue)
        {
            RetrieveEntityRequest retrieveDetails = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.All,
                LogicalName = entityName
            };

            RetrieveEntityResponse retrieveEntityResponseObj = (RetrieveEntityResponse)service.Execute(retrieveDetails);
            EntityMetadata metadata = retrieveEntityResponseObj.EntityMetadata;
            PicklistAttributeMetadata picklistMetadata = metadata.Attributes.FirstOrDefault(attribute => String.Equals(attribute.LogicalName, attributeName, StringComparison.OrdinalIgnoreCase)) as PicklistAttributeMetadata;
            OptionSetMetadata options = picklistMetadata.OptionSet;

            string optionsetLabel = (from o in options.Options
                                     where o.Value.Value == optionSetValue
                                     select o).First().Label.UserLocalizedLabel.Label;
            return optionsetLabel;
        }

        public static Entity RetrieveDataForEntityRef(IOrganizationService service, string[] columns, EntityReference target)
        {
            var retrieveRequest = new RetrieveRequest();
            retrieveRequest.ColumnSet = new ColumnSet(columns);
            retrieveRequest.Target = target;
            var retrieveResponse = (RetrieveResponse)service.Execute(retrieveRequest);
            if (retrieveResponse != null)
            {
                var entity = retrieveResponse.Entity as Entity;
                return entity;
            }
            else
            {
                return null;
            }
        }
    }
}