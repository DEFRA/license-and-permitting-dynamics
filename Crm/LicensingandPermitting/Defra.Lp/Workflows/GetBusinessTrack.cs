using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Lp.DataAccess;
using Lp.Model.EarlyBound;

namespace Defra.Lp.Workflows
{

    // Main code ativity class    
    public class GetBusinessTrack : WorkFlowActivityBase
    {

        #region Properties 

        /// <summary>
        /// The application that we need to use when searching for the business track
        /// </summary>
        [RequiredArgument]
        [Input("Application")]
        [ReferenceTarget(defra_application.EntityLogicalName)]
        public InArgument<EntityReference> Application { get; set; }


        /// <summary>
        /// Business track linked to the application
        /// </summary>
        [Output("Business Track")]
        [ReferenceTarget(defra_businesstrack.EntityLogicalName)]
        public OutArgument<EntityReference> BusinessTrack { get; set; }


        private ITracingService TracingService { get; set; }
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

            // 1. Validation

            // 2. Processing - Query CRM for contacts of a given role and linked to the given account
            DataAccessApplicationBusinessTrack dataAccess = new DataAccessApplicationBusinessTrack(this.Service, this.TracingService);
            EntityReference businessTrack = dataAccess.GetApplicationBusinessTrackEntityReference(Guid.Empty);

            // 3. Set the output parameters
            BusinessTrack.Set(executionContext, businessTrack);
        }
    }
}
