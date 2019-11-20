using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Defra.Lp.WastePermits.Plugins;
using System.Net;

namespace FakeXrmEasyTestProject
{
    [TestClass]
    public class ApplicationLineCreateWasteParamsTest
    {
        [TestMethod]
        public void TestMethod1()
        {

            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(ApplicationLineCreateWasteParams).Assembly,
                ConnectionStringName = "CRMOnline"
            };

            //appid a-10d29e16-1602-ea11-a811-000d3a44a2a9
            //appline id - aa81c9f8-1402-ea11-a811-000d3a44afcc

            //applineid 54440894-fa06-ea11-a811-000d3a44a8e9
            //appid 911c6fd6-f606-ea11-a811-000d3a44ade8
            var guid1 = Guid.Parse("36ac59f5-d607-ea11-a811-000d3a44a237");
            var target = new Entity("defra_applicationline") { Id = guid1 };
            target.Attributes.Add("defra_applicationid", new EntityReference("defra_application", Guid.Parse("e17b2ea0-d607-ea11-a811-000d3a44ade8")));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultPluginContext();

            //Execute our plugin against a target that doesn't contains the accountnumber attribute
            //var fakedPlugin = context.ExecutePluginWithTarget(target);
        }
    }
}
