﻿// <copyright file="GetAddressesForPostcode.cs" company="">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author></author>
// <date>10/11/2017 11:51:02 AM</date>
// <summary>Implements the GetAddressesForPostcode Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
using Defra.Lp.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json;
using System;
using System.Activities;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;

namespace Defra.Lp.Workflows
{
    /// </summary>    
    public class GetAddressesForPostcode: WorkFlowActivityBase
    {
        /// <summary>
        /// Executes the WorkFlow.
        /// </summary>
        /// <param name="crmWorkflowContext">The <see cref="LocalWorkflowContext"/> which contains the
        /// <param name="executionContext" > <see cref="CodeActivityContext"/>
        /// </param>       
        /// <remarks>
        /// For improved performance, Microsoft Dynamics 365 caches WorkFlow instances.
        /// The WorkFlow's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the WorkFlow. Also, multiple system threads
        /// could execute the WorkFlow at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in WorkFlows.
        /// </remarks>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {                 

            if (crmWorkflowContext == null)
            {
                throw new ArgumentNullException("crmWorkflowContext");
            }

            try
	        {
                var tracingService = executionContext.GetExtension<ITracingService>();
                var service = crmWorkflowContext.OrganizationService;

                var postcode = this.Postcode.Get(executionContext);
                tracingService.Trace(string.Format("In GetAddressesForPostcode with PostCode = {0}", postcode));

                var url = string.Format(Query.GetConfigurationValue(service, "AddressbaseFacadeUrl"), postcode);
                var addresses = string.Empty;

                using (var httpclient = new HttpClient())
                {
                    // Synchronous call to address base facade
                    var response = httpclient.GetAsync(url).Result;
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode != HttpStatusCode.BadRequest)
                        {
                            // Throw exception if its not 400
                            response.EnsureSuccessStatusCode();
                        }
                        else
                        {
                            // Handle error in body that was returned by address base facade 
                            // and throw a new one with the error message in it so can be used
                            // in web resource.

                            var httpErrorObject = response.Content.ReadAsStringAsync().Result;
                            tracingService.Trace(string.Format("Facacde Error: {0}", httpErrorObject));

                            // Template for anonymous deserialisation
                            var anonymousErrorObject = new
                            {
                                facade_status_code = 400,
                                facade_error_message = string.Empty,
                                facade_error_code = string.Empty,
                                supplier_was_called = true,
                                supplier_status_code = 400,
                                supplier_response = new { error = new { statuscode = 400, message = string.Empty } }
                            };
                            var deserializedErrorObject = JsonConvert.DeserializeAnonymousType(httpErrorObject, anonymousErrorObject);

                            var ex = new InvalidPluginExecutionException(deserializedErrorObject.supplier_response.error.message);
                            tracingService.Trace(string.Format("Throwing exception for Facacde Error: {0}", deserializedErrorObject.supplier_response.error.message));
                            throw ex;
                        }
                    }
                    else
                    {
                        addresses = response.Content.ReadAsStringAsync().Result;
                    }
                }
                this.Addresses.Set(executionContext, addresses);
                tracingService.Trace(string.Format("Returned addresses: {0}", addresses));
            }
	        catch (FaultException<OrganizationServiceFault> e)
            { 
                throw e;
            }	  
        }

        [RequiredArgument]
        [Input("Postcode")]
        public InArgument<string> Postcode { get; set; }

        [Output("Addresses")]
        public OutArgument<string> Addresses { get; set; }
    }
}