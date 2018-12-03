namespace WastePermits.DataAccess
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Data access layer used to query CRM for standard rule related data
    /// </summary>
    public static class DataAccessStandardRule
    {
        public static string GetStandardRules(this IOrganizationService service, EntityReference entityRef)
        {
            var entityName = "defra_application";
            var fieldName = "defra_applicationid";
            var lineEntityName = "defra_applicationline";
            return service.GetStandardRules(entityRef, entityName, fieldName, lineEntityName);
        }

        public static string GetStandardRules(this IOrganizationService service, EntityReference entityRef, string entityName, string fieldName, string lineEntityName)
        {
            var returnData = string.Empty;
            var fetchXml = string.Format(@"<fetch top='50' >
                                                  <entity name='{1}' >
                                                    <filter>
                                                      <condition attribute='{2}' operator='eq' value='{0}' />
                                                    </filter>
                                                    <link-entity name='{3}' from='{2}' to='{2}' >
                                                      <filter>
                                                        <condition attribute='defra_permittype' operator='eq' value='910400000' />
                                                      </filter>
                                                      <link-entity name='defra_standardrule' from='defra_standardruleid' to='defra_standardruleid' alias='permit' >
                                                        <attribute name='defra_name' />
                                                        <attribute name='defra_rulesnamegovuk' />
                                                      </link-entity>
                                                    </link-entity>
                                                  </entity>
                                                </fetch>", entityRef.Id.ToString(), entityName, fieldName, lineEntityName);
            RetrieveMultipleRequest fetchRequest = new RetrieveMultipleRequest
            {
                Query = new FetchExpression(fetchXml)
            };
            var results = ((RetrieveMultipleResponse)service.Execute(fetchRequest)).EntityCollection;

            if (results != null && results.Entities.Count > 0)
            {
                for (int i = 0; i < results.Entities.Count; i++)
                {
                    var code = string.Empty;
                    var name = string.Empty;
                    if (results.Entities[i].Contains("permit.defra_name"))
                    {
                        code = (string)results.Entities[i].GetAttributeValue<AliasedValue>("permit.defra_name").Value;
                    }
                    if (results.Entities[i].Contains("permit.defra_rulesnamegovuk"))
                    {
                        name = (string)results.Entities[i].GetAttributeValue<AliasedValue>("permit.defra_rulesnamegovuk").Value;
                    }
                    var permit = string.Format("{0} - {1}", code, name);
                    returnData = (i == 0) ? returnData + permit : returnData + "; " + permit;
                }
            }
            else
            {
                returnData = string.Empty;
            }

            return returnData;
        }
    }
}