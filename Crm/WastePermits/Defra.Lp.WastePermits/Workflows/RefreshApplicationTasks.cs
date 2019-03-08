using System;
using System.Collections.Generic;
using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using WastePermits.DataAccess;
using WastePermits.Model.EarlyBound;
using WastePermits.Model.Internal;

namespace Defra.Lp.WastePermits.Workflows
{
    /// <summary>
    /// Code activity adds and removes application tasks to an application depending on 
    /// the linked application lines
    /// </summary>
    public class RefreshApplicationTasks: WorkFlowActivityBase
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

        /// <summary>
        /// Main code activity methoid
        /// </summary>
        /// <param name="executionContext">Standard CRM execution context</param>
        /// <param name="crmWorkflowContext">Standard CRM workflow context</param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            // 1. SETUP

            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            IOrganizationService organisationService = crmWorkflowContext.OrganizationService;
            tracingService.Trace("Started");

            // Validation
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

            // Prepare the DALs
            DataAccessApplicationTask dalAppTask = new DataAccessApplicationTask(organisationService, tracingService);
            DataAccessApplication dalApp = new DataAccessApplication(organisationService, tracingService);

            // 2. PROCESSING

            // Get Application Type, SubType and owners
            ApplicationTypesAndOwners applicationDetails = dalApp.GetApplicationTypeAndOwner(application.Id);

            // Which tasks should be linked to the application?
            List<Guid> applicableTasks = dalAppTask.GetTaskDefinitionIdsThatApplyToApplication(application.Id, applicationDetails.ApplicationType, applicationDetails.ApplicationSubType, taskTypeIdArray) ?? new List<Guid>();

            // Which tasks are already linked?
            List<ApplicationTaskAndDefinitionId> existingTasks = dalAppTask.GetApplicationTaskIdsLinkedToApplication(application.Id, taskTypeIdArray) ?? new List<ApplicationTaskAndDefinitionId>();

            // Deactivate application tasks that no longer apply
            List<Guid> tasksToRemove = existingTasks.Where(existingTask => !applicableTasks.Contains(existingTask.ApplicationTaskDefinitionId))
                .Select(t => t.ApplicationTaskId)
                .ToList();
            tasksToRemove.ForEach(dalAppTask.DeactivateApplicationTask);

            // Create application tasks that apply
            List<Guid> tasksToAdd = applicableTasks.Where(applicableTask => existingTasks.All(t => t.ApplicationTaskDefinitionId != applicableTask)).ToList();
            tasksToAdd.ForEach(newtask => dalAppTask.CreateApplicationTask(application.Id, applicationDetails.OwningUser, applicationDetails.OwningTeam, newtask));

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
