using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Defra.Lp.Workflows;
using FakeXrmEasy;
using System.Net;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace FakeXRMeasyTestProject
{
    [TestClass]
    public class ApplicationCreateFolderInSharePointTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(ApplicationCreateFolderInSharePoint).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();

           
           

            var ltdCompnayId = Guid.Parse("2DAEA1DC-C2A7-E911-A980-000D3A20838A");
            var mainApp = new Entity("defra_application", ltdCompnayId);

            inputs.Add("Application", new Entity("defra_application", ltdCompnayId).ToEntityReference());

            var result = context.ExecuteCodeActivity<ApplicationCreateFolderInSharePoint>
    (mainApp, inputs);
        }
    }
}
