// <copyright file="RecalculateRollup.cs" company="">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author></author>
// <date>05/02/2018</date>
// <summary>Workflow Code Activity recalculates rollup fields on demand.</summary>
using System;
using System.Activities;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace Defra.Lp.Workflows
{
    /// <summary>
    /// Main Class
    /// </summary>
    public class RecalculateRollup : WorkFlowActivityBase
    {
        /// <summary>
        /// Field name to be recalculated
        /// </summary>
        [RequiredArgument]
        [Input("FieldName")]
        [Default("")]
        public InArgument<String> FieldName { get; set; }

        /// <summary>
        /// Dynamics URL for the record to recalculate
        /// </summary>
        [RequiredArgument]
        [Input("Parent Record URL")]
        [ReferenceTarget("")]
        public InArgument<String> ParentRecordURL { get; set; }

        /// <summary>
        /// Main Execution Method
        /// </summary>
        /// <param name="executionContext">Activity Execution Context</param>
        /// <param name="crmWorkflowContext">Includes organisation service and trace service</param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            string fieldName = this.FieldName.Get(executionContext);
            string parentRecordUrl = this.ParentRecordURL.Get(executionContext);
            if (string.IsNullOrEmpty(parentRecordUrl))
            {
                return;
            }
            string[] urlParts = parentRecordUrl.Split("?".ToArray());
            string[] urlParams = urlParts[1].Split("&".ToCharArray());
            string parentObjectTypeCode = urlParams[0].Replace("etc=", "");
            string parentId = urlParams[1].Replace("id=", "");
            crmWorkflowContext.TracingService.Trace("ParentObjectTypeCode=" + parentObjectTypeCode + "--ParentId=" + parentId);

            string parentEntityName = GetEntityNameFromCode(parentObjectTypeCode, crmWorkflowContext.OrganizationService);
            CalculateRollupFieldRequest calculateRollup = new CalculateRollupFieldRequest();
            calculateRollup.FieldName = fieldName;
            calculateRollup.Target = new EntityReference(parentEntityName, new Guid(parentId));
            crmWorkflowContext.OrganizationService.Execute(calculateRollup);
        }

        /// <summary>
        /// Query the Metadata to get the Entity Schema Name from the Object Type Code
        /// </summary>
        /// <param name="objectTypeCode">Entity type code</param>
        /// <param name="service">Organisation Service</param>
        /// <returns>Entity Schema Name</returns>
        private string GetEntityNameFromCode(string objectTypeCode, IOrganizationService service)
        {
            MetadataFilterExpression entityFilter = new MetadataFilterExpression(LogicalOperator.And);
            entityFilter.Conditions.Add(new MetadataConditionExpression("ObjectTypeCode", MetadataConditionOperator.Equals, Convert.ToInt32(objectTypeCode)));
            EntityQueryExpression entityQueryExpression = new EntityQueryExpression()
            {
                Criteria = entityFilter
            };
            RetrieveMetadataChangesRequest retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest()
            {
                Query = entityQueryExpression,
                ClientVersionStamp = null
            };
            RetrieveMetadataChangesResponse response = (RetrieveMetadataChangesResponse)service.Execute(retrieveMetadataChangesRequest);

            EntityMetadata entityMetadata = response.EntityMetadata[0];
            return entityMetadata.SchemaName.ToLower();
        }
    }
}