// <copyright file="CopyApplicationDataToPermit.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <summary>Code activity copies reference data from the Application to a Permit, generating 
// Permit Lines as required</summary>

using Defra.Lp.Workflows.Helpers;

namespace Defra.Lp.Workflows
{
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk.Workflow;
     using global::Model.Lp.Crm;
    using Location = global::Model.Lp.Crm.Location;

    /// <summary>
    /// Code Activity Man Class
    /// </summary>
    public sealed class CopyApplicationDataToPermit : WorkFlowActivityBase
    {
        /// <summary>
        /// Executes the workflow activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            // Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            if (tracingService == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
            }

            tracingService.Trace("Entered CopyApplicationDataToPermit.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("CopyApplicationDataToPermit.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            // Elevated rights service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);

            try
            {

                //Retrieve the application
                Entity application = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(Application.Permit));

                if (application.Attributes.Contains(Application.Permit) && application[Application.Permit] != null)
                {
                    //Init the copier
                    RelationshipManager copier = new RelationshipManager(service, Application.EntityLogicalName, application.Id, Permit.EntityLogicalName, ((EntityReference)application[Application.Permit]).Id);

                    //Copy Location
                    copier.LinkEntitiesToTarget(Location.EntityLogicalName, Location.Application, Location.Permit, true);

                    //Copy Lines
                    copier.CopyAs(
                        ApplicationLine.EntityLogicalName,
                        ApplicationLine.ApplicationId, 
                        new []
                        {
                            PermitLine.Name, PermitLine.PermitType, PermitLine.StandardRule
                        }, 
                        PermitLine.EntityLogicalName, 
                        PermitLine.Permit, 
                        true);
                }
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting CopyApplicationDataToPermit.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}