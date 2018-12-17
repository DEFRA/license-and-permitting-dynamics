﻿
// <copyright file="GetPermitDetailsForApplication.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>3/5/2018 8:36:50 AM</date>
// <summary>Implements the GetPermitDetailsForApplication Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using WastePermits.DataAccess;

namespace Defra.Lp.WastePermits.Workflows
{


    /// </summary>    
    public class GetPermitDetails: WorkFlowActivityBase
    {
        #region Properties 
        //Property for Entity defra_application
        [Input("Application")]
        [ReferenceTarget("defra_application")]
        public InArgument<EntityReference> Application { get; set; }

        [Input("Permit")]
        [ReferenceTarget("defra_permit")]
        public InArgument<EntityReference> Permit { get; set; }

        [Output("Return data")]
        public OutArgument<string> ReturnData { get; set; }

        private ITracingService TracingService { get; set; }
        private IWorkflowContext Context { get; set; }
        private IOrganizationService Service { get; set; }
        #endregion

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

            if (crmWorkflowContext == null)
            {
                throw new ArgumentNullException("crmWorkflowContext");
            }

            TracingService = executionContext.GetExtension<ITracingService>();
            Service = crmWorkflowContext.OrganizationService;
            Context = crmWorkflowContext.WorkflowExecutionContext;

            var application = Application.Get(executionContext);
            var permit = Permit.Get(executionContext);
            if (application == null && permit == null) return;

            string returnData = string.Empty;
            if (application != null)
            {
                TracingService.Trace("Getting Standard Rules for application: {0}", application.Id.ToString());
                returnData = Service.GetStandardRules(application);
            }
            else if (permit != null)
            {
                TracingService.Trace("Getting Standard Rules for permit: {0}", permit.Id.ToString());
                returnData = Service.GetStandardRules(permit, "defra_permit", "defra_permitid", "defra_permit_lines");
            }
                         
            ReturnData.Set(executionContext, returnData);
            TracingService.Trace("Returning data: {0}", returnData);
        }
    }
}