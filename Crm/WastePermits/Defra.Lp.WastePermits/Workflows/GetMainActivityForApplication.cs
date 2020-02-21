using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Lp.WastePermits.Workflows
{
    public class GetMainActivityForApplication : WorkFlowActivityBase
    {
        [Input("Application")]
        [ReferenceTarget("defra_application")]
        public InArgument<EntityReference> GetApplication { get; set; }

        [Output("MainActivity")]
        [ReferenceTarget("defra_applicationline")]
        public OutArgument<EntityReference> MianActivity { get; set; }

        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {

            if (crmWorkflowContext == null)
            {
                throw new ArgumentNullException("crmWorkflowContext");
            }

           

            var tracingService = executionContext.GetExtension<ITracingService>();
            var service = crmWorkflowContext.OrganizationService;
            var context = crmWorkflowContext.WorkflowExecutionContext;
            var appLineId = context.PrimaryEntityId;
            var appId = GetApplication.Get<EntityReference>(executionContext).Id;

            tracingService.Trace("inside GetMainActivityForApplication ");

            var fetchXml = @"<fetch >
                  <entity name='defra_applicationline'>
                    <attribute name='defra_applicationlineid' />
                    <attribute name='defra_name' />
                    <attribute name='defra_standardruleid' />
                    <attribute name='defra_linetype' />
                    <attribute name='defra_itemid' />
                    <attribute name='defra_item_type' />
                    <attribute name='defra_value' />
                    <order attribute='defra_value' descending='true' />
                    <filter type='and'>
                      <condition attribute='defra_applicationid' operator='eq'  uitype='defra_application' value='{" + appId + @"}' />
                     <condition attribute='statecode' value='0' operator='eq'/>
                    </filter>
                  </entity>
                </fetch>";

            tracingService.Trace(fetchXml);
            RetrieveMultipleRequest fetchRequest = new RetrieveMultipleRequest
            {
                Query = new FetchExpression(fetchXml)
            };

            var result = ((RetrieveMultipleResponse)service.Execute(fetchRequest)).EntityCollection.Entities.FirstOrDefault().ToEntityReference();
            MianActivity.Set(executionContext, result);
            tracingService.Trace("GetMainActivityForApplication done");
        }
    }
}
