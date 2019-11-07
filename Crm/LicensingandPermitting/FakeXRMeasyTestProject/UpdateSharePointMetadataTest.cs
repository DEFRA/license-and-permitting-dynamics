using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using Defra.Lp.Workflows;
using FakeXrmEasy;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace FakeXRMeasyTestProject
{
    [TestClass]
    public class UpdateSharePointMetadataTest
    {
        [TestMethod]
        public void TestMethod1()
        {

            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(UpdateSharePointMetadata).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();

            inputs.Add("Parent_Entity_Name", "defra_application");
            inputs.Add("Parent_Lookup_Name", "defra_applicationid");
            inputs.Add("Customer", "customer");
            inputs.Add("SiteDetails", "site");
            inputs.Add("PermitDetails", "permit");

            var ltdCompnayId = Guid.Parse("2DAEA1DC-C2A7-E911-A980-000D3A20838A");
            var mainApp = new Entity("defra_application", ltdCompnayId);

            var result = context.ExecuteCodeActivity<UpdateSharePointMetadata>
    (mainApp, inputs);

        }




    }
}

