using Lp.Model.EarlyBound;

namespace Lp.DataAccess.Tests.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using Connector;
    using Model.Crm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Crm.Sdk.Messages;

    [TestClass]
    public class DataAccessApplicationIntegrationTests
    {


        private static IOrganizationService _organizationService;
        private static IOrganizationService OrganizationService
        {
            get
            {
                if (_organizationService == null)
                {
                    var connector = new OrganisationServiceConnector();
                    var proxy = connector.GetOrganizationServiceProxy();
                    _organizationService = (IOrganizationService)proxy;
                }
                return _organizationService;
            }
        }

        private KeyValuePair<string, Guid> recordsToDelete = new KeyValuePair<string, Guid>();

        #region Tests


        [TestMethod]
        public void Integration_MirrorNewApplicationToPermit_Success()
        {
            var service = OrganizationService;
            Guid permitId = CreateApplicationAndPermit(service, 2, 3);

            // 4. Check the Permit now has all the original locations + the new ones
            Entity[] permitLocationAndDetails = DataAccessApplication.GetLocationAndLocationDetails(OrganizationService, null, permitId);
            Assert.IsTrue(permitLocationAndDetails.Length == 6);
        }

        [TestMethod]
        public void Integration_MirrorPermitToVariation_Success()
        {
            // 1. Create application and permit
            Guid permitId = CreateApplicationAndPermit(OrganizationService, 2, 5);

            // 2. Create Variation
            Entity variationApplication = CreateApplication(OrganizationService, ApplicationTypes.Variation, permitId);

            // 3. Call MirrorApplicationSitesToPermit
            DataAccessApplication.MirrorPermitLocationsAndDetailsToApplication(OrganizationService,variationApplication.Id);

            // 4. Check the Application now has all the Permit locations
            Entity[] applicationLocationAndDetails = DataAccessApplication.GetLocationAndLocationDetails(OrganizationService, variationApplication.Id, null);
            Assert.IsTrue(applicationLocationAndDetails.Length == 10);
        }

        [TestMethod]
        public void Integration_MirrorVariationToPermit_AddLocations_Success()
        {
            int startSites = 2;
            int startDetailsPerSite = 6;
            int additionalSites = 3;
            int additionalDetails = 2;

            Guid permitId = CreateApplicationAndPermit(OrganizationService, startSites, startDetailsPerSite);

            // 1. Create Application
            Entity variationApplication = CreateApplication(OrganizationService, ApplicationTypes.Variation, permitId);

            // 2. Call MirrorApplicationSitesToPermit
            DataAccessApplication.MirrorPermitLocationsAndDetailsToApplication(OrganizationService, variationApplication.Id);

            // 3. Add locations to Application

            for (int countSites = 0; countSites < additionalSites; countSites++)
            {
                CreateApplicationLocationAndDetails(OrganizationService, variationApplication.Id, "Additional Location " + additionalSites, additionalDetails);
            }

            // 6. Call MirrorApplicationSitesToPermit
            DataAccessApplication.MirrorApplicationLocationsAndDetailsToPermit(OrganizationService, variationApplication.Id);


            // 4. Check the Permit now has all the original locations + the new ones
            Entity[] permitLocationAndDetails = DataAccessApplication.GetLocationAndLocationDetails(OrganizationService, null, permitId);
            Assert.IsTrue(permitLocationAndDetails.Length == startSites * startDetailsPerSite + additionalSites * additionalDetails);
        }



        [TestMethod]
        public void Integration_MirrorVariationToPermit_RemoveLocations_Success()
        {
            int startSites = 5;
            int startDetailsPerSite = 2;


            Guid permitId = CreateApplicationAndPermit(OrganizationService, startSites, startDetailsPerSite);

            // 1. Create Application
            Entity variationApplication = CreateApplication(OrganizationService, ApplicationTypes.Variation, permitId);

            // 2. Call MirrorApplicationSitesToPermit
            DataAccessApplication.MirrorPermitLocationsAndDetailsToApplication(OrganizationService, variationApplication.Id);

            // 3. Remove 1 location from Application
            Entity[] applicationLocationAndDetails = DataAccessApplication.GetLocationAndLocationDetails(OrganizationService, variationApplication.Id, null);

            DeactivateLocation(OrganizationService, applicationLocationAndDetails[0]);

            // 6. Call MirrorApplicationSitesToPermit
            DataAccessApplication.MirrorApplicationLocationsAndDetailsToPermit(OrganizationService, variationApplication.Id);

            // 7. Check the Permit now has had a location removed
            Entity[] permitLocationAndDetails = DataAccessApplication.GetLocationAndLocationDetails(OrganizationService, null, permitId);
            Assert.IsTrue(permitLocationAndDetails.Length == startSites * startDetailsPerSite - startDetailsPerSite);
        }

        private void DeactivateLocation(IOrganizationService service, Entity applicationLocationAndDetail)
        {
            // Create the Request Object
            SetStateRequest state = new SetStateRequest();

            // Set the Request Object's Properties
            state.State = new OptionSetValue((int)defra_locationState.Inactive);
            state.Status = new OptionSetValue((int)defra_location_StatusCode.Inactive);

            // Point the Request to the case whose state is being changed
            state.EntityMoniker = applicationLocationAndDetail.ToEntityReference();

            // Execute the Request
            service.Execute(state);
        }

        #endregion

        #region Supporting Functions


        private Guid CreateApplicationAndPermit(IOrganizationService service, int numberOfSites, int numberOfSiteDetails)
        {
            // 1. Create Application
            Entity newApplication = CreateApplication(service);

            // 2. Create Application Line
            //Guid newApplicationLineId = CreatetApplicationLine(service, newApplicationId);

            // 3. Create Application Locations
            for (int count = 0; count < numberOfSites; count++)
            {
                CreateApplicationLocationAndDetails(service, newApplication.Id, "Main Location " + count, numberOfSiteDetails);
            }

            // 4. Create Permit 
            Entity application = OrganizationService.Retrieve(Application.EntityLogicalName, newApplication.Id,
                new ColumnSet(Application.PermitNumber));
            string permitNumber = application[Application.PermitNumber].ToString();

            Guid permitId = CreatePermit(service, permitNumber);

            // 5. Update Application Permit Lookup field
            UpdateApplicationPermitLookup(service, newApplication, permitId);

            // 6. Call MirrorApplicationSitesToPermit
            DataAccessApplication.MirrorApplicationLocationsAndDetailsToPermit(OrganizationService,newApplication.Id);

            // 5.1 Test GetApplicationSites
            // var sites = service.GetLocationAndLocationDetails(newApplication.Id, null);

            return permitId;
        }

        private void UpdateApplicationPermitLookup(IOrganizationService service, Entity applicationEntity, Guid permitId)
        {
            applicationEntity[Application.Permit] = new EntityReference(Permit.EntityLogicalName, permitId);
            service.Update(applicationEntity);
        }

        private Guid CreatePermit(IOrganizationService service, string permitNumber)
        {
            Entity entity = new Entity(Permit.EntityLogicalName)
            {
                [Permit.PermitNumber] = permitNumber,
                [Permit.Name] = "Integration Test " + DateTime.Now
            };
            return service.Create(entity);
        }

        private Guid CreateApplicationLocationAndDetails(IOrganizationService service, Guid newApplicationId, string locationName, int locationDetailCount)
        {

            // 1. Create Location
            Entity locationEntity = new Entity(Location.EntityLogicalName)
            {
                [Location.Application] = new EntityReference(Application.EntityLogicalName, newApplicationId),
                [Location.Name] = locationName
            };
            Guid locationId = service.Create(locationEntity);

            CreateApplicationLocationDetails(service, locationDetailCount, locationId);


            return locationId;
        }

        private static void CreateApplicationLocationDetails(IOrganizationService service, int locationDetailCount,
            Guid locationId)
        {
            // 2. Create Location Detail
            for (int count = 0; count < locationDetailCount; count++)
            {
                Entity locationDetailEntity = new Entity(LocationDetail.EntityLogicalName)
                {
                    [LocationDetail.Location] = new EntityReference(Location.EntityLogicalName, locationId),
                    [Location.Name] = "Integration Test " + DateTime.Now,
                    [LocationDetail.GridReference] = "ST-10000000" + count
                };
                service.Create(locationDetailEntity);
            }
        }

        /*
        private Guid CreatetApplicationLine(IOrganizationService service, Guid newApplicationId)
        {

        }
        */

        private static Entity CreateApplication(IOrganizationService service, ApplicationTypes applicationType = ApplicationTypes.NewApplication, Guid? permitId = null)
        {
            Entity newApplicationEntity =
                new Entity(Application.EntityLogicalName)
                {
                    [Application.Name] = "Integration Test " + DateTime.Now,
                    [Application.ApplicationType] = new OptionSetValue((int)applicationType),
                    [Application.Permit] = permitId.HasValue ? new EntityReference(Permit.EntityLogicalName, permitId.Value) : null
                };
            Guid newApplicationId = service.Create(newApplicationEntity);

            newApplicationEntity.Id = newApplicationId;
            return newApplicationEntity;
        }

        #endregion

    }
}
