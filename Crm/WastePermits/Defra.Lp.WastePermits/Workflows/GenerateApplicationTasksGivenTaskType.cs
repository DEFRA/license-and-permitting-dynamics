using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WastePermits.DataAccess;
using WastePermits.Model.EarlyBound;
using WastePermits.Model.Internal;

namespace Defra.Lp.WastePermits.Workflows
{
   public class GenerateApplicationTasksGivenTaskType : WorkFlowActivityBase
    {

        #region Code activity parameters
        /// <summary>
        /// Application that will have it's application tasks processed
        /// </summary>
        [RequiredArgument]
        [Input("Application")]
        [ReferenceTarget(defra_application.EntityLogicalName)]
        public InArgument<EntityReference> Application { get; set; }

        /// <summary>
        /// Task type or checklist to be processed for the given application
        /// </summary>
        [RequiredArgument]
        [Input("Task Type 1")]
        [ReferenceTarget(defra_tasktype.EntityLogicalName)]
        public InArgument<EntityReference> TaskType1 { get; set; }

        [Input("Task Type 2")]
        [ReferenceTarget(defra_tasktype.EntityLogicalName)]
        public InArgument<EntityReference> TaskType2 { get; set; }

        [Input("Task Type 3")]
        [ReferenceTarget(defra_tasktype.EntityLogicalName)]
        public InArgument<EntityReference> TaskType3 { get; set; }

        [Input("Task Type 4")]
        [ReferenceTarget(defra_tasktype.EntityLogicalName)]
        public InArgument<EntityReference> TaskType4 { get; set; }

        [Input("Task Type 5")]
        [ReferenceTarget(defra_tasktype.EntityLogicalName)]
        public InArgument<EntityReference> TaskType5 { get; set; }

        [Input("Task Type 6")]
        [ReferenceTarget(defra_tasktype.EntityLogicalName)]
        public InArgument<EntityReference> TaskType6 { get; set; }

        [Input("Task Type 7")]
        [ReferenceTarget(defra_tasktype.EntityLogicalName)]
        public InArgument<EntityReference> TaskType7 { get; set; }

        [Input("Task Type 8")]
        [ReferenceTarget(defra_tasktype.EntityLogicalName)]
        public InArgument<EntityReference> TaskType8 { get; set; }

        [Input("Task Type 9")]
        [ReferenceTarget(defra_tasktype.EntityLogicalName)]
        public InArgument<EntityReference> TaskType9 { get; set; }

        [Input("Task Type 10")]
        [ReferenceTarget(defra_tasktype.EntityLogicalName)]
        public InArgument<EntityReference> TaskType10 { get; set; }

        #endregion
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            IOrganizationService organisationService = crmWorkflowContext.OrganizationService;

            tracingService.Trace("Started...");

          

            EntityReference application = Application.Get(executionContext);

            if (application == null || application.Id == Guid.Empty)
            {
                throw new ArgumentException("Application parameter is invalid", nameof(application));
            }

            // Read the task type ids to filter application tasks by
            List<Guid> taskTypeIds = new List<Guid>();
            AddIdToList(TaskType1.Get(executionContext), taskTypeIds);
            AddIdToList(TaskType2.Get(executionContext), taskTypeIds);
            AddIdToList(TaskType3.Get(executionContext), taskTypeIds);
            AddIdToList(TaskType4.Get(executionContext), taskTypeIds);
            AddIdToList(TaskType5.Get(executionContext), taskTypeIds);
            AddIdToList(TaskType6.Get(executionContext), taskTypeIds);
            AddIdToList(TaskType7.Get(executionContext), taskTypeIds);
            AddIdToList(TaskType8.Get(executionContext), taskTypeIds);
            AddIdToList(TaskType9.Get(executionContext), taskTypeIds);
            AddIdToList(TaskType10.Get(executionContext), taskTypeIds);
            Guid[] taskTypeIdArray = taskTypeIds.ToArray();

            var fetchCrBuilder = "";
            foreach(var task in taskTypeIdArray)
            {
                fetchCrBuilder += "<value>{"+ task.ToString() +"}</value>";
            }

            var fetch = @"<fetch>
                              <entity name='defra_applicationtaskdefinition'>
                                <attribute name='defra_applicationtaskdefinitionid' />
                                <filter type='and'>
                                  <condition attribute='defra_tasktypeid' operator='in'>
                                   "+ fetchCrBuilder+ @"
                                  </condition>
                                  <condition attribute='statecode' operator='eq' value='0' />
                                </filter>
                              </entity>
                            </fetch>";

            tracingService.Trace(string.Format("fetchXML: {0}", fetch));

            tracingService.Trace("Try to run RetrieveMultiple...");
            var appTaskdefs = organisationService.RetrieveMultiple(new FetchExpression(fetch)).Entities;
            if (appTaskdefs.Count == 0)
                return;

            tracingService.Trace("DataAccessApplicationTask");
            DataAccessApplicationTask dalAppTask = new DataAccessApplicationTask(organisationService, tracingService);

            tracingService.Trace("DataAccessApplication");
            DataAccessApplication dalApp = new DataAccessApplication(organisationService, tracingService);

            tracingService.Trace("GetApplicationTypeAndOwner");
            ApplicationTypesAndOwners applicationDetails = dalApp.GetApplicationTypeAndOwner(application.Id);


            foreach (var appTaskdef in appTaskdefs)
            {
                tracingService.Trace(string.Format("Create application task name: {0}", appTaskdef.Id.ToString()));
                dalAppTask.CreateApplicationTask(application.Id, applicationDetails.OwningUser, applicationDetails.OwningTeam, appTaskdef.Id);
            }

            tracingService.Trace("Finished");
        }

        private static void AddIdToList(EntityReference entityReference, List<Guid> idList)
        {
            if (entityReference != null && entityReference.Id != Guid.Empty)
            {
                idList.Add(entityReference.Id);
            }
        }
    }
}
