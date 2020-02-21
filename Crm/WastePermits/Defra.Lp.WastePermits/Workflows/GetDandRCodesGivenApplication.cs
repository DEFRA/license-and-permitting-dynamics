using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Defra.Lp.WastePermits.Workflows.WorkFlowActivityBase;

namespace Defra.Lp.WastePermits.Workflows
{
    public class GetDandRCodesGivenApplication : WorkFlowActivityBase
    {
        [Output("Waste Codes")]
        public OutArgument<string> GetWasteCodes { get; set; }

        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            #region Create tracing and organisation service objects
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            IOrganizationService organisationService = crmWorkflowContext.OrganizationService;
            var context = crmWorkflowContext.WorkflowExecutionContext;
            tracingService.Trace("Started GetDandRCodesGivenApplication...");
            #endregion

            tracingService.Trace("Retrieve FetchXML, Column names and Schema names.....");
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='defra_applicationanswer'>
                             <filter type='and'>
                                                          <condition attribute='defra_question' operator='eq' uiname='Waste recovery codes' uitype='defra_applicationquestion' value='{1B1BE2EC-1DF0-E911-A812-000D3ABACF5B}' />
                        <condition attribute='defra_application' operator='eq'  uitype='defra_application' value='{" + context.PrimaryEntityId.ToString() + @"}' />
                                                        </filter>
                           <attribute name='defra_answertext' />
                        <attribute name='defra_applicationlineid' />
                              <link-entity name='defra_applicationline' from='defra_applicationlineid' to='defra_applicationlineid' link-type='inner' alias='ad'>
                              <link-entity name='defra_item' from='defra_itemid' to='defra_itemid' link-type='inner' alias='ae' >
                        <attribute name='defra_code'/>

                        </link-entity>
                            </link-entity>
                          </entity>
                        </fetch>
                            ";
            tracingService.Trace("FetchXML --> " + fetch);
            var answers = organisationService.RetrieveMultiple(new FetchExpression(fetch)).Entities.ToList().Select(x=> new { ansText=x.GetAttributeValue<string>("defra_answertext"),code=x.GetAttributeValue<AliasedValue>("ae.defra_code").Value }).ToList();

            if (answers.Count() == 0)
                return;
            
            var cwCodes = "";
            for (var i = 0; i < answers.Count(); i++)
            {
               
                    if (i != answers.Count() - 1)
                        cwCodes += answers[i].code +": "+ answers[i].ansText + ";";
                    else
                        cwCodes += answers[i].code + ": " + answers[i].ansText;

                
            }

            GetWasteCodes.Set(executionContext, cwCodes);
        }

    }
}

