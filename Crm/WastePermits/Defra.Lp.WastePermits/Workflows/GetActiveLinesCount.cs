// <copyright file="GetActiveLinesCount.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>3/20/2018 3:08:09 PM</date>
// <summary>Implements the GetActiveLinesCount Plugin. Counts active lines lined to an application</summary>

namespace Defra.Lp.WastePermits.Workflows
{
    using System;
    using System.Activities;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using DAL;

    /// <summary>
    /// Counts active lines lined to an application
    /// </summary>
    public class GetActiveLinesCount : WorkFlowActivityBase
    {
        [RequiredArgument]
        [Input("Application")]
        [ReferenceTarget("defra_application")]
        public InArgument<EntityReference> Application { get; set; }

        [RequiredArgument]
        [Output("Count")]
        public OutArgument<int> Count { get; set; }

        /// <summary>
        /// Executes the activity
        /// </summary>
        /// <param name="executionContext">Activity context</param>
        /// <param name="crmWorkflowContext">Includes organisation service</param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            // 1. Validate
            if (crmWorkflowContext == null)
            {
                throw new ArgumentNullException(nameof(crmWorkflowContext));
            }

            // 2. Count lines
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            IOrganizationService service = crmWorkflowContext.OrganizationService;
            var application = this.Application.Get(executionContext);
            var count = service.CountActiveApplicationLines(application.Id);

            // 3. Return Result
            this.Count.Set(executionContext, count);
            tracingService.Trace("Count = {0}", count);
        }
    }
}