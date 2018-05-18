using Defra.Lp.Common.SharePoint;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace Defra.Lp.Workflows
{
    public class UploadIndividualAttachmentToSharePoint: WorkFlowActivityBase
    {
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            if (crmWorkflowContext == null)
            {
                throw new ArgumentNullException(nameof(crmWorkflowContext));
            }

            try
            {
                var tracingService = executionContext.GetExtension<ITracingService>();
                var context = executionContext.GetExtension<IWorkflowContext>();
                var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
                var service = crmWorkflowContext.OrganizationService;
                var adminService = serviceFactory.CreateOrganizationService(null);

                tracingService.Trace("In UploadIndividualAttachmentToSharePoint.");

                var parentEntityName = Parent_Entity_Name.Get(executionContext);
                var parentLookupName = Parent_Lookup_Name.Get(executionContext);

                tracingService.Trace(string.Format("Parent Entity = {0}; Parent Lookup = {1}", parentEntityName, parentLookupName));

                AzureInterface azureInterface = new AzureInterface(adminService, service, tracingService);
                azureInterface.UploadFile(new EntityReference(context.PrimaryEntityName, context.PrimaryEntityId), parentEntityName, parentLookupName);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in Workflow assembly.", ex);
            }
        }

        [RequiredArgument]
        [Input("Parent Entity Name")]
        public InArgument<string> Parent_Entity_Name { get; set; }

        [RequiredArgument]
        [Input("Parent Lookup Name")]
        public InArgument<string> Parent_Lookup_Name { get; set; }
    }
}