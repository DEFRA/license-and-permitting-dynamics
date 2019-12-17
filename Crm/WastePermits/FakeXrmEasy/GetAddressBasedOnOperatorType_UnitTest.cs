using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Defra.Lp.WastePermits.Workflows;
using FakeXrmEasy;
using System.Net;
using System.Collections.Generic;
using WastePermits.Model.EarlyBound;

namespace FakeXrmEasyTestProject
{
    [TestClass]
    public class GetAddressBasedOnOperatorType_UnitTest
    {
        [TestMethod]
        public void Test_For_LTD_Compan_Operator_Name()
        {

            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetAddressBasedOnOperatorType).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();
           
            var ltdCompnayId = Guid.Parse("2DAEA1DC-C2A7-E911-A980-000D3A20838A");
          
            var mainApp = new Entity("defra_application", ltdCompnayId);

            var result = context.ExecuteCodeActivity<GetAddressBasedOnOperatorType>
    (mainApp,inputs);

        }

        [TestMethod]
        public void Test_For_SoleTrader_Operator_Name()
        {

            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetAddressBasedOnOperatorType).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();
           

           
            var soleId = Guid.Parse("3D180D6A-C7A7-E911-A97D-000D3A233B72");
            var mainApp = new Entity("defra_application", soleId);

            var result = context.ExecuteCodeActivity<GetAddressBasedOnOperatorType>
    (mainApp, inputs);
           

        }

        [TestMethod]
        public void Test_For_Partner_Operator_Name()
        {

            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetAddressBasedOnOperatorType).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();
        

            var partnerId = Guid.Parse("86C561FF-D2A7-E911-AA0A-000D3A2065C5");
            var mainApp = new Entity("defra_application", partnerId);

            var result = context.ExecuteCodeActivity<GetAddressBasedOnOperatorType>
    (mainApp, inputs);


        }

        [TestMethod]
        public void Test_For_Registeredcharity_Operator_Name()
        {

            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetAddressBasedOnOperatorType).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();
            inputs.Add("GetContact", new EntityReference("contact", null));
            inputs.Add("GetAccount", new EntityReference("account", new Guid("95F84FA5-F3EF-49C6-AC51-AB307DCF4151")));
            inputs.Add("GetOperatorType", new OptionSetValue((int)defra_organisation_type.Limitedcompany));


            var partnerId = Guid.Parse("86C561FF-D2A7-E911-AA0A-000D3A2065C5");
            var mainApp = new Entity("defra_application", partnerId);

            var result = context.ExecuteCodeActivity<GetAddressBasedOnOperatorType>
    (mainApp, inputs);


        }

        [TestMethod]
        public void Test_For_Limitedliabilitypartnership_Operator_Name()
        {

            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetAddressBasedOnOperatorType).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();
          


            var partnerId = Guid.Parse("2EEC4D50-A0A7-E911-A980-000D3A20838A");
            var mainApp = new Entity("defra_application", partnerId);

            var result = context.ExecuteCodeActivity<GetAddressBasedOnOperatorType>
    (mainApp, inputs);


        }

        [TestMethod]
        public void Test_For_Otherorganisationforexampleacluborassociation_Operator_Name()
        {

            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetAddressBasedOnOperatorType).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();
          

            var partnerId = Guid.Parse("EFBF132B-1EA9-E911-A980-000D3A20838A");
            var mainApp = new Entity("defra_application", partnerId);

            var result = context.ExecuteCodeActivity<GetAddressBasedOnOperatorType>
    (mainApp, inputs);


        }

        [TestMethod]
        public void Test_For_NULL_OP_TYPE_Operator_Name()
        {

            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetAddressBasedOnOperatorType).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();
           

            var partnerId = Guid.Parse("7A58F293-46A9-E911-A97F-000D3A23443B");
            var mainApp = new Entity("defra_application", partnerId);

            var result = context.ExecuteCodeActivity<GetAddressBasedOnOperatorType>
    (mainApp, inputs);


        }


        [TestMethod]
        public void Test_For_Registeredcharity_Operator_Name_Publicbodyaddress()
        {

            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetAddressBasedOnOperatorType).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();
            

            var partnerId = Guid.Parse("73585216-8007-E911-A990-000D3A2065C5");
            var mainApp = new Entity("defra_application", partnerId);

            var result = context.ExecuteCodeActivity<GetAddressBasedOnOperatorType>
    (mainApp, inputs);


        }

        [TestMethod]
        public void Test_For_notYPE_Operator_Name_Publicbodyaddress()
        {

            var context = new XrmRealContext
            {
                ProxyTypesAssembly = typeof(GetAddressBasedOnOperatorType).Assembly,
                ConnectionStringName = "CRMOnline"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var executionContext = context.GetDefaultWorkflowContext();

            //Inputs
            var inputs = new Dictionary<string, object>();


            var partnerId = Guid.Parse("CEA2C51C-B8A7-E911-AA0A-000D3A2065C5");
            var mainApp = new Entity("defra_application", partnerId);

            var result = context.ExecuteCodeActivity<GetAddressBasedOnOperatorType>
    (mainApp, inputs);


        }

        

    }
}
