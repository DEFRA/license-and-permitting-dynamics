
using System.Text;
using CardPayments;
using CardPayments.Model;

namespace Defra.Lp.Core.Workflows.CompaniesHouse
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.IO;
    using System.Runtime.Serialization.Json;

    public class CardPaymentService : RestServiceBase
    {
        public CardPaymentService(RestServiceConfiguration configuration)
            :base(configuration)
        {
        }

        public CreatePaymentResponse CreatePayment(CreatePaymentRequest request)
        {
            using (this.Httpclient)
            {
                // 1. Call API
                var byteContent = PrepareRequest(request);

                // Todo: Implement retry orchestration
                var result = Post(CardServiceConstants.CreatePaymentCommand, byteContent);
                
                // 2. Check Request Status
                if (result.IsSuccessStatusCode)
                {
                    // 3. Parse Response
                    return ParseResponse<CreatePaymentResponse>(result.Content);
                }
                else
                {
                    return new CreatePaymentResponse
                    {
                        error_message = result.Content?.ReadAsStringAsync().Result,
                        state = new State
                        {
                            status = "error"
                        }
                    };

                    //4. Handle Errors
                    //throw new ApplicationException($"There was an error calling the api {CardServiceConstants.CreatePaymentCommand}: {result.StatusCode} - {result.ReasonPhrase}");
                }
            }
        }


        public FindPaymentResponse FindPayment(FindPaymentRequest request)
        {
            using (this.Httpclient)
            {
                // 1. Call API
                // Todo: Implement retry orchestration
                var result = Get(CardServiceConstants.FindPaymentCommand + "/" + request.PaymentId );

                // 2. Check Request Status
                if (result.IsSuccessStatusCode)
                {
                    // 3. Parse Response
                    return ParseResponse<FindPaymentResponse>(result.Content);
                }
                else
                {
                    //4. Handle Errors
                    throw new ApplicationException($"There was an error calling the api {CardServiceConstants.FindPaymentCommand}: {result.StatusCode} - {result.StatusCode}");

                }
            }
        }

        private static ByteArrayContent PrepareRequest<T>(T request)
        {
            string parameter1 = "";
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(memoryStream, request);
                parameter1 = Encoding.Default.GetString(memoryStream.ToArray());
            }
            var buffer = System.Text.Encoding.UTF8.GetBytes(parameter1);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return byteContent;
        }

        private static T ParseResponse<T>(HttpContent responseContent)
        {
            string responseString = responseContent.ReadAsStringAsync().Result;
            //Set up the object...  
            MemoryStream stream1 = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream1);
            writer.Write(responseString);
            writer.Flush();
            stream1.Position = 0;
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            T response = (T) ser.ReadObject(stream1);
            return response;
        }
    }
}