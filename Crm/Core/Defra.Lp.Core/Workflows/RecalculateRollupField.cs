// <copyright file="RecalculateRollupField.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>05/02/2018</date>
// <summary>Workflow activity forces recalculation of rollup fields </summary>

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

namespace Defra.Lp.Core.Workflows
{
    /// <summary>
    /// Main Activity Class
    /// </summary>
    public class RecalculateRollupField : WorkFlowActivityBase
    {
        /// <summary>
        /// The field name to be recalculated
        /// </summary>
        #region "Parameter Definition"
        [RequiredArgument]
        [Input("FieldName")]
        [Default("")]
        public InArgument<String> FieldName { get; set; }

        /// <summary>
        /// The Dynamics URL for the record to be updated
        /// </summary>
        [RequiredArgument]
        [Input("Parent Record URL")]
        [ReferenceTarget("")]
        public InArgument<String> ParentRecordUrl { get; set; }
        #endregion

        /// <summary>
        /// Main activity execution method, recalculates the rollup field
        /// </summary>
        /// <param name="executionContext">The activity execution context</param>
        /// <param name="crmWorkflowContext">Contains the organisation service and tracing service</param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {

            string fieldName = this.FieldName.Get(executionContext);
            string parentRecordUrl = this.ParentRecordUrl.Get(executionContext);
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
            CalculateRollupFieldRequest calculateRollup = new CalculateRollupFieldRequest
            {
                FieldName = fieldName,
                Target = new EntityReference(parentEntityName, new Guid(parentId))
            };
            crmWorkflowContext.OrganizationService.Execute(calculateRollup);

        }

        /// <summary>
        /// Query the Metadata to get the Entity Schema Name from the Object Type Code
        /// </summary>
        /// <param name="objectTypeCode"></param>
        /// <param name="service"></param>
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