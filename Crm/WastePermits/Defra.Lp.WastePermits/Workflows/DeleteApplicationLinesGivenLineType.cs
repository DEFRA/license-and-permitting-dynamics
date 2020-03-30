using Lp.Model.Crm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WastePermits.Model.EarlyBound;

namespace Defra.Lp.WastePermits.Workflows
{
   public class DeleteApplicationLinesGivenLineType : WorkFlowActivityBase
    {
        [Input("LineType")]
        public InArgument<int> GetLineType { get; set; }

        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {

            var tracingService = executionContext.GetExtension<ITracingService>();
            tracingService.Trace("Inside DeleteApplicationLinesGivenLineType");

            var service = crmWorkflowContext.OrganizationService;
            var context = crmWorkflowContext.WorkflowExecutionContext;

            var currecntAppId = context.PrimaryEntityId;

            var appLines = GetAllApplicationLines(currecntAppId, service).Entities.ToList();
            tracingService.Trace("Get AllApplicationLines count:" + appLines.Count());

            int LineTypes=-1;

            if (GetLineType.Get(executionContext) != 0)
            {
                tracingService.Trace("got Discount lines parameter");
                LineTypes = GetLineType.Get(executionContext);

            }

            if(LineTypes!=-1)
            {
                
                appLines = appLines.Where(x => x.GetAttributeValue<OptionSetValue>(defra_applicationline.Fields.defra_linetype).Value == LineTypes).ToList();
            }

            tracingService.Trace("Try to delete application lines");
            foreach (var line in appLines)
            {
                service.Delete(defra_applicationline.EntityLogicalName ,line.Id);
            }

            tracingService.Trace("Finished");

        }

        private EntityCollection GetAllApplicationLines(Guid applicationId, IOrganizationService service)
        {
            // Get all the other application lines linked to the same application
            QueryExpression appLinesRulesQuery = new QueryExpression(ApplicationLine.EntityLogicalName)
            {
                ColumnSet =
                    new ColumnSet(
                        defra_applicationline.Fields.defra_linetype
                       ),
                Criteria = new FilterExpression()
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression(defra_applicationline.Fields.defra_applicationId, ConditionOperator.Equal, applicationId),
                        new ConditionExpression(defra_applicationline.Fields.StateCode, ConditionOperator.Equal, (int)ApplicationLineStates.Active),
                    }
                }
            };

            return service.RetrieveMultiple(appLinesRulesQuery);
        }
    }
}
