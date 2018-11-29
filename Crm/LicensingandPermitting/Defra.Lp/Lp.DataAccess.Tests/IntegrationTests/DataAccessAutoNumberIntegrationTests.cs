//Data Access integration tests

using Lp.TestSupport.Mock;

namespace Lp.DataAccess.Tests.IntegrationTests
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
    /// Main integration tests class
    /// </summary>
    [TestClass]
    public class DataAccessAutoNumberIntegrationTests
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
        public void Integration_TestEAWMLAutoNumberSuccess()
        {
            DataAccessAutoNumber dal = new DataAccessAutoNumber(OrganizationService, new MockTracingService());
            string nextNumber = dal.GetNextPermitNumber("EAWML");
            Assert.IsTrue(nextNumber.Length == 11);
        }
        
        #endregion
    }
}
