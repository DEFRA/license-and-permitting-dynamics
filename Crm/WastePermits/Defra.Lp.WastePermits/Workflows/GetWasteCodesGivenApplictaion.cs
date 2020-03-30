using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Lp.WastePermits.Workflows
{
   public class GetWasteCodesGivenApplictaion : WorkFlowActivityBase
    {
        [Output("DandRCodes")]
        public OutArgument<string> GetDandRCodes { get; set; }

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
                                  <entity name='annotation'>
                                   <attribute name='documentbody' /> 
                                     <filter type='and'>
                                      <condition attribute='subject' operator='eq' value='list of waste types' />
                                    </filter>
                                    <link-entity name='defra_application' from='defra_applicationid' to='objectid' link-type='inner' alias='ab'>
                                      <filter type='and'>
                                        <condition attribute='defra_applicationid' operator='eq' uiname='AB2395CD' uitype='defra_application' value='{" + context.PrimaryEntityId.ToString() + @"}' />
                                      </filter>
                                    </link-entity>
                                  </entity>
                                </fetch>";
            tracingService.Trace("FetchXML --> " + fetch);
            var noteEnt = organisationService.RetrieveMultiple(new FetchExpression(fetch)).Entities.FirstOrDefault();
            if (noteEnt == null)
                return;

            if (!noteEnt.Contains("documentbody"))
                return;

            var docBody = Convert.FromBase64String(noteEnt.Attributes["documentbody"].ToString());
            var bodyString = System.Text.Encoding.UTF8.GetString(docBody);
            tracingService.Trace("Note Body --> " + bodyString);

            if (bodyString == null)
            {
                tracingService.Trace("Body is empty retun");
                return;
            }

            var lines = bodyString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Count() < 1)
            {
                tracingService.Trace("Body has got not more than 1 line. Return.");
                return;

            }

            if (lines[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0] != "Code")
            {
                tracingService.Trace("Code has bot been found. Seems the note is not valid");
                return;
            }
            var codes = "";
            for(var i=1;i<lines.Count();i++)
            {
                tracingService.Trace(lines[i]);
                var l = lines[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (l.Count() > 1)
                    if (i != lines.Count()-1)
                        codes += l[0] + ";";
                    else
                        codes += l[0];

            }

            GetDandRCodes.Set(executionContext, codes);

        }
    }
}
