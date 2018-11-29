namespace WastePermits.DataAccess
{
    using System;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    public static class DataAccessCase
    {
        public static Int32 CountActiveCasesOfType(this IOrganizationService service, Guid applicationId, string caseTypes)
        {
            var addressTypeXml = string.Empty;          
            foreach (var ct in caseTypes.Split(';'))
            {
                addressTypeXml += string.Format("<value>{0}</value>", ct);
            }
            var fetchXml = string.Format(@"<fetch aggregate='true' >
                                              <entity name='incident' >
                                                <attribute name='incidentid' alias='recordcount' aggregate='count' />
                                                <filter>
                                                  <condition attribute='statecode' operator='eq' value='0' />
                                                  <condition attribute='casetypecode' operator='in' >
                                                    {1}
                                                  </condition>
                                                  <condition attribute='defra_application' operator='eq' value='{0}' />
                                                </filter>
                                              </entity>
                                            </fetch>", applicationId.ToString(), addressTypeXml);

            var countResult = service.RetrieveMultiple(new FetchExpression(fetchXml));
            foreach (var cnt in countResult.Entities)
            {
                return (int)((AliasedValue)cnt["recordcount"]).Value;
            }
            return 0;
        }
    }
}