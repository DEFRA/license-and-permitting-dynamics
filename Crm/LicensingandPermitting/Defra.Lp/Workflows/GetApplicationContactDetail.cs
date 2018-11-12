// CRM Code Activity used to return a Contact Detail record for a given applicationa and contact detail type
using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Lp.DataAccess;

namespace Defra.Lp.Workflows
{
    /// <summary>
    /// Code Activity returns a Contact Detail record
    /// </summary>
    public class GetApplicationContactDetail : WorkFlowActivityBase
    {
        #region Properties 

        /// <summary>
        /// The application the contact detail is linked to
        /// </summary>
        [RequiredArgument]
        [Input("Application")]
        [ReferenceTarget("defra_application")]
        public InArgument<EntityReference> Application { get; set; }

        /// <summary>
        /// The contact detail type to be retrieved
        /// </summary>
        [RequiredArgument]
        [Input("Contact Detail Type")]
        public InArgument<int> ContactDetailType { get; set; }

        /// <summary>
        /// The return value, a contact detail record
        /// </summary>
        [Output("Application Contact Detail")]
        [ReferenceTarget("defra_addressdetails")]
        public OutArgument<EntityReference> ApplicationContactDetail { get; set; }
        

        private ITracingService TracingService { get; set; }
        private IWorkflowContext Context { get; set; }
        private IOrganizationService Service { get; set; }

        #endregion
        /// <summary>
        /// Main code activity function
        /// </summary>
        /// <param name="executionContext"></param>
        /// <param name="crmWorkflowContext"></param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {

            if (crmWorkflowContext == null)
            {
                throw new ArgumentNullException(nameof(crmWorkflowContext));
            }

            TracingService = executionContext.GetExtension<ITracingService>();
            Service = crmWorkflowContext.OrganizationService;
            Context = crmWorkflowContext.WorkflowExecutionContext;

            // 1. Validation
            var application = this.Application.Get(executionContext);
            int contactDetailType = this.ContactDetailType.Get(executionContext);

            if (application == null)
            {
                TracingService.Trace("Application parameter not set.");

                return;
            }

            if (contactDetailType < 1)
            {
                TracingService.Trace("ContactDetailType parameter not set.");
                return;
            }

            TracingService.Trace("Getting Contact Detail {0} for application: {1}", contactDetailType, application.Id.ToString());

            // 2. Processing
            EntityReference contactDetail = DataAccessContactDetail.GetContactDetail(Service, application.Id, contactDetailType);

            // 3. Return value
            ApplicationContactDetail.Set(executionContext, contactDetail);
        }
    }
}