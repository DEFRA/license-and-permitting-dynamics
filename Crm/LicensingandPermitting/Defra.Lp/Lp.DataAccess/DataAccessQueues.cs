using Lp.Model.EarlyBound;
using Microsoft.Crm.Sdk.Messages;

namespace Lp.DataAccess
{
    using System;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Model.Crm;

    public static class DataAccessQueues
    {

        /// <summary>
        /// Get all active queue items for the given record
        /// </summary>
        /// <param name="service">CRM service</param>
        /// <param name="queueItemObjectId">Guid for the regarding object</param>
        /// <returns>Collection of active queue items</returns>
        public static EntityCollection GetQueueItems(IOrganizationService service, Guid queueItemObjectId)
        {
            QueryExpression qe = new QueryExpression
            {
                EntityName = QueueItem.EntityLogicalName,
                ColumnSet = new ColumnSet {AllColumns = false}
            };

            ConditionExpression isRegardingObjectId = new ConditionExpression
            {
                AttributeName = "objectid",
                Operator = ConditionOperator.Equal,
                Values = { queueItemObjectId }
            };

            ConditionExpression isActive = new ConditionExpression
            {
                AttributeName = "statecode",
                Operator = ConditionOperator.Equal,
                Values = { (int)QueueItemState.Active }
                
            };

            FilterExpression filter = new FilterExpression();
            filter.Conditions.Add(isRegardingObjectId);
            filter.Conditions.Add(isActive);

            qe.Criteria = filter;

            return service.RetrieveMultiple(qe);
        }


        /// <summary>
        /// Deactivates all active queue items for the given record
        /// </summary>
        /// <param name="service">CRM service</param>
        /// <param name="queueItemObjectId">Guid for the regarding object</param>
        public static void DeactivateQueueItems(IOrganizationService service, Guid queueItemObjectId)
        {
            // Get all active queue items for the given object
            EntityCollection activeQueueItems = GetQueueItems(service, queueItemObjectId);

            if (activeQueueItems?.Entities == null)
            {
                // Nothing to deactivate
                return;
            }

            // Deactivate all active queue items
            foreach (Entity queueItem in activeQueueItems.Entities)
            {
                DeactivateQueueItem(service, queueItem);
            }
        }

        /// <summary>
        /// Deactivates the given queue item
        /// </summary>
        /// <param name="service"></param>
        /// <param name="queueItem"></param>
        private static void DeactivateQueueItem(IOrganizationService service, Entity queueItem)
        {
            SetStateRequest state = new SetStateRequest
            {
                State = new OptionSetValue((int) QueueItemState.Inactive),
                Status = new OptionSetValue((int) QueueItem_StatusCode.Inactive),
                EntityMoniker = queueItem.ToEntityReference()
            };
            service.Execute(state);
        }
    }
}