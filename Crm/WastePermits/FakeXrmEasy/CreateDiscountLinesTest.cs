using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;
using Defra.Lp.WastePermits.Workflows;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Net;

namespace FakeXrmEasyTestProject
{
    [TestClass]
    public class CreateDiscountLinesTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(CreateDiscountLines).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();


            //9569808e-fd09-ea11-a811-000d3a649fc7

            var soleId = Guid.Parse("2653dedd-8316-ea11-a811-000d3a44a2a9");
            var mainApp = new Entity("defra_application", soleId);

            var result = context.ExecuteCodeActivity<CreateDiscountLines>
    (mainApp, inputs);

        }
    }
}
