using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;
using Defra.Lp.WastePermits.Workflows;
using System.Net;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasyTestProject
{
    [TestClass]
    public class GetWasteCodesGivenApplictaionTest
    {
        [TestMethod]
        public void Test_valid_Not()
        {
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetWasteCodesGivenApplictaion).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();

            var ltdCompnayId = Guid.Parse("4f1a3a19-8c04-ea11-a811-000d3a44a8e9");

            var mainApp = new Entity("defra_application", ltdCompnayId);

            var result = context.ExecuteCodeActivity<GetWasteCodesGivenApplictaion>
    (mainApp, inputs);
        }
    

    [TestMethod]
    public void Test_Invalid_Not()
    {
        var context = new XrmRealContext
        {
            ProxyTypesAssembly = typeof(GetWasteCodesGivenApplictaion).Assembly,
            ConnectionStringName = "CRMOnline"
        };
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        var executionContext = context.GetDefaultWorkflowContext();

        //Inputs
        var inputs = new Dictionary<string, object>();

        var ltdCompnayId = Guid.Parse("c9a1300d-1cef-e811-a97a-000d3a233e06");

        var mainApp = new Entity("defra_application", ltdCompnayId);

        var result = context.ExecuteCodeActivity<GetWasteCodesGivenApplictaion>
(mainApp, inputs);
    }
}

}
