using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Activities;
using Core.Helpers.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Workflow;
using Lp.DataAccess;

namespace Defra.Lp.Workflows
{ 
    public class ShareRecordWithTeam: WorkFlowActivityBase
    {
        #region "Parameter Definition"

        [RequiredArgument]
        [Input("Sharing Record URL")]
        [ReferenceTarget("")]
        public InArgument<String> SharingRecordURL { get; set; }

        [RequiredArgument]
        [Input("Team")]
        [ReferenceTarget("team")]
        public InArgument<EntityReference> Team { get; set; }

        /// <summary>
        /// Share Read privilege.
        /// </summary>
        [Input("Read Permission")]
        [Default("True")]
        public InArgument<bool> ShareRead { get; set; }

        /// <summary>
        /// Share Write privilege.
        /// </summary>
        [Input("Write Permission")]
        [Default("False")]
        public InArgument<bool> ShareWrite { get; set; }

        /// <summary>
        /// Share Delete privilege.
        /// </summary>
        [Input("Delete Permission")]
        [Default("False")]
        public InArgument<bool> ShareDelete { get; set; }

        /// <summary>
        /// Share Append privilege.
        /// </summary>
        [Input("Append Permission")]
        [Default("False")]
        public InArgument<bool> ShareAppend { get; set; }

        /// <summary>
        /// Share AppendTo privilege.
        /// </summary>
        [Input("Append To Permission")]
        [Default("False")]
        public InArgument<bool> ShareAppendTo { get; set; }

        /// <summary>
        /// Share Assign privilege.
        /// </summary>
        [Input("Assign Permission")]
        [Default("False")]
        public InArgument<bool> ShareAssign { get; set; }

        /// <summary>
        /// Share Share privilege.
        /// </summary>
        [Input("Share Permission")]
        [Default("False")]
        public InArgument<bool> ShareShare { get; set; }

        #endregion

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

                # region Create the services
                var tracingService = executionContext.GetExtension<ITracingService>();
	            if (tracingService == null)
	            {
	                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
	            }
	            tracingService.Trace($"Entered ShareRecordWithTeam.ExecuteCRMWorkFlowActivity(), Activity Instance Id: {executionContext.ActivityInstanceId}, Workflow Instance Id: {executionContext.WorkflowInstanceId}");
	            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
	            var service = serviceFactory.CreateOrganizationService(null);
                #endregion

                #region Get Parameters

	            bool shareAppend = this.ShareAppend.Get(executionContext);
	            bool shareAppendTo = this.ShareAppendTo.Get(executionContext);
	            bool shareAssign = this.ShareAssign.Get(executionContext);
	            bool shareDelete = this.ShareDelete.Get(executionContext);
	            bool shareRead = this.ShareRead.Get(executionContext);
	            bool shareShare = this.ShareShare.Get(executionContext);
	            bool shareWrite = this.ShareWrite.Get(executionContext);
                string sharingRecordUrl = SharingRecordURL.Get(executionContext);
                if (string.IsNullOrEmpty(sharingRecordUrl))
                {
                    return;
                }
	            var refObject = DataAccessMetaData.GetEntityReferenceFromRecordUrl(service, sharingRecordUrl);

                List<EntityReference> principals = new List<EntityReference>();
                EntityReference teamReference = Team.Get(executionContext);
                principals.Clear();
                if (teamReference != null)
                {
                    principals.Add(teamReference);
                }
                #endregion

                #region Grant Access Request

	            tracingService.Trace("Grant Request Start");

                service.GrantAccess(refObject, principals, shareAppend, shareAppendTo, shareAssign, shareDelete,
                    shareRead, shareShare, shareWrite);

	            tracingService.Trace("Grant Request End");

                #endregion

            }
            catch (FaultException<OrganizationServiceFault> e)
            {                
                // Handle the exception.
                throw e;
            }	  

        }
    }
}
