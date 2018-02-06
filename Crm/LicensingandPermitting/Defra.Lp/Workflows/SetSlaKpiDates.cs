// <copyright file="SetKpiInstanceWarningDate.cs" company="">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author></author>
// <date>05/02/2018</date>
// <summary></summary>
using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Defra.Lp.Workflows
{
    /// <summary>
    /// Main Class
    /// </summary>
    public class SetSlaKpiDates : WorkFlowActivityBase
    {
        /// <summary>
        /// Field name to be recalculated
        /// </summary>
        [RequiredArgument]
        [Input("SLA KPI Instance")]
        [ReferenceTarget("slakpiinstance")]
        public InArgument<EntityReference> SLAKPIInstance { get; set; }

        /// <summary>
        /// The new warning time for the KPI
        /// </summary>
        [Input("WarningTime")]
        public InArgument<DateTime> WarningTime { get; set; }

        /// <summary>
        /// The new failure time for the KPI
        /// </summary>
        [Input("FailureTime")]
        public InArgument<DateTime> FailureTime { get; set; }

        /// <summary>
        /// Main Execution Method
        /// </summary>
        /// <param name="executionContext">Activity Execution Context</param>
        /// <param name="crmWorkflowContext">Includes organisation service and trace service</param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            EntityReference kpiEntityRef = this.SLAKPIInstance.Get(executionContext);
            DateTime? warningTime = this.WarningTime.Get(executionContext);
            DateTime? failureTime = this.FailureTime.Get(executionContext);

            if (!warningTime.HasValue && !failureTime.HasValue)
            {
                // No date to set
                return;
            }

            //
            Entity kpiEntity = new Entity(kpiEntityRef.LogicalName, kpiEntityRef.Id);

            if (warningTime.HasValue)
            {
                kpiEntity[Model.SlaKpiInstance.WarningTime] = warningTime.Value;
                kpiEntity[Model.SlaKpiInstance.ComputedWarningTime] = warningTime.Value;
            }
            if (failureTime.HasValue)
            {
                kpiEntity[Model.SlaKpiInstance.FailureTime] = failureTime.Value;
                kpiEntity[Model.SlaKpiInstance.ComputedFailureTime] = failureTime.Value;
            }
            crmWorkflowContext.OrganizationService.Update(kpiEntity);
        }
    }
}