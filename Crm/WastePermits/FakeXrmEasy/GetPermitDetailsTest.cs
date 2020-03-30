using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Defra.Lp.WastePermits.Workflows;
using FakeXrmEasy;
using System.Net;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace FakeXrmEasyTestProject
{
    [TestClass]
    public class GetPermitDetailsTest
    {
        [TestMethod]
        public void TestMethod_When_bespoke()
        {
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetPermitDetails).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();

            inputs.Add("Application", new EntityReference("defra_application", Guid.Parse("67810b75-b420-ea11-a810-000d3a44a237")));
            inputs.Add("Permit", new EntityReference("defra_permit", null));
            //9569808e-fd09-ea11-a811-000d3a649fc7

            var soleId = Guid.Parse("D8307C83-940B-EA11-A811-000D3ABAC0B9");
            var mainApp = new Entity("defra_application", soleId);

            var result = context.ExecuteCodeActivity<GetPermitDetails>
    (mainApp, inputs);
        }

        [TestMethod]
        public void TestMethod_When_SR()
        {
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetPermitDetails).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();

            inputs.Add("Application", new EntityReference("defra_application", Guid.Parse("95ea96a2-b57c-e911-a97c-000d3a23443b")));
            inputs.Add("Permit", new EntityReference("defra_permit", null));
            //9569808e-fd09-ea11-a811-000d3a649fc7

            var soleId = Guid.Parse("7801c90f-3b15-ea11-a811-000d3a44a237");
            var mainApp = new Entity("defra_application", soleId);

            var result = context.ExecuteCodeActivity<GetPermitDetails>
    (mainApp, inputs);
        }

        [TestMethod]
        public void TestMethod_When_SR_Application_Is_NULL()
        {
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetPermitDetails).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();

            inputs.Add("Application", null);
            inputs.Add("Permit", new EntityReference("defra_permit", Guid.Parse("801645b8-12e5-e911-a812-000d3a44a2a9")));
            //9569808e-fd09-ea11-a811-000d3a649fc7

            var soleId = Guid.Parse("7801c90f-3b15-ea11-a811-000d3a44a237");
            var mainApp = new Entity("defra_application", soleId);

            var result = context.ExecuteCodeActivity<GetPermitDetails>
    (mainApp, inputs);
        }

        [TestMethod]
        public void TestMethod_When_Bespoke_Application_Is_NULL()
        {
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetPermitDetails).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();

            inputs.Add("Application", null);
            inputs.Add("Permit", new EntityReference("defra_permit", Guid.Parse("937f3266-db4c-ea11-a812-000d3a44ade8")));
            //9569808e-fd09-ea11-a811-000d3a649fc7

            var soleId = Guid.Parse("7801c90f-3b15-ea11-a811-000d3a44a237");
            var mainApp = new Entity("defra_application", soleId);

            var result = context.ExecuteCodeActivity<GetPermitDetails>
    (mainApp, inputs);
        }
    }
}
