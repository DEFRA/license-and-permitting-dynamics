using System.ServiceModel;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Lp.DataAccess;
using Lp.Model.Crm;
using Microsoft.Xrm.Sdk.Query;

namespace Defra.Lp.Workflows
{
    /// <summary>
    /// Code activity copies location and application lines from one application to another
    /// </summary>
    public class CopyApplicationDataToApplication: WorkFlowActivityBase
    {


        [RequiredArgument]
        [Input("Copy From Application")]
        [ReferenceTarget("defra_application")]
        public InArgument<EntityReference> ApplicationFrom { get; set; }

        [RequiredArgument]
        [Input("Copy To Application")]
        [ReferenceTarget("defra_application")]
        public InArgument<EntityReference> ApplicationTo { get; set; }

        /// <summary>
        /// Executes the WorkFlow.
        /// </summary>
        /// <param name="crmWorkflowContext">The <see cref="WorkFlowActivityBase.LocalWorkflowContext"/> which contains the
        /// <param name="executionContext" > <see cref="CodeActivityContext"/>
        /// </param>       
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {

            // Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            if (tracingService == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
            }

            tracingService.Trace("Entered CopyApplicationDataToApplication.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("CopyApplicationDataToApplication.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            // Elevated rights service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);

            try
            {
                EntityReference fromApplication = ApplicationFrom.Get(executionContext);
                EntityReference toApplication = ApplicationTo.Get(executionContext);



                if (fromApplication != null && toApplication != null)
                {
                    //Init the copier
                    DataAccessPermit copier = new DataAccessPermit(service, Application.EntityLogicalName, fromApplication.Id, Application.EntityLogicalName, toApplication.Id);

                    //Copy Location and Location details from Application to Permit
                    DataAccessApplication.MirrorApplicationLocationsAndDetailsToApplication(service, fromApplication.Id, toApplication.Id);

                    //Copy Lines
                    copier.CopyAs(
                        ApplicationLine.EntityLogicalName,
                        ApplicationLine.ApplicationId,
                        new[] {
                            ApplicationLine.Name,
                            ApplicationLine.PermitType,
                            ApplicationLine.StandardRule,
                            ApplicationLine.Owner
                        },
                        ApplicationLine.EntityLogicalName,
                        ApplicationLine.ApplicationId,
                        true, new ConditionExpression(PermitLine.LineType, ConditionOperator.Equal, (int)LineTypes.RegulatedFacility));
                }
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting CopyApplicationDataToApplication.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
         

    }

}
