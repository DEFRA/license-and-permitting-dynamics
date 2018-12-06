//Data Access App Answer integration tests
using Lp.TestSupport.Mock;

namespace Lp.DataAccess.Tests.IntegrationTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
    using System;
    using TestSupport.Connector;
    using TestSupport.IntegrationTests;

    /// <summary>
    /// Main integration tests class
    /// </summary>
    [TestClass]
    public class DataAccessApplicationAnswersIntegrationTests
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
                    _organizationService = connector.GetOrganizationServiceProxy();
                }
                return _organizationService;
            }
        }

        #endregion

        #region Tests

        [TestMethod]
        public void Integration_TestSetApplicationAnswerSuccess()
        {
            Entity application = DataAccessIntegrationTestSupport.CreateApplication(OrganizationService);

            DataAccessApplicationAnswers dal = new DataAccessApplicationAnswers(OrganizationService, new MockTracingService());

            // Create
            Guid answedId1= dal.SetApplicationAnswer("test-question-1", "answer-1", "", application.ToEntityReference(), null, true);

            // Update
            Guid answedId2 = dal.SetApplicationAnswer("test-question-1", "answer-2", "", application.ToEntityReference(), null, true);

            // Create
            Guid answedId3 = dal.SetApplicationAnswer("test-question-2", "answer-3", "", application.ToEntityReference(), null, true);

            // Update
            Guid answedId4 = dal.SetApplicationAnswer("test-question-2", "answer-4", "Answer Text", application.ToEntityReference(), null, true);

            Assert.IsTrue(answedId1 != Guid.Empty);
            Assert.IsTrue(answedId2 != Guid.Empty);
            Assert.IsTrue(answedId3 != Guid.Empty);
            Assert.IsTrue(answedId4 != Guid.Empty);
        }
        
        #endregion
    }
}
