namespace Lp.Common.Tests.IntegrationTests
{
    using Core.Helpers.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
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
        public void Integration_CreateNoteAndFileTogetherAndUploadToSharePoint_Success()
        {
            var service = OrganizationService;
            var application = DataAccessIntegrationTestSupport.CreateApplication(OrganizationService, Model.Crm.ApplicationTypes.NewApplication);

            for (int i = 1; i < 50; i++)
            {
                // Create Annotation
                var annotationEntity = new Entity(Annotation.EntityLogicalName)
                {
                    [Annotation.Fields.ObjectId] = new EntityReference(defra_application.EntityLogicalName, application.Id),
                    [Annotation.Fields.Subject] = "Integration Test " + DateTime.Now,
                    [Annotation.Fields.FileName] = string.Format("File{0}.txt", i.ToString()),
                    [Annotation.Fields.DocumentBody] = "Test Document for integration test".Base64Encode()
                };
                service.Create(annotationEntity);
            }

            // Assert ???

        }

        [TestMethod]
        public void Integration_CreateNoteAndFileSeparatelyThenUploadToSharePoint_Success()
        {
            var service = OrganizationService;
            var application = DataAccessIntegrationTestSupport.CreateApplication(OrganizationService, Model.Crm.ApplicationTypes.NewApplication);
            var notes = new EntityCollection();

            for (int i = 1; i < 50; i++)
            {
                // Create Annotation
                var annotationEntity = new Entity(Annotation.EntityLogicalName)
                {
                    [Annotation.Fields.ObjectId] = new EntityReference(defra_application.EntityLogicalName, application.Id),
                    [Annotation.Fields.Subject] = string.Format("File {0} for integration test at {1}", i.ToString(), DateTime.Now)
                };
                annotationEntity[Annotation.Fields.Id] = service.Create(annotationEntity);
                notes.Entities.Add(annotationEntity);
            }

            var cnt = 1;
            foreach (var note in notes.Entities)
            {
                note[Annotation.Fields.FileName] = string.Format("File{0}.txt", cnt.ToString());
                note[Annotation.Fields.DocumentBody] = "Test Document for integration test".Base64Encode();
                service.Update(note);
                cnt++;
            }

            // Assert ???

        }

        #endregion

    }
}
