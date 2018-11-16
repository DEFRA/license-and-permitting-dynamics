using System;
using System.Activities;
using Lp.DataAccess;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Defra.Lp.Workflows
{
    /// <summary>
    /// CRM code activity deactivates all queues linked to the given record
    /// </summary>
    public class RemoveFromQueues: WorkFlowActivityBase
    {
        /// <summary>
        /// Dynamics URL for the record to be processed
        /// </summary>
        [RequiredArgument]
        [Input("Parent Record URL")]
        [ReferenceTarget("")]
        public InArgument<String> ParentRecordURL { get; set; }

        /// <summary>
        /// Main Execution Method
        /// </summary>
        /// <param name="executionContext">Activity Execution Context</param>
        /// <param name="crmWorkflowContext">Includes organisation service and trace service</param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            // 1. Validation
            string parentRecordUrl = this.ParentRecordURL.Get(executionContext);
            if (string.IsNullOrEmpty(parentRecordUrl))
            {
                return;
            }

            // 2. Get the entity reference for the given record url
            EntityReference entityReference = DataAccessMetaData.GetEntityReferenceFromRecordUrl(crmWorkflowContext.OrganizationService, parentRecordUrl);

            // 3. Deactivate all queue items for the record
            DataAccessQueues.DeactivateQueueItems(crmWorkflowContext.OrganizationService, entityReference.Id);
        }
    }
}
