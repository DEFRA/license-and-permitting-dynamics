using System;
using System.Security.Policy;
using Core.DataAccess.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Lp.TestSupport.Connector;
using Lp.TestSupport.IntegrationTests;
using Core.Helpers.Extensions;
using System.Collections.Generic;
using Lp.Model.Crm;

namespace Lp.DataAccess.Tests.IntegrationTests
{
    [TestClass]
    public class CrmExtensionsIntegrationTests
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


        [TestMethod]
        public void Integration_GrantAccessSuccess()
        {
            Entity application = DataAccessIntegrationTestSupport.CreateApplication(OrganizationService);
            List<EntityReference> principals = new List<EntityReference>();
            EntityReference teamReference = new EntityReference(Team.EntityLogicalName, new Guid("1d038e0f-47c2-e711-80eb-3863bb357ff8"));
            principals.Add(teamReference);


            OrganizationService.GrantAccess(application.ToEntityReference(), principals, true, true, true, true, true, true, true);

            Assert.IsTrue(true);     
        }

        [TestMethod]
        public void Integration_RevokeAccessSuccess()
        {
            Entity application = DataAccessIntegrationTestSupport.CreateApplication(OrganizationService);
            List<EntityReference> principals = new List<EntityReference>();
            EntityReference teamReference = new EntityReference(Team.EntityLogicalName, new Guid("1d038e0f-47c2-e711-80eb-3863bb357ff8"));
            principals.Add(teamReference);

            OrganizationService.GrantAccess(application.ToEntityReference(), principals, true, true, true, true, true, true, true);

            OrganizationService.RevokeAccess(application.ToEntityReference(), principals);

            Assert.IsTrue(true);
        }
    }
}
