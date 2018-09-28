using System;
using Microsoft.Xrm.Sdk.Query;
using Model.Lp.Crm;

namespace Lp.DataAccess
{
    using Microsoft.Xrm.Sdk;

    public static class DataAccessApplication
    {
        public static string GetSiteDetails(this IOrganizationService service, EntityReference entityRef)
        {
            var entityName = "defra_application";
            var fieldName = "defra_applicationid";
            return service.GetSiteDetails(entityRef, entityName, fieldName);
        }

        /// <summary>
        /// Gets the site name from Location and Grid Reference or Address from Location Details\Address
        /// </summary>
        /// <param name="application"></param>
        /// <returns>Location name, grid reference and address concatenated into a string comma separated</returns>
        public static string GetSiteDetails(this IOrganizationService service, EntityReference entityRef, string entityName, string fieldName)
        {
            var fetchXml = string.Format(@"<fetch top='50' >
                                          <entity name='{1}' >
                                            <filter>
                                              <condition attribute='{2}' operator='eq' value='{0}' />
                                            </filter>
                                            <link-entity name='defra_location' from='{2}' to='{2}' alias='location' >
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
                                        </fetch>", entityRef.Id.ToString(), entityName, fieldName);
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

        /// <summary>
        /// Updates the Application and Permit Document Locations if they are supplied. These lookups are 
        /// used so workflows have easy access to document locations from Case and Permit.
        /// </summary>
        /// <param name="adminService"></param>
        /// <param name="context"></param>
        /// <param name="target"></param>
        /// <param name="applicationLocation"></param>
        /// <param name="permitLocation"></param>
        public static void UpdateDocumentLocations(this IOrganizationService service,
                                                   Entity target,
                                                   EntityReference applicationLocation = null,
                                                   EntityReference permitLocation = null)
        {
            var updateApplication = new Entity(Application.EntityLogicalName, target.Id);
            var doUpdate = false;
            if (applicationLocation != null)
            {
                updateApplication[Application.ApplicationDocumentLocation] = applicationLocation;
                doUpdate = true;
            }
            if (permitLocation != null)
            {
                updateApplication[Application.PermitDocumentLocation] = permitLocation;
                doUpdate = true;
            }
            if (doUpdate)
            {
                service.Update(updateApplication);
            }
        }


        /// <summary>
        /// Returns all the locations and location details linked to the given application
        /// </summary>
        /// <param name="service">CRM Organisation Service</param>
        /// <param name="applicationId">Application Id to return locations for</param>
        /// <returns>List of Locations and location details</returns>
        public static EntityCollection GetApplicationSites(this IOrganizationService service, Guid applicationId)
        {
            // Instantiate QueryExpression 
            QueryExpression qEdefraLocation = new QueryExpression("defra_location") { TopCount = 1000 };

            // Add columns tolocation
            qEdefraLocation.ColumnSet.AddColumns("statecode", "defra_name", "defra_locationcode", "defra_applicationid", "defra_locationid", "defra_permitid", "defra_highpublicinterest", "statuscode");

            // Define filter 
            qEdefraLocation.Criteria.AddCondition("defra_applicationid", ConditionOperator.Equal, applicationId);

            // Add link-entity defra_locationdetails
            LinkEntity qEdefraLocationDefraLocationdetails = qEdefraLocation.AddLink("defra_locationdetails", "defra_locationid", "defra_locationid", JoinOperator.LeftOuter);
            qEdefraLocationDefraLocationdetails.EntityAlias = "details";

            // Add columns to defra_locationdetails
            qEdefraLocationDefraLocationdetails.Columns.AddColumns("statecode", "defra_locationid", "defra_addressid", "defra_name", "defra_gridreferenceid", "statuscode", "defra_locationdetailsid", "ownerid");

            // Query CRM
            return service.RetrieveMultiple(qEdefraLocation);
        }

        /// <summary>
        /// Returns all the locations and location details linked to the given application
        /// </summary>
        /// <param name="service">CRM Organisation Service</param>
        /// <param name="applicationId">Application Id to return locations for</param>
        /// <returns>List of Locations and location details</returns>
        public static void MirrorApplicationSitesToPermit(this IOrganizationService service, Guid applicationId)
        {
            // 1. Get Application PermitId
            Entity applicationEntity = service.Retrieve(Application.EntityLogicalName, applicationId, new ColumnSet(Application.Permit));
            EntityReference permitEntityReference = applicationEntity.Attributes.ContainsKey(Application.Permit)
                ? applicationEntity[Application.Permit] as EntityReference
                : null;
            if (permitEntityReference == null || permitEntityReference.Id == Guid.Empty)
            {
                // No Permit linked to the application
                return;
            }

            // 2. Get Application Sites
            EntityCollection applicationSites = service.GetApplicationSites(applicationId);

            // 3. Get Permit Sites
            EntityCollection permitSites = GetPermitSites(service, permitEntityReference);

            // 4. Deactivate Removed Permit Sites
            DeactivateRemovedPermitSites(applicationSites, permitSites);

            // 5. Create New Permit Sites
            CreateNewPermitSites(applicationSites, permitSites);
        }

        private static EntityCollection GetPermitSites(IOrganizationService service, EntityReference permitEntityReference)
        {
            // Instantiate QueryExpression QEdefra_permit
            var qEdefraPermit = new QueryExpression("defra_permit");
            qEdefraPermit.TopCount = 50;

            // Add columns to QEdefra_permit.ColumnSet
            qEdefraPermit.ColumnSet.AddColumns("defra_permitid");

            // Define filter QEdefra_permit.Criteria
            qEdefraPermit.Criteria.AddCondition("defra_permitid", ConditionOperator.Equal, permitEntityReference.Id);

            // Add link-entity QEdefra_permit_defra_location
            var qEdefraPermitDefraLocation = qEdefraPermit.AddLink("defra_location", "defra_permitid", "defra_permitid");
            qEdefraPermitDefraLocation.EntityAlias = "permitLocation";

            // Add columns to QEdefra_permit_defra_location.Columns
            qEdefraPermitDefraLocation.Columns.AddColumns("statecode", "defra_name", "defra_locationcode", "defra_locationid",
                "defra_highpublicinterest", "statuscode", "ownerid");

            return service.RetrieveMultiple(qEdefraPermit);
        }

        private static void DeactivateRemovedPermitSites(EntityCollection applicationSites, EntityCollection permitSites)
        {

            foreach (var entity in permitSites.Entities)
            {
                
  
            }
       

        }

        private static void CreateNewPermitSites(EntityCollection applicationSites, EntityCollection permitSites)
        {

        }

    }
}