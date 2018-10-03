
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using Model.Lp.Crm;

namespace Lp.DataAccess.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Crm.Sdk.Messages;
    using System.Net;
    using System.ServiceModel.Description;

    

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
        public void MirrorNewApplicationToPermitSuccess()
        {
            var service = OrganizationService;
            CreateApplicationAndPermit(service, 5);
        }


        [TestMethod]
        public void MirrorPermitToVariationSuccess()
        {
            Guid permitId = CreateApplicationAndPermit(OrganizationService, 5);

            // 1. Create Application
            Entity variationApplication = CreateApplication(OrganizationService, ApplicationTypes.Variation, permitId);

            // 4. Call MirrorApplicationSitesToPermit
            OrganizationService.MirrorPermitSitesToApplication(variationApplication.Id);

        }



        [TestMethod]
        public void MirrorVariationToPermitSuccess()
        {
            Guid permitId = CreateApplicationAndPermit(OrganizationService, 5);

            // 1. Create Application
            Entity variationApplication = CreateApplication(OrganizationService, ApplicationTypes.Variation, permitId);

            // 2. Call MirrorApplicationSitesToPermit
            OrganizationService.MirrorPermitSitesToApplication(variationApplication.Id);

            // 3. Add locations to Application
            CreateApplicationLocationAndDetails(OrganizationService, variationApplication.Id, "Additinal Location 1", 3);
            CreateApplicationLocationAndDetails(OrganizationService, variationApplication.Id, "Additinal Location 2", 3);

            // 6. Call MirrorApplicationSitesToPermit
            OrganizationService.MirrorApplicationSitesToPermit(variationApplication.Id);
        }


        #endregion

        #region Supporting Functions


        private Guid CreateApplicationAndPermit(IOrganizationService service, int numberOfSiteDetails)
        {
            // 1. Create Application
            Entity newApplication = CreateApplication(service);

            // 2. Create Application Line
            //Guid newApplicationLineId = CreatetApplicationLine(service, newApplicationId);


            // 3. Create Application Location
            Guid applicationLocationId = CreateApplicationLocationAndDetails(service, newApplication.Id, "Main Location", numberOfSiteDetails);

            // 4. Create Permit 
            Entity application = OrganizationService.Retrieve(Application.EntityLogicalName, newApplication.Id,
                new ColumnSet(Application.PermitNumber));
            string permitNumber = application[Application.PermitNumber].ToString();

            Guid permitId = CreatePermit(service, permitNumber);

            // 5. Update Application Permit Lookup field
            UpdateApplicationPermitLookup(service, newApplication, permitId);

            // 5.1 Test GetApplicationSites
            var sites = service.GetSites(newApplication.Id, null);

            // 6. Call MirrorApplicationSitesToPermit
            service.MirrorApplicationSitesToPermit(newApplication.Id);

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
