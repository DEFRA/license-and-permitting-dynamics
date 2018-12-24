using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Workflow;
using Lp.DataAccess;

namespace Defra.Lp.Workflows
{

    public class UnshareRecordWithTeam: WorkFlowActivityBase
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

	            #region Create the services
	            var tracingService = executionContext.GetExtension<ITracingService>();
	            if (tracingService == null)
	            {
	                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
	            }
	            tracingService.Trace($"Entered UnshareRecordWithTeam.ExecuteCRMWorkFlowActivity(), Activity Instance Id: {executionContext.ActivityInstanceId}, Workflow Instance Id: {executionContext.WorkflowInstanceId}");
	            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
	            var service = serviceFactory.CreateOrganizationService(null);
                #endregion

	            #region Get Parameters

	            string sharingRecordUrl = SharingRecordURL.Get(executionContext);
	            if (string.IsNullOrEmpty(sharingRecordUrl))
	            {
	                return;
	            }
	            var refObject = DataAccessMetaData.GetEntityReferenceFromRecordUrl(service, sharingRecordUrl);

	            //
	            // Why do we need an List of principals?
	            //
	            List<EntityReference> principals = new List<EntityReference>();
	            EntityReference teamReference = Team.Get(executionContext);
	            principals.Clear();

	            if (teamReference != null)
	            {
	                principals.Add(teamReference);
	            }
                #endregion

	            #region Revoke Access

	            var revokeRequest = new RevokeAccessRequest {Target = refObject};
	            foreach (EntityReference principalObject in principals)
	            {
	                revokeRequest.Revokee = principalObject;
	                RevokeAccessResponse revokeResponse = (RevokeAccessResponse)service.Execute(revokeRequest);
	            }
	            tracingService.Trace("Revoked Permissions--- OK");

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
