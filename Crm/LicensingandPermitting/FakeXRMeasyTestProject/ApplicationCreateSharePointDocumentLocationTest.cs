using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;
using Defra.Lp.Plugins;
using System.Net;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace FakeXRMeasyTestProject
{
    [TestClass]
    public class ApplicationCreateSharePointDocumentLocationTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var fakedContext = new XrmFakedContext();
            var pluginContext = fakedContext.GetDefaultPluginContext();

            pluginContext.PrimaryEntityName = "defra_application";
            pluginContext.MessageName = "Create";
            pluginContext.Stage = 20;
            pluginContext.PrimaryEntityId= Guid.Parse("2DAEA1DC-C2A7-E911-A980-000D3A20838A");

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", new Entity("defra_application", Guid.Parse("2DAEA1DC-C2A7-E911-A980-000D3A20838A")));
            pluginContext.InputParameters = inputParameters;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            IPlugin pppp = new ApplicationCreateSharePointDocumentLocation("","");
            fakedContext.ExecutePluginWith(pluginContext,pppp);
    
        }
    }
}
