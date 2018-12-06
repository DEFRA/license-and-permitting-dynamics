


namespace Lp.Workflows
{
    using System;
    using System.Activities;
    using Microsoft.Xrm.Sdk;
    using Defra.Lp.Workflows;
    using Microsoft.Xrm.Sdk.Workflow;

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

            // 2. Processing - Query CRM for contacts of a given role and linked to the given account
            DataAccessContact dataAccess = new DataAccessContact(this.Service, this.TracingService);
            EntityReference[] contactEntityReferences = dataAccess.GetAccountContacts(account.Id, (Contact_AccountRoleCode)contactRole);

            // 3. Return contacts found
            if (contactEntityReferences == null)
            {
                // No contacts
                return;
            }
        }
    }
}
