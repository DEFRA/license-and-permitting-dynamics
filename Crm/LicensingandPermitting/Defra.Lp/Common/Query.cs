using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Lp.Common
{
    public static class Query
    {
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
                                 <entity name='rpa_configuration'>
                                     <attribute name='rpa_value' />
                                     <filter>
                                        <condition attribute='rpa_name' operator= 'eq' value='{0}' />
                                    </filter>
                                 </entity>
                            </fetch>", name);

            Entity configRecord = Query.QueryCRMForSingleEntity(adminService, query);

            if (configRecord == null || !configRecord.Attributes.Contains("rpa_value"))
            {
                throw new InvalidPluginExecutionException(string.Format("Configuration value {0} not found or has not been set", name));
            }
            else
            {
                return (string)configRecord["rpa_value"];
            }
        }
    }
}