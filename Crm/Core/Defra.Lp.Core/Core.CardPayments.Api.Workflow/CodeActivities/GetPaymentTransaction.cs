// <copyright file="GetPaymentTransaction.cs" company="">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author></author>
// <date>12/4/2017 12:00:54 PM</date>
// <summary>Implements the GetPaymentTransaction Code Activity.</summary>

using Core.Configuration;

namespace Defra.Lp.Core.CardPayments.Workflow.CodeActivities
{
    using System;
    using System.Activities;
    using Abstract;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;

    // Code Activity retrieves the PaymentTransaction entity reference from CRM
    public class GetPaymentTransaction : WorkFlowActivityBase
    {
        [Input("LookupPaymentReferenceNumber")]
        public InArgument<string> LookupPaymentReferenceNumber { get; set; }

        [Output("PaymentTransaction")]
        [ReferenceTarget("defra_paymenttransaction")]
        public OutArgument<EntityReference> PaymentTransaction { get; set; }

        /// <summary>
        /// Retrieves the PaymentTransaction entity reference from CRM
        /// and sets the output parameter
        /// </summary>
        /// <param name="executionContext"></param>
        /// <param name="crmWorkflowContext"></param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            {
                var tracingService = executionContext.GetExtension<ITracingService>();
                tracingService.Trace("FindPayment starting...");

                try
                {
                    // 1. Validation
                    ValidateNotNull(crmWorkflowContext);

                    // 2. Get Payment Transaction Record from CRM
                    string paymentRefNum = LookupPaymentReferenceNumber.Get(executionContext);
                    tracingService.Trace("Calling GetPaymentTransaction...");
                    EntityReference paymentTransaction = crmWorkflowContext.OrganizationService.GetPaymentTransaction(paymentRefNum, tracingService);

                    // 3. Set Output Parameter
                    tracingService.Trace("Setting PaymentTransaction {0}", paymentTransaction);
                    PaymentTransaction.Set(executionContext, paymentTransaction);
                }
                catch (Exception ex)
                {
                    // Todo: Log the Error
                    tracingService.Trace("Exception: " + ex);
                    throw ex;
                }

            }
        }
    }
}
