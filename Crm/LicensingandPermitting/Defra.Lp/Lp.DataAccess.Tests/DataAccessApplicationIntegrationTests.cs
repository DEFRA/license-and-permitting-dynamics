
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


        [TestMethod]
        public void TestMethod1()
        {

            var connector = new OrganisationServiceConnector();
            var proxy = connector.GetOrganizationServiceProxy();
            var service = (IOrganizationService)proxy;

            // 1. Create Application
            Entity newApplication = CreateApplication(service);

            // 2. Create Application Line
            //Guid newApplicationLineId = CreatetApplicationLine(service, newApplicationId);


            // 3. Create Application Location
            Guid applicationLocationId = CreateApplicationLocation(service, newApplication.Id);

            // 4. Create Permit 
            Guid permitId = CreatePermit(service);

            // 5. Update Application Permit Lookup field
            UpdateApplicationPermitLookup(service, newApplication, permitId);

            // 5.1 Test GetApplicationSites
            var sites = service.GetApplicationSites(newApplication.Id);

            // 6. Call MirrorApplicationSitesToPermit
            service.MirrorApplicationSitesToPermit(newApplication.Id);
        }

        private void UpdateApplicationPermitLookup(IOrganizationService service, Entity applicationEntity, Guid permitId)
        {
            applicationEntity[Application.Permit] = new EntityReference(Permit.EntityLogicalName, permitId);
            service.Update(applicationEntity);
        }

        private Guid CreatePermit(IOrganizationService service)
        {
            Entity entity = new Entity(Permit.EntityLogicalName)
            {
                [Permit.PermitNumber] = "ITEST",
                [Permit.Name] = "Integration Test " + DateTime.Now
            };
            return service.Create(entity);
        }

        private Guid CreateApplicationLocation(IOrganizationService service, Guid newApplicationId)
        {
            // 1. Create Location
            Entity locationEntity = new Entity(Location.EntityLogicalName)
            {
                [Location.Application] = new EntityReference(Application.EntityLogicalName, newApplicationId),
                [Location.Name] = "Integration Test " + DateTime.Now
            };
            Guid locationId = service.Create(locationEntity);

            // 2. Create Location Detail
            Entity locationDetailEntity = new Entity(LocationDetail.EntityLogicalName)
            {
                [LocationDetail.Location] = new EntityReference(Location.EntityLogicalName, locationId),
                [Location.Name] = "Integration Test " + DateTime.Now
            };
            service.Create(locationDetailEntity);

            return locationId;
        }

        /*
        private Guid CreatetApplicationLine(IOrganizationService service, Guid newApplicationId)
        {

        }
        */
        private static Entity CreateApplication(IOrganizationService service)
        {
            Entity newApplicationEntity =
                new Entity(Application.EntityLogicalName)
                {
                    [Application.Name] = "Integration Test " + DateTime.Now
                };
            Guid newApplicationId = service.Create(newApplicationEntity);

            newApplicationEntity.Id = newApplicationId;
            return newApplicationEntity;
        }
    }
}
