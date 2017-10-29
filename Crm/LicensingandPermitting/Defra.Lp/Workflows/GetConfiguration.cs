using Defra.Lp.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.ServiceModel;

namespace Defra.Lp.Workflows
{
    public class GetConfiguration: WorkFlowActivityBase
    {
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
                var tracingService = executionContext.GetExtension<ITracingService>();
                var service = crmWorkflowContext.OrganizationService;

                var configName = this.ConfigName.Get(executionContext);
                tracingService.Trace(string.Format("In GetConfiguration with defra_name = {0}", configName));

                var config = Query.GetConfigurationEntity(service, configName);

                if (config != null)
                {
                    this.Configuration.Set(executionContext, config.ToEntityReference());
                    tracingService.Trace(string.Format("Got Configuration with Id = {0}", config.Id.ToString()));
                    //tracingService.Trace(string.Format("Got SharePointPermitList with Id = {0}", ((EntityReference)config["defra_sharepointpermitlist"]).Id.ToString()));
                }
                else
                {
                    this.Configuration.Set(executionContext, null);
                    tracingService.Trace("No Config found");
                }

            }
            catch (FaultException<OrganizationServiceFault> e)
            {                
                // Handle the exception.
                throw e;
            }	  

        }

        [RequiredArgument]
        [Input("Config Name")]
        public InArgument<string> ConfigName { get; set; }

        [Output("Configuration Entity")]
        [ReferenceTarget("defra_configuration")]
        public OutArgument<EntityReference> Configuration { get; set; }
    }
}