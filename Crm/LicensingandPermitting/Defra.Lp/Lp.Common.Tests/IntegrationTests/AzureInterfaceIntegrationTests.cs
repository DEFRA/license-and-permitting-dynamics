namespace Lp.Common.Tests.IntegrationTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Model.Crm;
    using Model.EarlyBound;
    using System;
    using TestSupport.Connector;
    using TestSupport.IntegrationTests;

    /// <summary>
    /// Integration tests for SharePoint Logic Apps
    /// </summary>
    [TestClass]
    public class AzureInterfaceIntegrationTests
    {
        #region Test Setup

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

        private readonly DataAccessIntegrationTestSupport _dataAccessIntegrationTestSupport = new DataAccessIntegrationTestSupport();

        #endregion

        #region Tests

        [TestMethod]
        public void Integration_UploadAnnotationFileToSharePoint_Success()
        {
            var service = OrganizationService;
            Entity application = DataAccessIntegrationTestSupport.CreateApplication(OrganizationService, ApplicationTypes.NewApplication);

            // Create Annotation
            Entity annotationEntity = new Entity(Annotation.EntityLogicalName)
            {
                [Annotation.RegardingObjectId] = new EntityReference(Application.EntityLogicalName, application.Id),
                [Annotation.Subject] = "Integration Test " + DateTime.Now
            };

            // Update Annotation with file


            // Assert. How to test Async???

        }

        #endregion

    }
}
