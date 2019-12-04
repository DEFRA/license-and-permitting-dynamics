namespace WastePermits.DataAccess
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Query;
    using Model.EarlyBound;
    using Core.Helpers.Extensions;

    /// <summary>
    /// Data access layer used to query CRM for standard rule related data
    /// </summary>
    public static class DataAccessStandardRule
    {
        public static string GetStandardRules(this IOrganizationService service, EntityReference entityRef)
        {
            var entityName = defra_application.EntityLogicalName;
            var fieldName = defra_application.Fields.defra_applicationId;
            var lineEntityName = defra_applicationline.EntityLogicalName;
            return service.GetStandardRules(entityRef, entityName, fieldName, lineEntityName);
        }

        public static string GetStandardRules(this IOrganizationService service, EntityReference entityRef, string entityName, string fieldName, string lineEntityName)
        {
            const string alias = "permit";
            var returnData = string.Empty;
            var fetchXml = $@"<fetch top='50' >
                                  <entity name='{entityName}' >
                                    <filter>
                                      <condition attribute='{fieldName}' operator='eq' value='{entityRef.Id.ToString()}' />
                                    </filter>
                                    <link-entity name='{lineEntityName}' from='{fieldName}' to='{fieldName}' ><attribute name='defra_itemid' />
                                      <link-entity name='{defra_standardrule.EntityLogicalName}' from='{defra_standardrule.Fields.defra_standardruleId}' to='{defra_applicationline.Fields.defra_standardruleId}' alias='{alias}' link-type='outer' >
                                        <attribute name='{defra_standardrule.Fields.defra_name}' />
                                        <attribute name='{defra_standardrule.Fields.defra_rulesnamegovuk}' />
                                      </link-entity>
                                    </link-entity>
                                  </entity>
                                </fetch>";
            RetrieveMultipleRequest fetchRequest = new RetrieveMultipleRequest
            {
                Query = new FetchExpression(fetchXml)
            };
            var results = ((RetrieveMultipleResponse)service.Execute(fetchRequest)).EntityCollection;

            if (results != null && results.Entities.Count > 0)
            {
                for (int i = 0; i < results.Entities.Count; i++)
                {
                    var code = results[i].GetAliasedAttributeText($"{alias}.{defra_standardrule.Fields.defra_name}");
                    var name = results[i].GetAliasedAttributeText($"{alias}.{defra_standardrule.Fields.defra_rulesnamegovuk}");
                    var permit = $"{code} - {name}";
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
