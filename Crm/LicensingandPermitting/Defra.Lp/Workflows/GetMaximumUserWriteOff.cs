
// <copyright file="GetMaximumUserWriteOff.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author>Hugo Herrera</author>
// <date>4/13/2018 11:21:17 AM</date>
// <summary>Code Activity queries CRM teams for the maximum write-off the user can make</summary>

using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Lp.DataAccess;

namespace Defra.Lp.Workflows
{
    // Main Code Activity class  
    public class GetMaximumUserWriteOff : WorkFlowActivityBase
    {

        [Input("User")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> User { get; set; }

        [Output("MaximumWriteOffValue")]
        [RequiredArgument]
        public OutArgument<Money> MaximumWriteOffValue { get; set; }

        /// <summary>
        /// Main code activity function
        /// </summary>
        /// <param name="executionContext">Activity Context</param>
        /// <param name="crmWorkflowContext">CRM Contenxt inc servie</param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            var tracingService = executionContext.GetExtension<ITracingService>();
            tracingService.Trace("GetMaximumUserWriteOff starting...");

            try
            {
                // 1. Validation
                ValidateNotNull(crmWorkflowContext);
                EntityReference user = this.User.Get<EntityReference>(executionContext);
                //ValidateNotNull(user);
                if (user == null || user.Id == Guid.Empty)
                {
                    // Get the user from the context
                    user = new EntityReference(Model.User.LogicalName, crmWorkflowContext.WorkflowExecutionContext.UserId);
                }

                // 2. Query CRM for maximum write off user can make
                tracingService.Trace("Calling GetMaximumUserCanWriteOff...");
                Money maxWriteOff = crmWorkflowContext.OrganizationService.GetMaximumUserCanWriteOff(user.Id);

                // 3. Return the response
                tracingService.Trace("maxWriteOff={0}", maxWriteOff);
                this.MaximumWriteOffValue.Set(executionContext, maxWriteOff);
                tracingService.Trace("GetMaximumUserWriteOff end.");

            }
            catch (Exception ex)
            {
                // Todo: Log the Error
                tracingService.Trace("Exception: " + ex);
                throw ex;
            }
        }
    }
}
