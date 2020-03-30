using Defra.Lp.WastePermits.Workflows;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FakeXrmEasyTestProject
{
    [TestClass]
    public class GetDandRCodesGivenApplicationTest
    {
        [TestMethod]
        public void Test_valid_Not()
        {
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetDandRCodesGivenApplication).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();

            var ltdCompnayId = Guid.Parse("3A54B8B0-E31B-EA11-A811-000D3A44A237");

            var mainApp = new Entity("defra_application", ltdCompnayId);

            var result = context.ExecuteCodeActivity<GetDandRCodesGivenApplication>
                (mainApp, inputs);
        }
    }
}
