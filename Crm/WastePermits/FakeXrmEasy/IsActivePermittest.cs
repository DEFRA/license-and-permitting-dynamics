using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Defra.Lp.WastePermits.Workflows;
using FakeXrmEasy;
using System.Net;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasyTestProject
{
    [TestClass]
    public class IsActivePermittest
    {
        [TestMethod]
        public void Test_WithNot_Valid_Application()
        {
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetValidPermitApplication).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();
            inputs.Add("GetPermit", new EntityReference("defra_permit",new Guid("DE129E7E-ECB1-E911-A98A-000D3A233E06")));

            var application = Guid.Parse("1ECB5256-E9B1-E911-A980-000D3A206976");

            var mainApp = new Entity("defra_application", application);

            var result = context.ExecuteCodeActivity<GetValidPermitApplication>
    (mainApp, inputs); //92622D64-FEB1-E911-A98A-000D3A233E06
        }

        [TestMethod]
        public void Test_With_Valid_Application()
        {
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetValidPermitApplication).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();
            inputs.Add("GetPermit", new EntityReference("defra_permit", new Guid("DE129E7E-ECB1-E911-A98A-000D3A233E06")));

            var application = Guid.Parse("92622D64-FEB1-E911-A98A-000D3A233E06");

            var mainApp = new Entity("defra_application", application);

            var result = context.ExecuteCodeActivity<GetValidPermitApplication>
    (mainApp, inputs); //
        }
    }
}
