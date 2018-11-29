// Dynamics 365 Code Activity, returns an auto number string
namespace Lp.Workflows
{
    using System.Activities;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using Defra.Lp.Workflows;
    using DataAccess;

    /// <summary>
    /// Main code activity class
    /// </summary>
    public class GetNextAutoNumber : WorkFlowActivityBase
    {
        /// <summary>
        /// The auto number key to be used
        /// </summary>
        [Input("AutoNumber Name"), RequiredArgument]
        [Default("")]
        public InArgument<string> AutoNumberName { get; set; }

        /// <summary>
        /// Returns the generated number
        /// </summary>
        [Output("Generated Number"), RequiredArgument]
        [Default("ERROR")]
        public OutArgument<string> GeneratedNumber { get; set; }

        /// <summary>
        /// Executes the workflow activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            // Create the services
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            if (tracingService == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
            }

            tracingService.Trace(
                "Entered GetNextAutoNumber.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);

            // Call CRM
            DataAccessAutoNumber dal = new DataAccessAutoNumber(service, tracingService);
            string permitNumber = dal.GetNextPermitNumber(AutoNumberName.Get(executionContext));

            // Return the auto-number
            GeneratedNumber.Set(executionContext, permitNumber);

            tracingService.Trace($"GetNextAutoNumber Done. Generated number: {permitNumber}, Activity Instance Id: {executionContext.ActivityInstanceId}, Workflow Instance Id: {executionContext.WorkflowInstanceId}");
        }
    }
}
