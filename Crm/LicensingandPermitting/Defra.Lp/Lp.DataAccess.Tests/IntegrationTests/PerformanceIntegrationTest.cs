namespace Lp.DataAccess.Tests.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using Lp.TestSupport.Connector;
    using Model.Crm;
    using Model.EarlyBound;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Query;

    [TestClass]
    public class LateBoundIntegrationTests
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

        private static OrganizationServiceProxy _organizationServiceProxy;
        private static OrganizationServiceProxy OrganizationServiceProxy
        {
            get
            {
                if (_organizationServiceProxy == null)
                {
                    var connector = new OrganisationServiceConnector();
                    _organizationServiceProxy = connector.GetOrganizationServiceProxy();
                    _organizationServiceProxy.EnableProxyTypes();
                }
                return _organizationServiceProxy;
            }
        }

        private KeyValuePair<string, Guid> recordsToDelete = new KeyValuePair<string, Guid>();

        #region Tests

        [TestMethod]
        public void LateBoundCreateApplicationAndPermitSuccess1()
        {
            CreateApplication(OrganizationService, ApplicationTypes.NewApplication);
        }


        [TestMethod]
        public void EarlyBoundCreateApplicationAndPermitSuccess1()
        {
            OrganizationServiceContext orgContext = new OrganizationServiceContext(OrganizationServiceProxy);
            EarlyBoundCreateApplication(orgContext, defra_ApplicationType.NewApplication);
        }

        [TestMethod]
        public void EarlyBoundReadApplicationPermitTest()
        {
            OrganizationServiceContext orgContext = new OrganizationServiceContext(OrganizationServiceProxy);
        }

        [TestMethod]
        public void LateBoundCreateApplicationAndPermitSuccess2()
        {
            CreateApplication(OrganizationService, ApplicationTypes.NewApplication);
        }

        [TestMethod]
        public void EarlyBoundCreateApplicationAndPermitSuccess2()
        {
            OrganizationServiceContext orgContext = new OrganizationServiceContext(OrganizationServiceProxy);
            EarlyBoundCreateApplication(orgContext, defra_ApplicationType.NewApplication);
        }

        [TestMethod]
        public void LateBoundCreateApplicationAndPermitSuccess3()
        {
            CreateApplication(OrganizationService, ApplicationTypes.NewApplication);
        }

        [TestMethod]
        public void EarlyBoundCreateApplicationAndPermitSuccess3()
        {
            OrganizationServiceContext orgContext = new OrganizationServiceContext(OrganizationServiceProxy);
            EarlyBoundCreateApplication(orgContext, defra_ApplicationType.NewApplication);
        }

        [TestMethod]
        public void LateBoundCreateApplicationAndPermitSuccess4()
        {
            CreateApplication(OrganizationService, ApplicationTypes.NewApplication);
        }

        [TestMethod]
        public void EarlyBoundCreateApplicationAndPermitSuccess4()
        {
            OrganizationServiceContext orgContext = new OrganizationServiceContext(OrganizationServiceProxy);
            EarlyBoundCreateApplication(orgContext, defra_ApplicationType.NewApplication);
        }


        [TestMethod]
        public void LateBoundCreateApplicationAndPermitSuccess1000()
        {
            for (int count = 0; count < 100; count++)
            {
                CreateApplication(OrganizationService, ApplicationTypes.NewApplication);
            }
        }


        [TestMethod]
        public void EarlyBoundCreateApplicationAndPermitSuccess1000()
        {
            var connector = new OrganisationServiceConnector();
            var proxy = connector.GetOrganizationServiceProxy();
            proxy.EnableProxyTypes();
            for (int count = 0; count < 100; count++)
            {
                CreateApplication(OrganizationService, ApplicationTypes.NewApplication);
            }
        }

        #endregion

        #region Supporting Functions

        private Guid LateBoundCreateApplicationAndPermit(IOrganizationService service, int numberOfSiteDetails)
        {
            // 1. Create Application
            Entity newApplication = CreateApplication(service);

            // 3. Create Application Location
            Guid applicationLocationId = CreateApplicationLocationAndDetails(service, newApplication.Id, "Main Location", numberOfSiteDetails);

            // 4. Create Permit 
            Entity application = OrganizationService.Retrieve(Application.EntityLogicalName, newApplication.Id,
                new ColumnSet(Application.PermitNumber));
            string permitNumber = application[Application.PermitNumber].ToString();

            Guid permitId = CreatePermit(service, permitNumber);

            // 5. Update Application Permit Lookup field
            UpdateApplicationPermitLookup(service, newApplication, permitId);
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

        private static Entity EarlyBoundCreateApplication(OrganizationServiceContext service, defra_ApplicationType applicationType = defra_ApplicationType.NewApplication, Guid? permitId = null)
        {
            defra_application newApplicationEntity =
                new defra_application
                {
                    defra_name = "Integration Test " + DateTime.Now,
                    defra_applicationtype = new OptionSetValue((int)applicationType),
                    defra_permitId = permitId.HasValue ? new EntityReference(Permit.EntityLogicalName, permitId.Value) : null

                };

            service.AddObject(newApplicationEntity);
            service.SaveChanges();
            return newApplicationEntity;
        }
        #endregion
    }
}
