using System;
using Lp.Model.Crm;
using Lp.Model.EarlyBound;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Lp.DataAccess.Tests.IntegrationTests
{
    public class DataAccessIntegrationTestSupport
    {
        public void DeactivateLocation(IOrganizationService service, Entity applicationLocationAndDetail)
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

        public Guid CreateApplicationAndPermit(IOrganizationService service, int numberOfSites, int numberOfSiteDetails)
        {
            // 1. Create Application
            Entity newApplication = CreateApplication(service);

            // 2. Create Application Line
            //Guid newApplicationLineId = CreatetApplicationLine(service, newApplicationId);

            // 3. Create Application Locations
            for (int count = 0; count < numberOfSites; count++)
            {
                CreateApplicationLocationAndDetails(service, newApplication.Id, "Main Location " + count, numberOfSiteDetails, count > 1 ? true : false);
            }

            // 4. Create Permit 
            Entity application = service.Retrieve(Application.EntityLogicalName, newApplication.Id,
                new ColumnSet(Application.PermitNumber));
            string permitNumber = application[Application.PermitNumber].ToString();

            Guid permitId = CreatePermit(service, permitNumber);

            // 5. Update Application Permit Lookup field
            UpdateApplicationPermitLookup(service, newApplication, permitId);

            // 6. Call MirrorApplicationSitesToPermit
            DataAccessApplication.MirrorApplicationLocationsAndDetailsToPermit(service, newApplication.Id);

            // 5.1 Test GetApplicationSites
            // var sites = service.GetLocationAndLocationDetails(newApplication.Id, null);

            return permitId;
        }

        public void UpdateApplicationPermitLookup(IOrganizationService service, Entity applicationEntity, Guid permitId)
        {
            applicationEntity[Application.Permit] = new EntityReference(Permit.EntityLogicalName, permitId);
            service.Update(applicationEntity);
        }

        public Guid CreatePermit(IOrganizationService service, string permitNumber)
        {
            Entity entity = new Entity(Permit.EntityLogicalName)
            {
                [Permit.PermitNumber] = permitNumber,
                [Permit.Name] = "Integration Test " + DateTime.Now
            };
            return service.Create(entity);
        }

        public Guid CreateApplicationLocationAndDetails(IOrganizationService service, Guid newApplicationId, string locationName, int locationDetailCount, bool createAddress)
        {

            // 1. Create Location
            Entity locationEntity = new Entity(Location.EntityLogicalName)
            {
                [Location.Application] = new EntityReference(Application.EntityLogicalName, newApplicationId),
                [Location.Name] = locationName
            };
            Guid locationId = service.Create(locationEntity);

            CreateApplicationLocationDetails(service, locationDetailCount, locationId, createAddress);


            return locationId;
        }

        public static void CreateApplicationLocationDetails(IOrganizationService service, int locationDetailCount, Guid locationId, bool createAddress)
        {
            // 2. Create Location Detail
            for (int count = 0; count < locationDetailCount; count++)
            {
                Entity locationDetailEntity = new Entity(LocationDetail.EntityLogicalName)
                {
                    [LocationDetail.Location] = new EntityReference(Location.EntityLogicalName, locationId),
                    [LocationDetail.Name] = "Integration Test " + DateTime.Now
                   
                };

                if (createAddress)
                {
                    Guid addressId = CreateAddress(service);
                    locationDetailEntity.Attributes.Add(LocationDetail.Address,
                        new EntityReference(Address.EntityLogicalName, addressId));
                }
                else
                {
                    locationDetailEntity.Attributes.Add(LocationDetail.GridReference, "ST-10000000" + count);
                }
                service.Create(locationDetailEntity);
            }
        }

        public static Guid CreateAddress(IOrganizationService service)
        {
            // Create Address

            Entity locationDetailEntity = new Entity(Address.EntityLogicalName)
            {
                [Location.Name] = "Address " + DateTime.Now
            };
            return service.Create(locationDetailEntity);
        }

        public static Entity CreateApplication(IOrganizationService service, ApplicationTypes applicationType = ApplicationTypes.NewApplication, Guid? permitId = null)
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
    }
}