using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CardPayments;
using CardPayments.Model;
using Defra.Lp.Core.Workflows.CompaniesHouse;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.CardPayments.Tests
{

    [TestClass]
    public class CardPaymentsServiceTests
    {
        private const string APIKey = "";
        private string pid;

        private RestServiceConfiguration serviceConfiguration = new RestServiceConfiguration
        {
            ApiKey = "5npsgtkn8p0sd5r0l03qhhlsuafbk9cbgecu038ksvuvl091uhvo8i6pk4",
            SecurityProtocol = SecurityProtocolType.Tls12,
            SecurityHeader = "Bearer",
            TargetHost = "publicapi.payments.service.gov.uk",
            TargetUrl = "https://publicapi.payments.service.gov.uk/"
        };

        
        [TestMethod]
        public void CreatePaymentIsSuccessful()
        {
            CardPaymentService service = new CardPaymentService(serviceConfiguration);

            var response = service.CreatePayment(new CreatePaymentRequest
            {
                amount = 100,
                description = "Test1",
                return_url = "https://defra/",
                reference = "MSTEST1"
            });
            Assert.IsNotNull(response);
        }


        [TestMethod]
        public void CreatePaymentGovPayOffline()
        {
            var config = serviceConfiguration;
            config.TargetUrl = "https://test.invaliddomaindoesnotexisteverever.com/";
            config.TargetHost = "test.invaliddomaindoesnotexisteverever.com";

            CardPaymentService service = new CardPaymentService(serviceConfiguration);

            var response = service.CreatePayment(new CreatePaymentRequest
            {
                amount = 100,
                description = "Test1",
                return_url = "https://defra/",
                reference = "MSTEST1"
            });
            Assert.IsNotNull(response.state.status = "error");
        }


        [TestMethod]
        public void FindPaymentIsSuccessful()                                                                                   
        {
            var paymentResponse = new CardPaymentService(serviceConfiguration).CreatePayment(new CreatePaymentRequest
            {
                amount = 150000,
                description = "Waste Permit",
                return_url = "https://ag-ea-lp-dev-waste.crm4.dynamics.com/main.aspx?etc=10013&extraqs=formid%3df5880608-0176-460b-97dd-8d840c9d8bd4&id=%7b669F4908-0826-E811-A951-000D3AB3984F%7d&pagetype=entityrecord",
                reference = "PERMIT 123"
            });

            var response = new CardPaymentService(serviceConfiguration).FindPayment(new FindPaymentRequest
            {
                PaymentId = paymentResponse.payment_id
            });

            pid = response.payment_id;
            Assert.IsNotNull(response);

            response = new CardPaymentService(serviceConfiguration).FindPayment(new FindPaymentRequest
            {
                PaymentId = paymentResponse.payment_id
            });

            Assert.IsNotNull(response);
        }



        [TestMethod]
        public void FindPaymentSuccessful()
        {

            var response = new CardPaymentService(serviceConfiguration).FindPayment(new FindPaymentRequest
            {
                PaymentId = pid
            });

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void FindPaymentSuccessfulGovPayOffline()
        {
            var config = serviceConfiguration;
            config.TargetUrl = "https://test.invaliddomaindoesnotexisteverever.com/";
            config.TargetHost = "test.invaliddomaindoesnotexisteverever.com";

            var response = new CardPaymentService(config).FindPayment(new FindPaymentRequest
            {
                PaymentId = pid
            });

            Assert.IsNotNull(response.state.status = "error");
        }
    }
}
