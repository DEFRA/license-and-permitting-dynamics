namespace WastePermits.DataAccess
{
    using System;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using WastePermits.Model.EarlyBound;

    /// <summary>
    /// Data access layer for Application Lines
    /// </summary>
    public static class DataAccessApplicationLine
    {

        /// <summary>
        /// Count active application lines for a given application 
        /// </summary>
        /// <param name="service">CRM service</param>
        /// <param name="applicationLineId">App to query</param>
        /// <returns>Active app line count</returns>
        public static int CountActiveApplicationLines(this IOrganizationService service, Guid applicationLineId)
        {
            var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>";
            fetchXml += "<entity name='" + defra_applicationline.EntityLogicalName + "'>";
            fetchXml += "<attribute name='defra_name' alias='recordcount' aggregate='count' />";
            fetchXml += "<filter type='and'>";
            fetchXml += "<condition attribute='statecode' operator='eq' value='0' />";
            fetchXml += "<condition attribute='defra_applicationid' operator='eq' uitype='contact' value='" + applicationLineId + "' />";
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

