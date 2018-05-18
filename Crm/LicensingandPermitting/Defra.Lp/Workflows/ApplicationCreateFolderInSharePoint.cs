﻿// <copyright file="ApplicationCreateFolderInSharePoint.cs" company="">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author></author>
// <date>10/6/2017 4:49:48 PM</date>
// <summary>Implements the ApplicationCreateFolderInSharePoint Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Defra.Lp.Common.SharePoint;

namespace Defra.Lp.Workflows
{
    /// </summary>    
    public class ApplicationCreateFolderInSharePoint: WorkFlowActivityBase
    {
        [RequiredArgument]
        [Input("Application")] 
        [ReferenceTarget("defra_application")]
        public InArgument<EntityReference> Application { get; set; }

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
                var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
                var service = crmWorkflowContext.OrganizationService;
                var adminService = serviceFactory.CreateOrganizationService(null);

                tracingService.Trace("In ApplicationCreateFolderInSharePoint with Application Name");

                var application = this.Application.Get(executionContext);

                AzureInterface azureInterface = new AzureInterface(adminService, service, tracingService);
                azureInterface.CreateFolder(application);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in Workflow assembly.", ex);
            }
        }
    }
}