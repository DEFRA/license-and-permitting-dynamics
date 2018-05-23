namespace Lp.DataAccess
{
    using Microsoft.Xrm.Sdk;

    public static class DataAccessApplication
    {
        /// <summary>
        /// Gets the site name from Location and Grid Reference or Address from Location Details\Address
        /// </summary>
        /// <param name="application"></param>
        /// <returns>Location name, grid reference and address concatenated into a string comma separated</returns>
        public static string GetSiteDetailsForApplication(this IOrganizationService service, EntityReference application)
        {
            var fetchXml = string.Format(@"<fetch top='50' >
                                          <entity name='defra_application' >
                                            <filter>
                                              <condition attribute='defra_applicationid' operator='eq' value='{0}' />
                                            </filter>
                                            <link-entity name='defra_location' from='defra_applicationid' to='defra_applicationid' alias='location' >
                                              <attribute name='defra_name' />
                                              <link-entity name='defra_locationdetails' from='defra_locationid' to='defra_locationid' alias='locationdetail' >
                                                <attribute name='defra_gridreferenceid' />
                                                <link-entity name='defra_address' from='defra_addressid' to='defra_addressid' link-type='outer' alias='address' >
                                                  <attribute name='defra_name' />
                                                  <attribute name='defra_premises' />
                                                  <attribute name='defra_street' />
                                                  <attribute name='defra_locality' />
                                                  <attribute name='defra_towntext' />
                                                  <attribute name='defra_postcode' />
                                                </link-entity>
                                              </link-entity>
                                            </link-entity>
                                          </entity>
                                        </fetch>", application.Id.ToString());
            var results = Query.QueryCRMForMultipleRecords(service, fetchXml);
            if (results != null && results.Entities.Count > 0)
            {
                var returnData = string.Empty;
                for (int i = 0; i < results.Entities.Count; i++)
                {
                    var siteDetail = string.Empty;
                    var siteAddress = string.Empty;
                    var gridRef = string.Empty;
                    if (results.Entities[i].Contains("location.defra_name"))
                    {
                        siteDetail = (string)results.Entities[i].GetAttributeValue<AliasedValue>("location.defra_name").Value;
                    }
                    if (results.Entities[i].Contains("locationdetail.defra_gridreferenceid"))
                    {
                        gridRef = (string)results.Entities[i].GetAttributeValue<AliasedValue>("locationdetail.defra_gridreferenceid").Value;
                    }
                    if (results.Entities[i].Contains("address.defra_name"))
                    {
                        siteAddress = (string)results.Entities[i].GetAttributeValue<AliasedValue>("address.defra_name").Value;
                    }
                    if (!string.IsNullOrEmpty(siteAddress))
                    {
                        siteDetail = string.Format("{0}, {1}", siteDetail, siteAddress);
                    }
                    if (!string.IsNullOrEmpty(gridRef))
                    {
                        siteDetail = string.Format("{0}, {1}", siteDetail, gridRef);
                    }
                    returnData = (i == 0) ? siteDetail : returnData + "; " + siteDetail;
                }
                return returnData;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}