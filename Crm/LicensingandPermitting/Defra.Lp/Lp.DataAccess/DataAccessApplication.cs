namespace Lp.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Crm.Sdk.Messages;
    using Model.EarlyBound;
    using Model.Crm;

    /// <summary>
    /// Class that deals with CRM data access requests relating to Applications
    /// </summary>
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
                        if (!siteAddress.Equals(siteDetail, StringComparison.OrdinalIgnoreCase))
                        {
                            siteDetail = string.Format("{0}, {1}", siteDetail, siteAddress);
                        }
                    }
                    if (!string.IsNullOrEmpty(gridRef))
                    {
                        if (string.IsNullOrEmpty(siteDetail))
                        {
                            siteDetail = gridRef;
                        }
                        else
                        {
                            siteDetail = string.Format("{0}, {1}", siteDetail, gridRef);
                        }
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
        public static Entity[] GetLocationAndLocationDetails(IOrganizationService service, Guid? applicationId, Guid? permitId)
        {
            // Set-up Location Query
            QueryExpression qEdefraLocation = new QueryExpression(Location.EntityLogicalName) { TopCount = 1000 };
            qEdefraLocation.ColumnSet.AddColumns(Location.State, Location.Name, Location.LocationCode, Location.Application, Location.LocationId, Location.Permit, Location.HighPublicInterest, Location.Status, Location.OwnerId);
            qEdefraLocation.Criteria.AddCondition(Location.State, ConditionOperator.Equal, (int)defra_locationState.Active);

            // Application Locations?
            if (applicationId.HasValue)
            {
                qEdefraLocation.Criteria.AddCondition(Location.Application, ConditionOperator.Equal, applicationId);
            }

            // Permit Locations?
            if (permitId.HasValue)
            {
                qEdefraLocation.Criteria.AddCondition(Location.Permit, ConditionOperator.Equal, permitId);
            }

            // Add link-entity defra_locationdetails
            LinkEntity qEdefraLocationDefraLocationdetails = qEdefraLocation.AddLink(LocationDetail.EntityLogicalName, Location.LocationId, LocationDetail.Location, JoinOperator.LeftOuter);
            qEdefraLocationDefraLocationdetails.EntityAlias = LocationDetail.Alias;
            qEdefraLocationDefraLocationdetails.Columns.AddColumns(LocationDetail.State, LocationDetail.Location, LocationDetail.Address, LocationDetail.Name, LocationDetail.GridReference, LocationDetail.Status, LocationDetail.LocationDetailId, LocationDetail.Owner);

            // Only retrieve active location details
            qEdefraLocationDefraLocationdetails.LinkCriteria.AddCondition(LocationDetail.State, ConditionOperator.Equal, (int)defra_locationdetailsState.Active);

            // Query CRM
            return service.RetrieveMultiple(qEdefraLocation).Entities.ToArray();
        }

        /// <summary>
        /// Mirrors Location and Location Details from application to permit
        /// </summary>
        /// <param name="service">CRM Organisation Service</param>
        /// <param name="applicationId">Application Id to be processed</param>
        public static void MirrorApplicationLocationsAndDetailsToPermit(IOrganizationService service, Guid applicationId)
        {
            // 1. Get Application Permit, and exist if not set
            EntityReference permitEntityReference = GetApplicationPermitId(service, applicationId);
            if (permitEntityReference == null)
            {
                return;
            }

            // 2. Get Application Sites
            Entity[] applicationSites = GetLocationAndLocationDetails(service, applicationId, null);

            // 3. Get Permit Sites
            Entity[] permitSites = GetLocationAndLocationDetails(service, null, permitEntityReference.Id);

            // 4. Deactivate Removed Permit Sites Details
            DeactivatePermitLocationsDetailsIfNeeded(service, applicationSites, permitSites);

            // 5. Get Permit Sites after tidy up
            permitSites = GetLocationAndLocationDetails(service, null, permitEntityReference.Id);

            // 6. Deactivate Empty Permit Locations
            Entity[] remainingPermitSites = DeactivatePermitLocationIfNeeded(service, permitSites);

            // 7. Create New Permit Sites and Details
            CreateNewPermitLocationAndDetails(service, applicationSites, remainingPermitSites, permitEntityReference.Id);
        }


        /// <summary>
        /// Mirrors Location and Location Details from application to permit
        /// </summary>
        /// <param name="service">CRM Organisation Service</param>
        /// <param name="fromApplicationId">Application Id to be copied from</param>
        /// <param name="toApplicationId">Application Id to copy to</param>
        public static void MirrorApplicationLocationsAndDetailsToApplication(IOrganizationService service, Guid fromApplicationId, Guid toApplicationId)
        {
            // 1. Get Application Sites
            Entity[] applicationSites = GetLocationAndLocationDetails(service, fromApplicationId, null);

            // 2. Create Sites and Details on target application
            CreateNewApplicationtLocationAndDetails(service, applicationSites, toApplicationId);
        }

        /// <summary>
        /// Mirrors Location and Location Details from permit to application
        /// </summary>
        /// <param name="service">CRM Organisation Service</param>
        /// <param name="applicationId">Application Id to be processed</param>
        public static void MirrorPermitLocationsAndDetailsToApplication(IOrganizationService service, Guid applicationId)
        {
            // 1. Get Application PermitId
            EntityReference permitEntityReference = GetApplicationPermitId(service, applicationId);
            if (permitEntityReference == null)
            {
                return;
            }

            // 2. Get Permit Sites
            Entity[] permitSites = GetLocationAndLocationDetails(service, null, permitEntityReference.Id);

            // 3. Create Application Sites, when going from Permit to Application, the application wil be empty
            CreateNewApplicationtLocationAndDetails(service, permitSites, applicationId);
        }

        /// <summary>
        /// Get application permit if it exists
        /// </summary>
        /// <param name="service">CRM Org service</param>
        /// <param name="applicationId">application id to check for a permit</param>
        /// <returns>Permit EntityReference if it exists</returns>
        private static EntityReference GetApplicationPermitId(IOrganizationService service, Guid applicationId)
        {
            Entity applicationEntity = service.Retrieve(Application.EntityLogicalName, applicationId, new ColumnSet(Application.Permit));
            EntityReference permitEntityReference = applicationEntity.Attributes.ContainsKey(Application.Permit)
                ? applicationEntity[Application.Permit] as EntityReference
                : null;
            return permitEntityReference;
        }

        /// <summary>
        /// Removes the Permit location detail records that should no longer be there
        /// </summary>
        /// <param name="service"></param>
        /// <param name="applicationSitesAndDetails"></param>
        /// <param name="permitSitesAndDetails"></param>
        /// <returns></returns>
        private static Entity[] DeactivatePermitLocationsDetailsIfNeeded(IOrganizationService service, Entity[] applicationSitesAndDetails, Entity[] permitSitesAndDetails)
        {
            // List of locations and location details currently linked to the permit, i.e. What we have linked to the Permit at the moment
            List<Entity> remainingPermitSiteDetails = permitSitesAndDetails.ToList();

            // Iterate through permit Location details
            foreach (var permitSiteAndDetail in permitSitesAndDetails)
            {
                // 1. Check if Permit Location Detail Matches an Application Site Detail
                Entity applicationSiteAndDetail = GetMatchingLocation(permitSiteAndDetail, applicationSitesAndDetails);

                if (applicationSiteAndDetail != null)
                {
                    // Permit Location Detail has matching Application Site Detail, keep it
                    continue;
                }

                // 2. Permit Location Detail not in application, deactivate it
                var locationDetailToUnlink = permitSiteAndDetail.Contains(GetAliasLocationDetailFieldName(defra_locationdetails.PrimaryIdAttribute))
                        ? ((AliasedValue)permitSiteAndDetail[GetAliasLocationDetailFieldName(defra_locationdetails.PrimaryIdAttribute)]).Value as Guid?
                        : null;
                if (locationDetailToUnlink != null)
                {
                    // There is Permit Location Detail to deactivate.
                    DeactivatePermitLocationDetail(service, locationDetailToUnlink.Value);
                }

                remainingPermitSiteDetails.Remove(permitSiteAndDetail);
            }
            return remainingPermitSiteDetails.ToArray();
        }

        /// <summary>
        /// Removes the Permit location records that should no longer be there
        /// </summary>
        /// <param name="service"></param>
        /// <param name="permitSitesAndDetails"></param>
        /// <returns></returns>
        private static Entity[] DeactivatePermitLocationIfNeeded(IOrganizationService service, Entity[] permitSitesAndDetails)
        {
            // List of locations and location details currently linked to the permit, i.e. What we have linked to the Permit at the moment
            List<Entity> remainingPermitSiteDetails = permitSitesAndDetails.ToList();

            // Iterate through permit Location details
            foreach (var permitSiteAndDetail in permitSitesAndDetails)
            {

                // 2. Check if Permit Location has any Location Details
                var permitLocationDetailId = permitSiteAndDetail.Contains(GetAliasLocationDetailFieldName(defra_locationdetails.PrimaryIdAttribute))
                    ? ((AliasedValue)permitSiteAndDetail[GetAliasLocationDetailFieldName(defra_locationdetails.PrimaryIdAttribute)]).Value as Guid?
                    : null;

                if (permitLocationDetailId == null)
                {
                    // There is Permit Location Detail to deactivate.
                    DeactivatePermitLocation(service, permitSiteAndDetail.Id);
                    remainingPermitSiteDetails.Remove(permitSiteAndDetail);
                }
            }
            return remainingPermitSiteDetails.ToArray();
        }


        /// <summary>
        /// Creates Location and Location details records that exist in the Application
        /// but not on the Permit
        /// </summary>
        /// <param name="service">CRM Organisation Service</param>
        /// <param name="applicationSiteAndDetails">Location and location detail records currently linked to the Application</param>
        /// <param name="permitSitesAndDetails">>Location and location detail records currently linked to the Permit</param>
        /// <param name="permitId">Target Permit Id</param>
        private static void CreateNewPermitLocationAndDetails(IOrganizationService service, Entity[] applicationSiteAndDetails, Entity[] permitSitesAndDetails, Guid permitId)
        {
            List<Entity> newAndExistingPermitSites = permitSitesAndDetails.ToList();

            // 1. Iterate through Application Site Details
            foreach (var applicationSiteAndDetail in applicationSiteAndDetails)
            {
                // 2. Check if site already exists in Permit
                Entity permitSite = GetMatchingLocation(applicationSiteAndDetail, newAndExistingPermitSites.ToArray());

                Entity permitSiteAndDetail = null;

                // 3. If Permit Location does not exist, create it
                if (permitSite == null)
                {
                    permitSite = CopyLocation(service, applicationSiteAndDetail, null, permitId);
                    newAndExistingPermitSites.Add(permitSite);
                }
                else
                {
                    // Ok it's an existing location in the target, check if the location detail already rexists
                    permitSiteAndDetail = GetMatchingLocationDetail(applicationSiteAndDetail, permitSitesAndDetails);
                }

                // 4. If Permit Location detail does not exist, create it
                if (permitSiteAndDetail == null)
                {
                    CopyLocationDetail(service, applicationSiteAndDetail, permitSite.Id);
                }
            }
        }

        /// <summary>
        /// Creates location and location detail records that do not already exist
        /// in the Application but do exist in the Permit
        /// </summary>
        /// <param name="service">CRM Org service</param>
        /// <param name="permitSitesAndDetails">Location and location detail records currently linked to the Permit</param>
        /// <param name="applicationId">Target Application Id</param>
        private static void CreateNewApplicationtLocationAndDetails(IOrganizationService service, Entity[] permitSitesAndDetails, Guid applicationId)
        {
            List<Entity> applicationSites = new List<Entity>();

            // 1. Iterate through Permit Site Details
            foreach (var permitSiteAndDetail in permitSitesAndDetails)
            {
                // 2. Check if site already exists in Permit
                Entity applicationSite = GetMatchingLocation(permitSiteAndDetail, applicationSites.ToArray());

                Entity applicationSiteAndDetail = GetMatchingLocationDetail(permitSiteAndDetail, applicationSites.ToArray());

                // 3. If Application Location does not exist, create it
                if (applicationSite == null)
                {
                    applicationSite = CopyLocation(service, permitSiteAndDetail, applicationId, null);
                    applicationSites.Add(applicationSite);
                }

                // 4. If Permit Location detail does not exist, create it
                if (applicationSiteAndDetail == null)
                {
                    CopyLocationDetail(service, permitSiteAndDetail, applicationSite.Id);
                }
            }
        }

        /// <summary>
        /// Copies a location record to the given target
        /// </summary>
        /// <param name="service">CRM Org service</param>
        /// <param name="locationToCopy">Location record to copy to target</param>
        /// <param name="targetApplicationId">Optional target application id</param>
        /// <param name="targetPermitId">Optional target permit id</param>
        /// <returns></returns>
        private static Entity CopyLocation(IOrganizationService service, Entity locationToCopy, Guid? targetApplicationId, Guid? targetPermitId)
        {
            // 1. Mirror Location
            Entity locationEntity = new Entity(Location.EntityLogicalName)
            {
                [Location.Name] = locationToCopy.Contains(Location.Name) ? locationToCopy[Location.Name] : null
            };

            if (targetApplicationId != null)
            {
                locationEntity.Attributes.Add(Location.Application, new EntityReference(Application.EntityLogicalName, targetApplicationId.Value));
            }

            if (targetPermitId != null)
            {
                locationEntity.Attributes.Add(Location.Permit, new EntityReference(Permit.EntityLogicalName, targetPermitId.Value));
            }

            if (locationToCopy.Contains(Location.HighPublicInterest))
            {
                locationEntity.Attributes.Add(Location.HighPublicInterest, locationToCopy[Location.HighPublicInterest]);
            }

            locationEntity.Attributes.Add(Location.OwnerId, locationToCopy[Location.OwnerId]);

            // 2. Create and return new location
            locationEntity.Id = service.Create(locationEntity);
            return locationEntity;
        }

        /// <summary>
        /// Copy location detail record to given target
        /// </summary>
        /// <param name="service">CRM Org Serviced</param>
        /// <param name="locationDetailToCopy">Location detail to be copied to target location</param>
        /// <param name="targetLocationId">The location id to receive the copy</param>
        /// <returns></returns>
        private static Entity CopyLocationDetail(IOrganizationService service, Entity locationDetailToCopy, Guid targetLocationId)
        {
            // Check if there is a location detail to copy
            if (!locationDetailToCopy.Contains(GetAliasLocationDetailFieldName(LocationDetail.Address)) &&
                !locationDetailToCopy.Contains(GetAliasLocationDetailFieldName(LocationDetail.GridReference)))
            {
                // No location detail to create;
                return null;
            }

            // 1. Mirror Location Detail
            Entity locationDetailEntity = new Entity(LocationDetail.EntityLogicalName)
            {
                [LocationDetail.Location] = new EntityReference(Location.EntityLogicalName, targetLocationId),
                [LocationDetail.Name] = locationDetailToCopy.Contains(GetAliasLocationDetailFieldName(LocationDetail.Name)) ? ((AliasedValue)locationDetailToCopy[GetAliasLocationDetailFieldName(LocationDetail.Name)]).Value : null,
                [LocationDetail.GridReference] = locationDetailToCopy.Contains(GetAliasLocationDetailFieldName(LocationDetail.GridReference)) ? ((AliasedValue)locationDetailToCopy[GetAliasLocationDetailFieldName(LocationDetail.GridReference)]).Value : null,
                [LocationDetail.Address] = locationDetailToCopy.Contains(GetAliasLocationDetailFieldName(LocationDetail.Address)) ? ((AliasedValue)locationDetailToCopy[GetAliasLocationDetailFieldName(LocationDetail.Address)]).Value : null,
                [LocationDetail.Owner] = locationDetailToCopy[Location.OwnerId],
            };

            // 2. Create and return new location detail 
            locationDetailEntity.Id = service.Create(locationDetailEntity);
            return locationDetailEntity;
        }

        /// <summary>
        /// Try to find location in the given array that matches the location name and publid interest fields 
        /// </summary>
        /// <param name="locationToFind">Location detail to check if exists in target array</param>
        /// <param name="locationsToSearch">Location detail we are checking to see if it exists</param>
        /// <returns>Matching location detail in target if found</returns>
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

        /// <summary>
        /// Try to find location detail in the given array that matches the location detail name, address and grid reference fields 
        /// </summary>
        /// <param name="locationToFind">Location to check if exists in target array</param>
        /// <param name="locationsToSearch">Location we are checking to see if it exists</param>
        /// <returns>Matching location in target if found</returns>
        private static Entity GetMatchingLocationDetail(Entity locationDetailToFind, Entity[] locationDetailsToSearch)
        {
            foreach (Entity locationDetailToSearch in locationDetailsToSearch)
            {

                if (MatchesAttribute(locationDetailToSearch, locationDetailToFind, Location.Name)
                    && MatchesAttribute(locationDetailToSearch, locationDetailToFind, Location.HighPublicInterest)
                    && MatchesAttribute(locationDetailToSearch, locationDetailToFind, GetAliasLocationDetailFieldName(LocationDetail.Name))
                    && MatchesAttribute(locationDetailToSearch, locationDetailToFind, GetAliasLocationDetailFieldName(LocationDetail.Address))
                    && MatchesAttribute(locationDetailToSearch, locationDetailToFind, GetAliasLocationDetailFieldName(LocationDetail.GridReference)))
                {
                    return locationDetailToSearch;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns an aliased Location Detail field name used throughout this class
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static string GetAliasLocationDetailFieldName(string fieldName)
        {
            return LocationDetail.Alias + "." + fieldName;
        }

        /// <summary>
        /// Checks if the two entities have an attribute that matches in value
        /// </summary>
        /// <param name="entity1">First entity</param>
        /// <param name="entity2">Second Entity</param>
        /// <param name="attributeName">Attribute to check</param>
        /// <returns>True if value matches, otherwise false</returns>
        private static bool MatchesAttribute(Entity entity1, Entity entity2, string attributeName)
        {
            var entityAttribute1 = entity1.Contains(attributeName) ? entity1[attributeName] : null;
            var entityAttribute2 = entity2.Contains(attributeName) ? entity2[attributeName] : null;

            if (entityAttribute1 == null && entityAttribute2 == null)
            {
                return true;
            }

            if (entityAttribute1 != null && entityAttribute1.Equals(entityAttribute2))
            {
                return true;
            }

            var aliasedEntityAttribute1 = entityAttribute1 as AliasedValue;
            var aliasedEntityAttribute2 = entityAttribute1 as AliasedValue;
            if (aliasedEntityAttribute1 != null && aliasedEntityAttribute1.Value.Equals(aliasedEntityAttribute2.Value))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Unlinks a permit location from a permit
        /// </summary>
        /// <param name="service">CRM Organisation Service</param>
        /// <param name="locationId">location entity to unlink from a permit</param>
        private static void DeactivatePermitLocation(IOrganizationService service, Guid locationId)
        {
            SetStateRequest state = new SetStateRequest();
            state.State = new OptionSetValue((int)defra_locationState.Inactive);
            state.Status = new OptionSetValue((int)defra_location_StatusCode.Inactive);
            state.EntityMoniker = new EntityReference(defra_location.EntityLogicalName, locationId);
            service.Execute(state);
        }

        /// <summary>
        /// Unlinks a location detail record from it's location
        /// </summary>
        /// <param name="service">CRM Org service</param>
        /// <param name="locationDetailId">Location Detail record id to be unlinked from it's location</param>
        private static void DeactivatePermitLocationDetail(IOrganizationService service, Guid locationDetailId)
        {
            SetStateRequest state = new SetStateRequest();
            state.State = new OptionSetValue((int)defra_locationdetailsState.Inactive);
            state.Status = new OptionSetValue((int)defra_locationdetails_StatusCode.Inactive);
            state.EntityMoniker = new EntityReference(defra_locationdetails.EntityLogicalName, locationDetailId);
            service.Execute(state);
        }


        /// <summary>
        /// Returns the number of applications linked to a permit, with an optional status filter
        /// </summary>
        /// <param name="service">CRM Organisation service</param>
        /// <param name="permitId">Permit Guid</param>
        /// <param name="filterByStatusCodes">Optional filter applicationstatus codes </param>
        /// <returns>Number of applications found</returns>
        public static int GetCountForApplicationsLinkedToPermit(IOrganizationService service, Guid permitId, defra_application_StatusCode[] filterByStatusCodes = null)
        {
            // Query CRM for all applications linked to a permit
            QueryExpression qe = new QueryExpression(defra_application.EntityLogicalName);
            qe.ColumnSet = new ColumnSet(Application.StatusCode, Application.Name);
            qe.Criteria.AddCondition(Application.Permit, ConditionOperator.Equal, permitId);
            qe.Criteria.FilterOperator = LogicalOperator.And;

            // And optionally, filter by status
            if (filterByStatusCodes != null && filterByStatusCodes.Length > 0)
            {
                foreach (var filterStatusCode in filterByStatusCodes)
                {
                    qe.Criteria.AddCondition(Application.StatusCode, ConditionOperator.NotEqual, (int)filterStatusCode);
                }
            }

            // Call CRM
            EntityCollection result = service.RetrieveMultiple(qe);

            // Return count
            if (result?.Entities != null)
            {
                return result.Entities.Count;
            }
            return 0;
        }

        /// <summary>
        /// Recalculates the application balance fields
        /// </summary>
        /// <param name="service">CRM service</param>
        /// <param name="tracingService">CRM Tracing service</param>
        /// <param name="applicationId">Id for application to recalculate fields on</param>
        public static void RecalculateApplicationBalances(IOrganizationService service, ITracingService tracingService, Guid applicationId)
        {
            tracingService.Trace($"RecalculateApplicationBalances() Started using application id: {applicationId}");

            // 1. Validation - Check the application is still active
            Entity application = service.Retrieve(
                defra_application.EntityLogicalName,
                applicationId,
                new ColumnSet(
                    Application.State,
                    Application.BalanceRefunds,
                    Application.BalanceLineItems,
                    Application.BalancePayments,
                    Application.Balance));

            if (application == null)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, "Could not recalculate application balances, could not find application with id " + applicationId);
            }

            if (!application.Contains(Application.State) || ((OptionSetValue)application[Application.State]).Value != (int)defra_applicationState.Active)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, "Could not recalculate application balances as application is no longer active, application id:  " + applicationId);
            }

            // 2. Get the Application Line Values
            tracingService.Trace($"RecalculateApplicationBalances() Getting application lines");
            EntityCollection applicationLines = DataAccessApplicationLine.GetApplicationLineValues(service, applicationId);

            // 3. Get the Payment Values
            tracingService.Trace($"RecalculateApplicationBalances() Getting completed payments");
            EntityCollection payments = DataAccessPayments.GetPaymentValues(service, applicationId, new [] {defra_payment_StatusCode.RefundIssued, defra_payment_StatusCode.PaymentComplete});

            // 4. Calculate Balances
            Decimal balanceTotal;
            Decimal balanceApplicationLines = 0;
            Decimal balanceIncomingPayments = 0;
            Decimal balanceOutgoingPayments = 0;

            // Application Line Balance
            tracingService.Trace($"RecalculateApplicationBalances() Summing up application lines");
            if (applicationLines?.Entities != null)
            {
                foreach (Entity applicationLine in applicationLines.Entities)
                {
                    if (applicationLine.Contains(ApplicationLine.Value) &&
                        applicationLine[ApplicationLine.Value] is Money)
                    {
                        Money appLineValue = (Money)applicationLine[ApplicationLine.Value];
                        balanceApplicationLines = balanceApplicationLines + appLineValue.Value;
                    }
                }
            }

            // Payment Balances
            tracingService.Trace($"RecalculateApplicationBalances() Summing up payments");
            if (payments?.Entities != null)
            {
                foreach (Entity payment in payments.Entities)
                {
                    if (payment.Contains(Payment.PaymentValue) &&
                        payment[Payment.PaymentValue] is Money)
                    {
                        Money paymentValue = (Money)payment[Payment.PaymentValue];

                        if (paymentValue == null)
                        {
                            // No payment value
                            continue;
                        }

                        // Added it to the incoming or outgoing payment balance
                        if (paymentValue.Value > 0)
                        {
                            balanceIncomingPayments = balanceIncomingPayments + paymentValue.Value;
                        }
                        else
                        {
                            balanceOutgoingPayments = balanceOutgoingPayments + paymentValue.Value;
                        }
                    }
                }
            }

            // Calculate total balance
            balanceTotal = balanceApplicationLines - balanceIncomingPayments - balanceOutgoingPayments;
            tracingService.Trace($"RecalculateApplicationBalances() balanceTotal={balanceTotal}, balanceApplicationLines={balanceApplicationLines}, balanceIncomingPayments={balanceIncomingPayments}, balanceOutgoingPayments={balanceOutgoingPayments}");

            // 5. Set-up a new application entity to be updated (so we update just the balance fields, and revers the refunds field for display purposes
            Entity applicationUpdateEntity = new Entity(application.LogicalName, application.Id);
            AddAttributeToTargetApplicationIfChanged(tracingService, application, applicationUpdateEntity, Application.BalanceLineItems, balanceApplicationLines);
            AddAttributeToTargetApplicationIfChanged(tracingService, application, applicationUpdateEntity, Application.BalancePayments, balanceIncomingPayments);
            AddAttributeToTargetApplicationIfChanged(tracingService, application, applicationUpdateEntity, Application.BalanceRefunds, balanceOutgoingPayments * -1);
            AddAttributeToTargetApplicationIfChanged(tracingService, application, applicationUpdateEntity, Application.Balance, balanceTotal);
            
            // 6. Update the application fields if required
            if (application.Attributes.Count > 0)
            {
                tracingService.Trace("RecalculateApplicationBalances() Updating application...");
                service.Update(applicationUpdateEntity);
            }
            else
            {
                tracingService.Trace("RecalculateApplicationBalances() Not updating application...");
            }

            tracingService.Trace("RecalculateApplicationBalances() End.");
        }

        /// <summary>
        /// Helper function that adds attributes to the applicationToUpdate entity if
        /// the new value is different to the value held by originalApplication
        /// </summary>
        /// <param name="tracingService">CRM Tracing service</param>
        /// <param name="originalApplication">Application with original values</param>
        /// <param name="applicationToUpdate">Application that will be used to updated CRM</param>
        /// <param name="attributeName">Attribute being updated</param>
        /// <param name="newValue">New value</param>
        private static void AddAttributeToTargetApplicationIfChanged(ITracingService tracingService, Entity originalApplication, Entity applicationToUpdate, string attributeName, decimal newValue)
        {
            // Only update if attribute not set in source, or source needs updating
            if (!originalApplication.Contains(attributeName)
                || (originalApplication[attributeName] is Money
                    && ((Money) originalApplication[attributeName]).Value != newValue))
            {
                applicationToUpdate.Attributes.Add(attributeName, new Money(newValue));
                tracingService.Trace($"Updating {attributeName} = {newValue}");
            }
        }
    }
}