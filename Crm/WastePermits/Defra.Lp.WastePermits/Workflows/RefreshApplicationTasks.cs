using System;
using System.Collections.Generic;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using WastePermits.DataAccess;
using WastePermits.Model.EarlyBound;

namespace Defra.Lp.WastePermits.Workflows
{
    /// <summary>
    /// Code activity adds and removes application tasks to an application depending on 
    /// the linked application lines
    /// </summary>
    public class RefreshApplicationTasks: WorkFlowActivityBase
    {

        [RequiredArgument]
        [Input("Application")]
        [ReferenceTarget(defra_application.EntityLogicalName)]
        public InArgument<EntityReference> Application { get; set; }

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

        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            // Set-up
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            IOrganizationService organisationService = crmWorkflowContext.OrganizationService;
            tracingService.Trace("Started");

            // Validation
            var application = Application.Get(executionContext);
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

            // Call data access layer to add/remove defra_applicationtask records as needed
            tracingService.Trace("Calling DataAccessApplicationTask.AddOrRemoveApplicationTasksAsRequired()");
            DataAccessApplicationTask dal = new DataAccessApplicationTask(organisationService, tracingService);
            dal.AddOrRemoveApplicationTasksAsRequired(application.Id, taskTypeIds.ToArray());
            tracingService.Trace("Done");
        }

        /// <summary>
        /// Adds the Id from an entity reference into a list, but only if it exists
        /// </summary>
        /// <param name="entityReference">Entity reference to extract the Guid from</param>
        /// <param name="idList">List to add Guids to</param>
        private static void AddIdToList(EntityReference entityReference, List<Guid> idList)
        {
            if (entityReference != null && entityReference.Id != Guid.Empty)
            {
                idList.Add(entityReference.Id);
            }
        }
    }
}
