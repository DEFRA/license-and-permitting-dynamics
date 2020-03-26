using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;
using Defra.Lp.Plugins;
using System.Net;
using Microsoft.Xrm.Sdk;

namespace FakeXRMeasyTestProject
{
    [TestClass]
    public class SendSingleAttachmentToLogicAppTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(SendSingleAttachmentToLogicApp).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            var guid1 = Guid.Parse("2B4E4CD1-5E68-EA11-A811-000D3A44AFCC");
            var target = new Entity("annotation") { Id = guid1 };

            //Execute our plugin against a target that doesn't contains the accountnumber attribute
            var fakedPlugin = context.ExecutePluginWithTarget<SendSingleAttachmentToLogicApp>(target);


        }
    }
}
