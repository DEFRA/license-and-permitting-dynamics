using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Defra.Lp.WastePermits.Workflows;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Net;

namespace FakeXrmEasy
{
    [TestClass]
    public class FMeTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            
            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(CreateFmeSupportRecords).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();
            inputs.Add("GetProcess", new EntityReference("workflow", new Guid("95F84FA5-F3EF-49C6-AC51-AB307DCF4151")));
            inputs.Add("GetFetchQuery", @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='defra_application'>
    <attribute name='defra_applicationid' />
    <attribute name='defra_name' />
    <attribute name='createdon' />
    <order attribute='defra_name' descending='false' />
    <filter type='and'>
      <condition attribute='statuscode' operator='eq' value='910400000' />
    </filter>
  </entity>
</fetch>");

            var result = context.ExecuteCodeActivity<CreateFmeSupportRecords>
       ( inputs);

        }
    }
}

       
