﻿// <copyright file="CompaniesHouseValidation.cs" company="">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author></author>
// <date>12/4/2017 12:00:54 PM</date>
// <summary>Implements the CompaniesHouseValidation Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Runtime.Serialization;
using CardPayments;
using CardPayments.Interfaces;
using CardPayments.Model;
using Core.Configuration;
using Core.Model.Entities;
using Defra.Lp.Core.Core.CardPayments.Workflow;
using Defra.Lp.Core.Core.CardPayments.Workflow.Constants;
using Defra.Lp.Core.Workflows.CompaniesHouse;
using Messages = Defra.Lp.Core.Core.CardPayments.Workflow.Messages;

namespace Defra.Lp.Core.Workflows
{

    /// </summary>    
    public class CreatePayment : WorkFlowActivityBase
    {
        // Input Parameters

        [RequiredArgument]
        [Input("Amount")]
        public InArgument<Money> Amount { get; set; }

        [RequiredArgument]
        [Input("Reference")]
        public InArgument<string> Reference { get; set; }

        [RequiredArgument]
        [Input("Return URL")]
        public InArgument<string> ReturnUrl { get; set; }

        [RequiredArgument]
        [Input("Description")]
        public InArgument<string> Description { get; set; }

        [Input("Configuration Prefix")]
        public InArgument<string> ConfigurationPrefix { get; set; }

        // Output Parameters

        [RequiredArgument]
        [Output("Status")]
        public OutArgument<string> Status { get; set; }

        [RequiredArgument]
        [Output("PaymentId")]
        public OutArgument<string> PaymentId { get; set; }

        [Output("Finished")]
        public OutArgument<bool> Finished { get; set; }

        [RequiredArgument]
        [Output("NextUrlHref")]
        public OutArgument<string> NextUrlHref { get; set; }

        [Output("NextUrlMethod")]
        public OutArgument<string> NextUrlMethod { get; set; }

        [Output("CancelUrlHref")]
        public OutArgument<string> CancelUrlHref { get; set; }

        [Output("CancelUrlMethod")]
        public OutArgument<string> CancelUrlMethod { get; set; }

        [Output("PaymentProvider")]
        public OutArgument<string> PaymentProvider { get; set; }



        /// <summary>
        /// Executes the WorkFlow.
        /// </summary>
        /// <param name="crmWorkflowContext">The <see cref="WorkFlowActivityBase.LocalWorkflowContext"/> which contains the
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
            try
            {
                // 1. Validation
                ValidateNotNull(crmWorkflowContext);

                // 2. Prepare API Request
                CreatePaymentRequest apiRequest = this.PrepareCardPaymentRequest(executionContext);

                // 2. Retrieve Configuration
                RestServiceConfiguration cardServiceConfiguration = this.RetrieveCardPaymentServiceConfiguration(crmWorkflowContext.OrganizationService, ConfigurationPrefix.Get(executionContext));

                // 3. Set-up the Api Service
                CardPaymentService cardPaymentService = new CardPaymentService(cardServiceConfiguration);

                // 4. Call the API
                CreatePaymentResponse apiResponse = cardPaymentService.CreatePayment(apiRequest);

                // 5. Return the response
                this.PrepareOutputParameters(apiResponse);

            }
            catch (Exception ex)
            {
                // Todo: Log the Error
                throw ex;
            }
        }

        private static void ValidateNotNull(LocalWorkflowContext crmWorkflowContext)
        {
            if (crmWorkflowContext == null)
            {
                throw new ArgumentNullException(nameof(crmWorkflowContext));
            }
        }

        private void PrepareOutputParameters(CreatePaymentResponse apiResponse)
        {
            throw new NotImplementedException();
        }

        private CreatePaymentRequest PrepareCardPaymentRequest(CodeActivityContext executionContext)
        {
            CreatePaymentRequest apiRequest = new CreatePaymentRequest
            {
                amount = (int) (Amount.Get(executionContext).Value*100),
                description = Description.Get(executionContext),
                reference = Reference.Get(executionContext),
                return_url = ReturnUrl.Get(executionContext),
            };

            if (apiRequest.amount <= 0)
            {
                throw new ArgumentException(Messages.InvalidArgument, nameof(Amount));
            }
            if (string.IsNullOrWhiteSpace(apiRequest.description))
            {
                throw new ArgumentException(Messages.InvalidArgument, nameof(Description));
            }
            if (string.IsNullOrWhiteSpace(apiRequest.reference))
            {
                throw new ArgumentException(Messages.InvalidArgument, nameof(Reference));
            }
            if (string.IsNullOrWhiteSpace(apiRequest.return_url))
            {
                throw new ArgumentException(Messages.InvalidArgument, nameof(ReturnUrl));
            }
            return apiRequest;
        }

        private RestServiceConfiguration RetrieveCardPaymentServiceConfiguration(IOrganizationService organizationService, string configurationPrefix)
        {
            RestServiceConfiguration config = new RestServiceConfiguration
            {
                ApiKey =
                    organizationService.GetConfigurationStringValue(string.IsNullOrWhiteSpace(configurationPrefix)
                        ? CardPaymentServiceSecureConfigurationKeys.ApiKey
                        : string.Format("{0}{1}", configurationPrefix + CardPaymentServiceSecureConfigurationKeys.ApiKey))
            };

            return config;
        }
    }
}