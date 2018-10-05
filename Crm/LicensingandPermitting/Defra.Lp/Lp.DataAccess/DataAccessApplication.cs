using System;
using System.Collections.Generic;
using System.Linq;
using Lp.Model.EarlyBound;
using Microsoft.Crm.Sdk;
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
        /// <param name="permitId">Permit Id to return locations for</param>
        /// <returns>List of Locations and location details</returns>
        public static EntityCollection GetLocationAndLocationDetails(this IOrganizationService service, Guid? applicationId, Guid? permitId)
        {
            // Instantiate QueryExpression 
            QueryExpression qEdefraLocation = new QueryExpression("defra_location") { TopCount = 1000 };

            // Add columns tolocation
            qEdefraLocation.ColumnSet.AddColumns("statecode", "defra_name", "defra_locationcode", "defra_applicationid", "defra_locationid", "defra_permitid", "defra_highpublicinterest", "statuscode");
            
            // Only retrieve active locations
            qEdefraLocation.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)defra_locationState.Active);

            // Define filter 
            if (applicationId.HasValue)
            {
                qEdefraLocation.Criteria.AddCondition("defra_applicationid", ConditionOperator.Equal, applicationId);
            }

            if (permitId.HasValue)
            {
                qEdefraLocation.Criteria.AddCondition("defra_permitid", ConditionOperator.Equal, permitId);
            }

            // Add link-entity defra_locationdetails
            LinkEntity qEdefraLocationDefraLocationdetails = qEdefraLocation.AddLink("defra_locationdetails", "defra_locationid", "defra_locationid", JoinOperator.LeftOuter);
            qEdefraLocationDefraLocationdetails.EntityAlias = LocationDetail.Alias;

            // Only retrieve active location details
            qEdefraLocationDefraLocationdetails.LinkCriteria.AddCondition("statecode", ConditionOperator.Equal, (int)defra_locationdetailsState.Active);

            // Add columns to defra_locationdetails
            qEdefraLocationDefraLocationdetails.Columns.AddColumns("statecode", "defra_locationid", "defra_addressid", "defra_name", "defra_gridreferenceid", "statuscode", "defra_locationdetailsid", "ownerid");

            // Query CRM
            return service.RetrieveMultiple(qEdefraLocation);
        }

        /// <summary>
        /// Mirrors Location and Location Details from application to permit
        /// </summary>
        /// <param name="service">CRM Organisation Service</param>
        /// <param name="applicationId">Application Id to be processed</param>
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
            EntityCollection applicationSites = service.GetLocationAndLocationDetails(applicationId, null);

            // 3. Get Permit Sites
            EntityCollection permitSites = service.GetLocationAndLocationDetails(null, permitEntityReference.Id);

            // 4. Deactivate Removed Permit Sites and Details
            Entity[] remainingPermitSites = DeactivatePermitSitesIfNeeded(service, applicationSites, permitSites);

            // 5. Create New Permit Sites and Details
            CreateNewPermitSites(service, applicationSites.Entities.ToArray(), remainingPermitSites, permitEntityReference);
        }

        /// <summary>
        /// Mirrors Location and Location Details from permit to application
        /// </summary>
        /// <param name="service">CRM Organisation Service</param>
        /// <param name="applicationId">Application Id to be processed</param>
        public static void MirrorPermitSitesToApplication(this IOrganizationService service, Guid applicationId)
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

            // 2. Get Permit Sites
            EntityCollection permitSites = service.GetLocationAndLocationDetails(null, permitEntityReference.Id);

            // 3. Create New Sites
            CreateNewApplicationtSites(service, permitSites.Entities.ToArray(), applicationId);
        }


        private static Entity[] DeactivatePermitSitesIfNeeded(IOrganizationService service, EntityCollection applicationSitesAndDetails, EntityCollection permitSitesAndDetails)
        {
            // What we have linked to the Permit at the moment
            List<Entity> remainingPermitSiteDetails = permitSitesAndDetails.Entities.ToList();

            // 1. Iterate through permit sites
            foreach (var permitSiteAndDetail in permitSitesAndDetails.Entities)
            {
                // 2. Check if Permit Site Matches an Application Site
                Entity applicationSiteAndDetail = GetMatchingLocation(permitSiteAndDetail, applicationSitesAndDetails.Entities.ToArray());

                if (applicationSiteAndDetail == null)
                {
                    // 3. Unlink the Permit Site Detail in CRM
                    UnlinkPermitSiteDetail(service, permitSiteAndDetail);

                    // 4. Remove the the remaining permit site details
                    remainingPermitSiteDetails.Remove(permitSiteAndDetail);
                }
            }

            // 5. Check if we need to unlink permitLocations
            foreach (var permitSiteAndDetail in permitSitesAndDetails.Entities)
            {
                // Does the permit Location record need to be unlinked from the permit? (i.e. there are not Side details left for that location
                var permitLocationStillUsed = false;

                foreach (var remainingPermitSideDetail in remainingPermitSiteDetails)
                {
                    if (remainingPermitSideDetail[Location.LocationId] == permitSiteAndDetail[Location.LocationId])
                    {
                        // Location still being used, let it stay
                        permitLocationStillUsed = true;
                        break;
                    }
                }

                if (!permitLocationStillUsed)
                {
                    UnlinkPermitSite(service, permitSiteAndDetail);
                }
            }
            return remainingPermitSiteDetails.ToArray();
        }

        private static void CreateNewPermitSites(IOrganizationService service, Entity[] applicationSiteAndDetails, Entity[] permitSitesAndDetails, EntityReference permitEntityReference)
        {
            List<Entity> newAndExistingPermitSites = permitSitesAndDetails.ToList();

            // 1. Iterate through Application Site Details
            foreach (var applicationSiteAndDetail in applicationSiteAndDetails)
            {
                // 2. Check if site already exists in Permit
                Entity permitSite = GetMatchingLocation(applicationSiteAndDetail, newAndExistingPermitSites.ToArray());

                Entity permitSiteAndDetail = GetMatchingLocationDetail(applicationSiteAndDetail, permitSitesAndDetails);

                // 3. If Permit Location does not exist, create it
                if (permitSite == null)
                {
                    permitSite = CopyLocation(service, applicationSiteAndDetail, null, permitEntityReference.Id);
                    newAndExistingPermitSites.Add(permitSite);
                }

                // 4. If Permit Location detail does not exist, create it
                if (permitSiteAndDetail == null)
                {
                    CopyLocationDetail(service, applicationSiteAndDetail, permitSite);
                }
            }
        }



        private static void CreateNewApplicationtSites(IOrganizationService service, Entity[] permitSitesAndDetails, Guid applicationId)
        {
            List<Entity> applicationSites = new List<Entity>();

            // 1. Iterate through Permit Site Details
            foreach (var permitSiteAndDetail in permitSitesAndDetails)
            {
                // 2. Check if site already exists in Permit
                Entity applicationSite = GetMatchingLocation(permitSiteAndDetail, applicationSites.ToArray());

                Entity applicationSiteAndDetail = GetMatchingLocationDetail(permitSiteAndDetail, permitSitesAndDetails);

                // 3. If Application Location does not exist, create it
                if (applicationSite == null)
                {
                    applicationSite = CopyLocation(service, permitSiteAndDetail, applicationId, null);
                    applicationSites.Add(applicationSite);
                }

                // 4. If Permit Location detail does not exist, create it
                if (applicationSiteAndDetail == null)
                {
                    CopyLocationDetail(service, permitSiteAndDetail, applicationSite);
                }
            }
        }

        private static Entity CopyLocation(IOrganizationService service, Entity locationToCopy, Guid? applicationId, Guid? permitId)
        {
            // 1. Mirror Location
            Entity locationEntity = new Entity(Location.EntityLogicalName)
            {
                [Location.Name] = locationToCopy.Contains(Location.Name) ? locationToCopy[Location.Name]: null
            };

            if (applicationId != null)
            {
                locationEntity.Attributes.Add(Location.Application, new EntityReference(Application.EntityLogicalName, applicationId.Value));
            }

            if (permitId != null)
            {
                locationEntity.Attributes.Add(Location.Permit, new EntityReference(Permit.EntityLogicalName, permitId.Value));
            }

            if (locationToCopy.Contains(Location.HighPublicInterest))
            {
                locationEntity.Attributes.Add(Location.HighPublicInterest, locationToCopy[Location.HighPublicInterest]);
            }

            // 2. Create and return new location
            locationEntity.Id = service.Create(locationEntity);
            return locationEntity;
        }

        private static Entity CopyLocationDetail(IOrganizationService service, Entity locationDetailToCopy, Entity permitLocation)
        {
            // Check if there is a location detail to copy

            if (!locationDetailToCopy.Contains(LocationDetail.Alias + "." + LocationDetail.Adress) &&
                !locationDetailToCopy.Contains(LocationDetail.Alias + "." + LocationDetail.GridReference))
            {
                // No location detail to create;
                return null;
            }

            // 1. Mirror Location Detail
            Entity locationDetailEntity = new Entity(LocationDetail.EntityLogicalName)
            {
                [LocationDetail.Location] = new EntityReference(Location.EntityLogicalName, permitLocation.Id),
                [LocationDetail.Name] = locationDetailToCopy.Contains(LocationDetail.Alias + "." + LocationDetail.Name) ? ((AliasedValue)locationDetailToCopy[LocationDetail.Alias + "." + LocationDetail.Name]).Value : null,
                [LocationDetail.GridReference] = locationDetailToCopy.Contains(LocationDetail.Alias + "." + LocationDetail.Name) ? ((AliasedValue)locationDetailToCopy[LocationDetail.Alias + "." + LocationDetail.GridReference]).Value : null,
                [LocationDetail.Adress] = locationDetailToCopy.Contains(LocationDetail.Alias + "." + LocationDetail.Adress) ? ((AliasedValue)locationDetailToCopy[LocationDetail.Alias + "." + LocationDetail.Adress]).Value : null
            };

            // 2. Create and return new location detail 
            locationDetailEntity.Id = service.Create(locationDetailEntity);
            return locationDetailEntity;
        }

        private static Entity GetMatchingLocation(Entity locationToFind, Entity[] locationsToSearch)
        {
            foreach (Entity locationToSearch in locationsToSearch)
            {

                if (MatchesAttribute(locationToSearch, locationToFind, Location.Name)
                    && MatchesAttribute(locationToSearch, locationToFind, Location.HighPublicInterest))
                {
                    return locationToSearch;
                }
            }
            return null;
        }


        private static Entity GetMatchingLocationDetail(Entity locationDetailToFind, Entity[] locationDetailsToSearch)
        {
            foreach (Entity locationDetailToSearch in locationDetailsToSearch)
            {

                if (MatchesAttribute(locationDetailToSearch, locationDetailToFind, LocationDetail.Name)
                    && MatchesAttribute(locationDetailToSearch, locationDetailToFind, LocationDetail.Adress)
                    && MatchesAttribute(locationDetailToSearch, locationDetailToFind, LocationDetail.GridReference))
                {
                    return locationDetailToSearch;
                }
            }
            return null;
        }


        private static bool MatchesAttribute(Entity entity1, Entity entity2, string attributeName)
        {
            var entityAttribute1 = entity1.Contains(attributeName) ? entity1[attributeName] : null;
            var entityAttribute2 = entity2.Contains(attributeName) ? entity2[attributeName] : null;

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (entityAttribute1 != null && entityAttribute1.Equals(entityAttribute2))
            {
                return true;
            }
            return false;
        }


        private static void UnlinkPermitSite(IOrganizationService service, Entity locationEntity)
        {
            locationEntity[Location.Permit] = null;
            service.Update(locationEntity);
        }

        private static void UnlinkPermitSiteDetail(IOrganizationService service, Entity locationEntityAndDetail)
        {
            locationEntityAndDetail[LocationDetail.Location] = null;
            service.Update(locationEntityAndDetail);
        }
    }
}