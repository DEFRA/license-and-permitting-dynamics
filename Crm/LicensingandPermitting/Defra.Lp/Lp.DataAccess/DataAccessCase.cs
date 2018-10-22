namespace Lp.DataAccess
{

    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    /// <summary>
    /// Class provides data access to the cases
    /// </summary>
    public static class DataAccessCase
    {
        /// <summary>
        /// Returns the number of cases of a given type that are linked to an application
        /// </summary>
        /// <param name="service"></param>
        /// <param name="applicationId"></param>
        /// <param name="caseTypes"></param>
        /// <returns></returns>
        public static Int32 CountActiveCasesOfType(this IOrganizationService service, Guid applicationId, string caseTypes)
        {
            var addressTypeXml = string.Empty;
            foreach (var ct in caseTypes.Split(';'))
            {
                addressTypeXml += $"<value>{ct}</value>";
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
