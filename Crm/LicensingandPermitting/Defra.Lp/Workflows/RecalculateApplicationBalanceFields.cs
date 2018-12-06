using System.ServiceModel;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Lp.DataAccess;

namespace Defra.Lp.Workflows
{
    /// <summary>
    /// Code activity prompts an application to recalc it's balance fields
    /// </summary>
    public class RecalculateApplicationBalanceFields: WorkFlowActivityBase
    {

        /// <summary>
        /// Application to be recalculated
        /// </summary>
        [RequiredArgument]
        [Input("Application")]
        [ReferenceTarget("defra_application")]
        public InArgument<EntityReference> Application { get; set; }

        /// <summary>
        /// Main code activity method
        /// </summary>
        /// <param name="executionContext">CRM Exec Context</param>
        /// <param name="crmWorkflowContext">CRM Workflow Context</param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {

            // Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            if (tracingService == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
            }

            tracingService.Trace("Entered RecalculateApplicationBalanceFields.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            // Elevated rights service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);

            try
            {
                EntityReference app = Application.Get(executionContext);

                if (app != null)
                {
                    // Call DAL to recalculate the application fields
                    DataAccessApplication.RecalculateApplicationBalances(service, tracingService, app.Id);
                }
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());
                throw;
            }

            tracingService.Trace("Exiting RecalculateApplicationBalanceFields.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}
