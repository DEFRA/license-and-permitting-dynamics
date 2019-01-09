namespace Lp.DataAccess
{
    using System;
    using System.Linq;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.Metadata.Query;

    /// <summary>
    /// Data access class dealing with CRM MetaData transactions
    /// </summary>
    public static class DataAccessMetaData
    {

        /// <summary>
        /// Returns an entity reference for a given Dynamics Record Url
        /// </summary>
        /// <param name="service">CRM Service</param>
        /// <param name="parentRecordUrl">Dynamcis Record Url</param>
        /// <returns>Entity Reference</returns>
        public static EntityReference GetEntityReferenceFromRecordUrl(IOrganizationService service, string parentRecordUrl)
        {
            EntityReference entityReference;
            string[] urlParts = parentRecordUrl.Split("?".ToArray());
            string[] urlParams = urlParts[1].Split("&".ToCharArray());
            string parentObjectTypeCode = urlParams[0].Replace("etc=", "");
            string parentId = urlParams[1].Replace("id=", "");
            string parentEntityName = GetEntityNameFromCode(service, parentObjectTypeCode);
            entityReference = new EntityReference(parentEntityName, new Guid(parentId));
            return entityReference;
        }

        /// <summary>
        /// Query the Metadata to get the Entity Schema Name from the Object Type Code
        /// </summary>
        /// <param name="objectTypeCode">Entity type code</param>
        /// <param name="service">Organisation Service</param>
        /// <returns>Entity Schema Name</returns>
        public static string GetEntityNameFromCode(IOrganizationService service, string objectTypeCode)
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
            RetrieveMetadataChangesResponse response =
                (RetrieveMetadataChangesResponse) service.Execute(retrieveMetadataChangesRequest);

            EntityMetadata entityMetadata = response.EntityMetadata[0];
            return entityMetadata.SchemaName.ToLower();
        }
    }
}