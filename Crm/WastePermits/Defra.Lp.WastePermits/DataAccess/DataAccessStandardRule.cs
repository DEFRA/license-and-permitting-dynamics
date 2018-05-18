using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace DAL
{
    public static class DataAccessStandardRule
    {
        public static string GetStandardRules(this IOrganizationService service, EntityReference application)
        {
            var returnData = string.Empty;
            var fetchXml = string.Format(@"<fetch top='50' >
                                                  <entity name='defra_application' >
                                                    <filter>
                                                      <condition attribute='defra_applicationid' operator='eq' value='{0}' />
                                                    </filter>
                                                    <link-entity name='defra_applicationline' from='defra_applicationid' to='defra_applicationid' >
                                                      <filter>
                                                        <condition attribute='defra_permittype' operator='eq' value='910400000' />
                                                      </filter>
                                                      <link-entity name='defra_standardrule' from='defra_standardruleid' to='defra_standardruleid' alias='permit' >
                                                        <attribute name='defra_name' />
                                                        <attribute name='defra_rulesnamegovuk' />
                                                      </link-entity>
                                                    </link-entity>
                                                  </entity>
                                                </fetch>", application.Id.ToString());
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