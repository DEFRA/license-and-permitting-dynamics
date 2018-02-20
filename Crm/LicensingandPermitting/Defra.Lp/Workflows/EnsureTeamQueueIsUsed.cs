
// <copyright file="EnsureTeamQueueIsUsed.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>2/16/2018 2:55:24 PM</date>
// <summary> Function ensure that the given Queue Item is set to use a Team Queue 
// and sets the Worked By to the user if the queue item was originall set using a user's default queue
// </summary>
using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Defra.Lp.Workflows
{
    /// <summary>
    /// Main Class
    /// </summary>
    public class EnsureTeamQueueIsUsed : WorkFlowActivityBase
    {
        /// <summary>
        /// Field name to be recalculated
        /// </summary>
        [RequiredArgument]
        [Input("Queue Item")]
        [ReferenceTarget("queueitem")]
        public InArgument<EntityReference> QueueItemEntityReference { get; set; }

        /// <summary>
        /// Function ensure that the given Queue Item is set to use a Team Queue
        /// and sets the Worked By to the user if the queue item was originall set using a user's default queue
        /// </summary>
        /// <param name="executionContext">Code activity execution context</param>
        /// <param name="crmWorkflowContext">Contains the organisation service and trace service</param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            // Validation
            if (crmWorkflowContext == null)
            {
                throw new ArgumentNullException(nameof(crmWorkflowContext));
            }

            // 1. Get the Queue currently set for the Queue Item we are interested in
            EntityReference queueItemEntityReference = QueueItemEntityReference.Get(executionContext);
            Entity queueItem = GetQueueItem(crmWorkflowContext, queueItemEntityReference);
            EntityReference queue = queueItem[Model.QueueItem.Queue] as EntityReference;
            if (queue == null)
            {
                // There is no queue, nothing to do here.
                return;
            }

            // 2. Get the user that has this Queue as a Default Queue
            var user = GetUserForQueue(crmWorkflowContext, queue);

            // 3. If we have a user, it means we need to switch the queue item to use a team queue
            if (user != null)
            {
                var queueToUse = user[Model.User.DefaultTeamQueue] as EntityReference;
                var target = queueItem[Model.QueueItem.Target] as EntityReference;

                // Is there no default team queue set-up for the user?
                if (queueToUse == null)
                {
                    // No default team queue means we can't change the queue, nothing to do here
                    return;
                }

                // Move a record from a source queue to a destination queue
                // by using the AddToQueue request message.
                AddToQueueRequest routeRequest = new AddToQueueRequest
                {
                    SourceQueueId = queue.Id,
                    Target = target,
                    DestinationQueueId = queueToUse.Id
                };
                crmWorkflowContext.OrganizationService.Execute(routeRequest);


                // 4. And finally, set the worked on for the queueitem to be the user.
                // Create an instance of an existing queueitem in order to specify 
                // the user that will be working on it using PickFromQueueRequest.
                PickFromQueueRequest pickFromQueueRequest = new PickFromQueueRequest
                {
                    QueueItemId = queueItem.Id,
                    WorkerId = user.Id
                };
                crmWorkflowContext.OrganizationService.Execute(pickFromQueueRequest);
            }
        }

        /// <summary>
        /// Gets the user entity if the queue item is assigned to a user's default queue
        /// </summary>
        /// <param name="crmWorkflowContext">Includes the Organisation Service</param>
        /// <param name="queue">The queue that will be checked to see if it is a user's default queue</param>
        /// <returns>The user record if a user's default queue was passed in</returns>
        private static Entity GetUserForQueue(LocalWorkflowContext crmWorkflowContext, EntityReference queue)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = Model.User.LogicalName,
                ColumnSet = new ColumnSet(Model.User.DefaultQueue, Model.User.DefaultTeamQueue),
                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression
                        {
                            Conditions =
                            {
                                new ConditionExpression(Model.User.DefaultQueue, ConditionOperator.Equal, queue.Id),
                            },
                        }
                    }
                }
            };
            EntityCollection users = crmWorkflowContext.OrganizationService.RetrieveMultiple(query);
            if (users?.Entities != null && users.Entities.Count > 0)
            {
                return users[0];
            }
            return null;
        }

        /// <summary>
        /// Gets the queue item for the given entity reference
        /// </summary>
        /// <param name="crmWorkflowContext">Includes the Organisation Service</param>
        /// <param name="queueItemEntityReference">The queue that will be checked to see if it is a user's default queue</param>
        /// <returns></returns>
        private static Entity GetQueueItem(LocalWorkflowContext crmWorkflowContext, EntityReference queueItemEntityReference)
        {
            Entity queueItem = crmWorkflowContext.OrganizationService.Retrieve
                (queueItemEntityReference.LogicalName,
                    queueItemEntityReference.Id,
                    new ColumnSet(Model.QueueItem.Queue, Model.QueueItem.Target, Model.QueueItem.WorkedBy)
                );
            return queueItem;
        }
    }
}
