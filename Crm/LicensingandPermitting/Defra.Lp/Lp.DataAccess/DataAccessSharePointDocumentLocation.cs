namespace Lp.DataAccess
{
    using Microsoft.Xrm.Sdk;

    public static class DataAccessSharePointDocumentLocation
    {
        public static EntityReference FindDefaultSharePointSite(this IOrganizationService service)
        {
            var fetchXml = string.Format(@"<fetch top='1' >
                                              <entity name='sharepointsite' >
                                                <attribute name='name' />
                                                <attribute name='isdefault' />
                                                <attribute name='sharepointsiteid' />
                                                <filter type='and' >
                                                  <condition attribute='isdefault' operator='eq' value='1' />
                                                </filter>
                                              </entity>
                                            </fetch>");
            var result = Query.QueryCRMForSingleEntity(service, fetchXml);
            if (result == null || !result.Attributes.Contains("sharepointsiteid"))
            {
                throw new InvalidPluginExecutionException("Unable to find Default SharePoint Site. Has Server based SharePoint integration been enabled?");
            }
            else
            {
                var defaultSiteRef = result.GetAttributeValue<EntityReference>("sharepointsiteid");
                if (defaultSiteRef == null)
                {
                    throw new InvalidPluginExecutionException("Unable to find Default SharePoint Site. Has Server based SharePoint integration been enabled?");
                }
                else
                {
                    return defaultSiteRef;
                }
            }
        }

        public static EntityReference FindPermitListInSharePoint(this IOrganizationService service, string defaultSite, string permitListName)
        {
            var fetchXml = string.Format(@"<fetch top='1' >
                                              <entity name='sharepointdocumentlocation' >
                                                <attribute name='regardingobjectid' />
                                                <attribute name='relativeurl' />
                                                <attribute name='name' />
                                                <attribute name='parentsiteorlocation' />
                                                <attribute name='sharepointdocumentlocationid' />
                                                <filter type='and' >
                                                    <condition attribute='name' operator='eq' value='{0}' />
                                                    <condition attribute='parentsiteorlocation' operator='eq' value='{1}' />
                                                </filter>
                                              </entity>
                                            </fetch>", permitListName, defaultSite);
            var result = Query.QueryCRMForSingleEntity(service, fetchXml);
            if (result == null || !result.Attributes.Contains("sharepointdocumentlocationid"))
            {
                throw new InvalidPluginExecutionException(string.Format("Unable to find Document Location for {0} SharePoint List.", permitListName));
            }
            else
            {
                var defaultSiteRef = result.GetAttributeValue<EntityReference>("sharepointdocumentlocationid");
                if (defaultSiteRef == null)
                {
                    throw new InvalidPluginExecutionException(string.Format("Unable to find Document Location for {0} SharePoint List.", permitListName));
                }
                else
                {
                    return defaultSiteRef;
                }
            }
        }


        public static EntityReference CreatePermitDocumentLocation(this IOrganizationService service, string permitNumber, EntityReference parentLocation, EntityReference regarding)
        {
            Entity newLocation = new Entity("sharepointdocumentlocation");
            newLocation["name"] = permitNumber;
            newLocation["parentsiteorlocation"] = parentLocation;
            newLocation["relativeurl"] = permitNumber;
            newLocation["regardingobjectid"] = regarding;

            newLocation.Id = service.Create(newLocation);

            return newLocation.ToEntityReference();
        }

        public static void CreateApplicationDocumentLocation(this IOrganizationService service, string applicationNumber, EntityReference parentLocation, EntityReference regarding)
        {
            Entity newLocation = new Entity("sharepointdocumentlocation");
            newLocation["name"] = applicationNumber;
            newLocation["parentsiteorlocation"] = parentLocation;
            newLocation["relativeurl"] = applicationNumber;
            newLocation["regardingobjectid"] = regarding;

            newLocation.Id = service.Create(newLocation);
        }
    }
}