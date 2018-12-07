using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Lp.DataAccess;

namespace Defra.Lp.Workflows
{
    /// <summary>
    /// Code activity adds and removes appplication answer records to an application as required
    /// </summary>
    public class RefreshApplicationAnswers: WorkFlowActivityBase
    {
        /// <summary>
        /// The application to process
        /// </summary>
        [RequiredArgument]
        [Input("Application")]
        [ReferenceTarget("defra_application")]
        public InArgument<EntityReference> Application { get; set; }
        
        private ITracingService TracingService { get; set; }

        private IOrganizationService Service { get; set; }

        /// <summary>
        /// Main code activity method
        /// </summary>
        /// <param name="executionContext">Standard execution context</param>
        /// <param name="crmWorkflowContext">Standard workflow context</param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            if (crmWorkflowContext == null)
            {
                throw new ArgumentNullException(nameof(crmWorkflowContext));
            }

            TracingService = executionContext.GetExtension<ITracingService>();
            Service = crmWorkflowContext.OrganizationService;

            // 1. Validation
            EntityReference applicatEntityReference = this.Application.Get(executionContext);

            if (applicatEntityReference == null)
            {
                TracingService.Trace("Account parameter not set.");

                return;
            }

            TracingService.Trace("Getting Contacts for Application {0} ", applicatEntityReference.Id);

            // 2. Processing - add and remove application answers as dictated by the application lines
            DataAccessApplicationAnswers dal = new DataAccessApplicationAnswers(this.Service, this.TracingService);
            dal.RefreshApplicationAnswers(applicatEntityReference.Id);
        }
    }
}
