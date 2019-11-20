using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;
using Defra.Lp.WastePermits.Workflows;
using System.Net;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Lp.Model.Crm;
using WastePermits.Model.EarlyBound;

namespace FakeXrmEasyTestProject
{
    [TestClass]
    public class DeleteApplicationLinesGivenLineTypeTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(DeleteApplicationLinesGivenLineType).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();
           inputs.Add("GetLineType", 910400003);

            //9569808e-fd09-ea11-a811-000d3a649fc7

            var soleId = Guid.Parse("a99288d1-9407-ea11-a811-000d3a44a237");
            var mainApp = new Entity("defra_application", soleId);

            var result = context.ExecuteCodeActivity<DeleteApplicationLinesGivenLineType>
    (mainApp, inputs);
        }
    }
}
