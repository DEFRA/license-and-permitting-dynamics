using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;
using Defra.Lp.Workflows;
using System.Net;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace FakeXRMeasyTestProject
{
    [TestClass]
    public class UploadEmailAndAttachmentsToSharePointTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(UploadEmailAndAttachmentsToSharePoint).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();

            // master var mailId = Guid.Parse("575e588c-4ea2-e911-aa05-000d3a2065c5");
            //0ec946a0-fcf3-e911-a811-000d3a649465

            var mailId = Guid.Parse("d667072e-dfff-e911-a811-000d3a64905b");
            var mainApp = new Entity("email", mailId);

            //inputs.Add("Application", new Entity("defra_application", mailId).ToEntityReference());

            var result = context.ExecuteCodeActivity<UploadEmailAndAttachmentsToSharePoint>
    (mainApp, inputs);
        }
    }
}
