namespace DAL
{
    using System;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    public static class DataAccessCase
    {

        public static Int32 CountActiveCasesOfType(this IOrganizationService service, Guid applicationLineId, OptionSetValue caseType)
        {
            var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>";
            fetchXml += "<entity name='incident'>";
            fetchXml += "<attribute name='title' alias='recordcount' aggregate='count' />";
            fetchXml += "<filter type='and'>";
            fetchXml += "<condition attribute='statecode' operator='eq' value='0' />";
            fetchXml += "<condition attribute='defra_applicationid' operator='eq' uitype='contact' value='" + applicationLineId + "' />";
            fetchXml += "<condition attribute='casetype' operator='eq'  value='" + caseType.Value + "' />";
            fetchXml += "</filter>";
            fetchXml += "</entity>";
            fetchXml += "</fetch>";

            EntityCollection countResult = service.RetrieveMultiple(new FetchExpression(fetchXml));

            foreach (Entity c in countResult.Entities)
            {
                return (int)((AliasedValue)c["recordcount"]).Value;
            }
            return 0;

        }
    }
}

