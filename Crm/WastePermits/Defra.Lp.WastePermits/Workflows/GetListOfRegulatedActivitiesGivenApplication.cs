using Core.Helpers.Extensions;
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
using WastePermits.Model.EarlyBound;

namespace Defra.Lp.WastePermits.Workflows
{
    public class GetListOfRegulatedActivitiesGivenApplication : WorkFlowActivityBase
    {
        [Output("StandardRuleRefList")]
        public OutArgument<string> StandardRuleRefList { get; set; }

        [Output("ActivityChargeCodeList")]
        public OutArgument<string> ActivityChargeCodeList { get; set; }

        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {

            if (crmWorkflowContext == null)
            {
                throw new ArgumentNullException("crmWorkflowContext");
            }

           var tracingService = executionContext.GetExtension<ITracingService>();
           var service = crmWorkflowContext.OrganizationService;
           var context = crmWorkflowContext.WorkflowExecutionContext;
            var applicationId = context.PrimaryEntityId;

            SetListOfAllActivitiesGivenApplication(service, executionContext, applicationId.ToString());


        }

        public void SetListOfAllActivitiesGivenApplication(IOrganizationService service, CodeActivityContext context, string appId)
        {

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
                    <link-entity name='defra_item' from='defra_itemid' to='defra_itemid' link-type='outer' alias='item'>
                      <attribute name='defra_code' />
                          <attribute name='defra_name' />
                            <attribute name='defra_itemtypeid' />
                      
                    </link-entity>
                    <link-entity name='defra_standardrule' from='defra_standardruleid' to='defra_standardruleid' visible='false' link-type='outer' alias='standardrule'>
                      <attribute name='defra_rulesnamegovuk' />
                        <attribute name='defra_name' />

                    </link-entity>
                  </entity>
                </fetch>";

            RetrieveMultipleRequest fetchRequest = new RetrieveMultipleRequest
            {
                Query = new FetchExpression(fetchXml)
            };

            var results = ((RetrieveMultipleResponse)service.Execute(fetchRequest)).EntityCollection;
            var aliasItem = "item";
            var aliasSr = "standardrule";
            var nonSrCodeList = "";
            var srCodeList = "";

            if (results != null && results.Entities.Count > 0)
            {
                for (int i = 0; i < results.Entities.Count; i++)
                {
                    if (results[i].Contains(defra_standardrule.Fields.defra_standardruleId))
                    {
                        var code = results[i].GetAliasedAttributeText($"{aliasSr}.{defra_standardrule.Fields.defra_name}");
                        var name = results[i].GetAliasedAttributeText($"{aliasSr}.{defra_standardrule.Fields.defra_rulesnamegovuk}");
                        var permit = $"{code} - {name}";

                        if (i == 0)
                        {
                            srCodeList = code;
                        }
                        else
                        {
                            srCodeList = srCodeList + ";" + code;
                        }
                    }
                    // Only activities for non SR
                    else if (results[i].Contains(defra_item.Fields.defra_itemId) && (results[i].Contains("item.defra_itemtypeid") && (results[i].GetAliasedAttributeId("item.defra_itemtypeid") == System.Guid.Parse("493f0abd-34d9-e811-a96e-000d3a23443b"))))
                    {

                        var code = results[i].GetAliasedAttributeText($"{aliasItem}.{defra_item.Fields.defra_code}");
                        var name = results[i].GetAliasedAttributeText($"{aliasItem}.{defra_item.Fields.defra_name}");

                        var permit = $"{code} - {name}";
                        if (i == 0)
                        {
                            nonSrCodeList = code;
                        }
                        else

                        {
                            nonSrCodeList = nonSrCodeList + ";" + code;
                        }

                    }

                }
            }
            ActivityChargeCodeList.Set(context, nonSrCodeList);
            StandardRuleRefList.Set(context, srCodeList);

        }

    }
}
