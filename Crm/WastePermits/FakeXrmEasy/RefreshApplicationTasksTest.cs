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
    public class RefreshApplicationTasksTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GenerateApplicationTasksGivenTaskType).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();
            inputs.Add("Application", new EntityReference("defra_application", new Guid("ca31d9a8-9dc1-e811-a96a-000d3a23443b")));
            inputs.Add("TaskType1", new EntityReference("defra_tasktype", new Guid("f949cc52-6221-e911-a98d-000d3ab311f1")));
            inputs.Add("TaskType2", new EntityReference("defra_tasktype", new Guid("63987ec8-33ef-e811-a988-000d3ab31f97")));


            var result = context.ExecuteCodeActivity<GenerateApplicationTasksGivenTaskType>
     (inputs);


        }
    }
}
