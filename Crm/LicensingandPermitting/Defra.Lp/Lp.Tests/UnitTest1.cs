using System;
using System.ServiceModel.Description;
using Microsoft.Crm.Sdk.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Client;

namespace Lp.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreatePayment()
        {
            var config = new ServerConnection.Configuration();
            config.EndpointType = AuthenticationProviderType.LiveId;
            config.ServerAddress = "https://ag-ea-lp-dev-corelp.crm4.dynamics.com";

            var conn = ServerConnection.GetOrganizationProxy(new ServerConnection.Configuration());

        }
    }
}
