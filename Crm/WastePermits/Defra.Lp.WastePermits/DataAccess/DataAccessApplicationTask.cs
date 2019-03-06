namespace WastePermits.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    using Core.DataAccess.Base;
    using Core.Helpers.Extensions;
    using Model.EarlyBound;
    using Model.Internal;

    /// <summary>
    /// Data access class provides methods to read/write Application Task records
    /// </summary>
    public class DataAccessApplicationTask : DataAccessBase
    {
        /// <summary>
        /// Constructor for DAL class
        /// </summary>
        /// <param name="organisationService"></param>
        /// <param name="tracingService"></param>
        public DataAccessApplicationTask(IOrganizationService organisationService, ITracingService tracingService)
            : base(organisationService, tracingService) { }

        /// <summary>
        /// Returns a list of task definitions that apply to an application
        /// </summary>
        /// <param name="applicationId">Application Guid</param>
        /// <param name="applicationSubTypeId">Optional filter by Application Sub Type, otherwise check for null</param>
        /// /// <param name="applicationTypeValue">Optional filter by Application Type, otherwise check for null</param>
        /// <param name="filterByTaskTypeIds">Task Types to filter by</param>
        /// <returns>List of defra_applicationtaskdefinition ids </returns>
        public List<Guid> GetTaskDefinitionIdsThatApplyToApplication(Guid applicationId, int? applicationTypeValue, Guid? applicationSubTypeId, params Guid[] filterByTaskTypeIds)
        {
            TracingService.Trace($"GetTaskDefinitionIdsThatApplyToApplication({applicationId}, {filterByTaskTypeIds}) Start...");

            // Query defra_applicationtaskdefinition
            var query = new QueryExpression(defra_applicationtaskdefinition.EntityLogicalName) { Distinct = true };
            query.ColumnSet.AddColumns(
                defra_applicationtaskdefinition.Fields.defra_name,
                defra_applicationtaskdefinition.Fields.defra_shortname,
                defra_applicationtaskdefinition.Fields.defra_applicationtaskdefinitionId);
            query.Criteria.AddCondition(defra_applicationtaskdefinition.Fields.StateCode, ConditionOperator.Equal, (int)defra_applicationtaskdefinitionState.Active);

            // Link to defra_itemapplicationtaskdefinition - the linker entity
            var linkQuery = query.AddLink(
                defra_itemapplicationtaskdefinition.EntityLogicalName,
                defra_applicationtaskdefinition.Fields.defra_tasktypeid,
                defra_itemapplicationtaskdefinition.Fields.defra_applicationtasktypeid);
            linkQuery.LinkCriteria.AddCondition(defra_itemapplicationtaskdefinition.Fields.StateCode, ConditionOperator.Equal, (int)defra_itemapplicationtaskdefinitionState.Active);
            linkQuery.LinkCriteria.AddCondition(defra_itemapplicationtaskdefinition.Fields.defra_scope, ConditionOperator.Equal, (int)defra_application_task_scope.Application);

            // Filter defra_itemapplicationtaskdefinition by application type
            if (applicationTypeValue.HasValue)
            {
                linkQuery.LinkCriteria.AddCondition(defra_itemapplicationtaskdefinition.Fields.defra_applicationtype, ConditionOperator.Equal, applicationTypeValue.Value);
            }
            else
            {
                linkQuery.LinkCriteria.AddCondition(defra_itemapplicationtaskdefinition.Fields.defra_applicationtype, ConditionOperator.Null);
            }

            // Filter defra_itemapplicationtaskdefinition by application sub type
            if (applicationSubTypeId.HasValue)
            {
                linkQuery.LinkCriteria.AddCondition(defra_itemapplicationtaskdefinition.Fields.defra_application_subtype, ConditionOperator.Equal, applicationSubTypeId.Value);
            }
            else
            {
                linkQuery.LinkCriteria.AddCondition(defra_itemapplicationtaskdefinition.Fields.defra_application_subtype, ConditionOperator.Null);
            }

            // Link to defra_standardrule
            var linkToStandardRule = linkQuery.AddLink(
                defra_standardrule.EntityLogicalName,
                defra_itemapplicationtaskdefinition.Fields.defra_standardruleid,
                defra_standardrule.Fields.defra_standardruleId);
            linkToStandardRule.LinkCriteria.AddCondition(defra_standardrule.Fields.StateCode, ConditionOperator.Equal, (int)defra_standardruleState.Active);

            // Link to defra_applicationline
            var linkToAppLine = linkToStandardRule.AddLink(
                defra_applicationline.EntityLogicalName,
                defra_standardrule.Fields.defra_standardruleId,
                defra_applicationline.Fields.defra_standardruleId);
            linkToAppLine.LinkCriteria.AddCondition(defra_applicationline.Fields.StateCode, ConditionOperator.Equal, (int)defra_applicationlineState.Active);
            linkToAppLine.LinkCriteria.AddCondition(defra_applicationline.Fields.defra_applicationId, ConditionOperator.Equal, applicationId);

            // Link to defra_tasktype
            var linkToType = query.AddLink(defra_tasktype.EntityLogicalName, defra_applicationtaskdefinition.Fields.defra_tasktypeid, defra_tasktype.Fields.defra_tasktypeId);

            // Filter by Active Task Types
            linkToType.LinkCriteria.AddCondition(defra_tasktype.Fields.StateCode, ConditionOperator.Equal, (int)defra_tasktypeState.Active);

            // Filter by specific Task Types (e.g. duly making checklist)
            if (filterByTaskTypeIds != null && filterByTaskTypeIds.Length > 0)
            {
                var filterTaskType = new FilterExpression();
                linkToType.LinkCriteria.AddFilter(filterTaskType);
                filterTaskType.FilterOperator = LogicalOperator.Or;
                foreach (Guid taskTypeId in filterByTaskTypeIds)
                {
                    filterTaskType.AddCondition(defra_tasktype.Fields.defra_tasktypeId, ConditionOperator.Equal, taskTypeId);
                }
            }

            // Query CRM
            EntityCollection entityCollectionResult = OrganisationService.RetrieveMultiple(query);

            // Return a list of defra_applicationtaskdefinition ids 
            List<Guid> retVal = null;
            if (entityCollectionResult?.Entities != null)
            {
                TracingService.Trace("GetTaskDefinitionIdsThatApplyToApplication() Returning data");
                retVal = entityCollectionResult.Entities
                    .Select(e => e.GetAttributeIdOrDefault(defra_applicationtaskdefinition.Fields
                        .defra_applicationtaskdefinitionId)).ToList();
            }

            retVal?.ForEach(t => TracingService.Trace($"GetTaskDefinitionIdsThatApplyToApplication - {t}"));
            TracingService.Trace($"GetTaskDefinitionIdsThatApplyToApplication() Done");
            return retVal;
        }

        /// <summary>
        /// Returns a list of task definitions that apply to an application
        /// </summary>
        /// <param name="applicationId">Application Guid</param>
        /// <param name="filterByTaskTypeIds">Task Types to filter by</param>
        /// <returns>List of defra_applicationtaskdefinition ids </returns>
        public List<ApplicationTaskAndDefinitionId> GetApplicationTaskIdsLinkedToApplication(Guid applicationId, params Guid[] filterByTaskTypeIds)
        {
            TracingService.Trace($"GetApplicationTaskIdsLinkedToApplication({applicationId}, {filterByTaskTypeIds}) Start...");

            // Select defra_applicationtask records 
            var query = new QueryExpression(defra_applicationtask.EntityLogicalName);
            query.ColumnSet.AddColumns(defra_applicationtask.Fields.defra_applicationtaskId, defra_applicationtask.Fields.defra_applicationtaskdefinitionid);
            query.Criteria.AddCondition(defra_applicationtask.Fields.StateCode, ConditionOperator.Equal, (int)defra_applicationtaskState.Active);
            query.Criteria.AddCondition(defra_applicationtask.Fields.defra_applicationid, ConditionOperator.Equal, applicationId);

            // Link to defra_applicationtaskdefinition
            var linkToTaskDefinition = query.AddLink(
                defra_applicationtaskdefinition.EntityLogicalName,
                defra_applicationtask.Fields.defra_applicationtaskdefinitionid,
                defra_applicationtaskdefinition.Fields.defra_applicationtaskdefinitionId);

            linkToTaskDefinition.LinkCriteria.AddCondition(
                defra_applicationtaskdefinition.Fields.StateCode,
                ConditionOperator.Equal,
                (int)defra_applicationtaskdefinitionState.Active);

            // Filter by specific Task Types (e.g. duly making checklist)
            if (filterByTaskTypeIds != null && filterByTaskTypeIds.Length > 0)
            {
                var filterByTaskType = new FilterExpression();
                linkToTaskDefinition.LinkCriteria.AddFilter(filterByTaskType);

                filterByTaskType.FilterOperator = LogicalOperator.Or;

                foreach (Guid taskTypeId in filterByTaskTypeIds)
                {
                    filterByTaskType.AddCondition(defra_tasktype.Fields.defra_tasktypeId, ConditionOperator.Equal, taskTypeId);
                }
            }

            // Query CRM
            EntityCollection entityCollectionResult = OrganisationService.RetrieveMultiple(query);
            List<ApplicationTaskAndDefinitionId> retVal = null;
            // Return a list of defra_applicationtaskdefinition ids 
            if (entityCollectionResult?.Entities != null)
            {
                TracingService.Trace($"GetApplicationTaskIdsLinkedToApplication() Returning data");
                retVal = entityCollectionResult.Entities
                    .Select(e => new ApplicationTaskAndDefinitionId
                    {
                        ApplicationTaskId = e.GetAttributeIdOrDefault(defra_applicationtask.Fields.defra_applicationtaskId),
                        ApplicationTaskDefinitionId = e.GetAttributeIdOrDefault(defra_applicationtask.Fields.defra_applicationtaskdefinitionid)
                    }).ToList();
            }

            retVal?.ForEach(t => TracingService.Trace($"GetApplicationTaskIdsLinkedToApplication - ApplicationTaskId={t.ApplicationTaskId}, ApplicationTaskDefinitionId={t.ApplicationTaskDefinitionId}"));
            TracingService.Trace("GetApplicationTaskIdsLinkedToApplication() Done");
            return retVal;
        }

        /// <summary>
        /// Deactivates a defra_applicationtask record
        /// </summary>
        /// <param name="applicationTaskIdGuid">defra_applicationtask id</param>
        public void DeactivateApplicationTask(Guid applicationTaskIdGuid)
        {
            TracingService.Trace($"GetApplicationTaskIdsLinkedToApplication({applicationTaskIdGuid})");

            var state = new SetStateRequest
                {
                    State = new OptionSetValue((int) defra_applicationtaskState.Inactive),
                    Status = new OptionSetValue((int)defra_applicationtask_StatusCode.Inactive),
                    EntityMoniker = new EntityReference(defra_applicationtask.EntityLogicalName, applicationTaskIdGuid)
                };
            OrganisationService.Execute(state);
        }

        /// <summary>
        /// Creates a defra_applicationtask record
        /// </summary>
        /// <param name="applicationId">defra_application id to link the record to</param>
        /// <param name="ownerTeamId"></param>
        /// <param name="applicationTaskDefinitionId">defra_applicationtaskdefinition id to link the record to</param>
        /// <param name="ownerUserId"></param>
        /// <returns>Newly created record id</returns>
        public Guid CreateApplicationTask(Guid applicationId, Guid? ownerUserId, Guid? ownerTeamId, Guid applicationTaskDefinitionId)
        {
            TracingService.Trace($"CreateApplicationTask(applicationId={applicationId}, ownerUserId={ownerUserId}, ownerTeamId={ownerTeamId},applicationTaskDefinitionId={applicationTaskDefinitionId})");

            Entity applicationTask = new Entity(defra_applicationtask.EntityLogicalName);
            applicationTask.Attributes.Add(defra_applicationtask.Fields.defra_applicationid, new EntityReference(defra_application.EntityLogicalName, applicationId));
            applicationTask.Attributes.Add(defra_applicationtask.Fields.defra_applicationtaskdefinitionid, new EntityReference(defra_applicationtaskdefinition.EntityLogicalName, applicationTaskDefinitionId));

            if (ownerUserId.HasValue)
            {
                applicationTask.Attributes.Add(defra_applicationtask.Fields.OwnerId, new EntityReference(SystemUser.EntityLogicalName, ownerUserId.Value));
            }
            else if (ownerTeamId.HasValue)
            {
                applicationTask.Attributes.Add(defra_applicationtask.Fields.OwnerId, new EntityReference(Team.EntityLogicalName, ownerTeamId.Value));
            }
            return OrganisationService.Create(applicationTask);
        }
    }
}
