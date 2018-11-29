namespace WastePermits.DataAccess
{
    using System;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Model.EarlyBound;

    /// <summary>
    /// Class provides data access layer for Item/SR related facitlities
    /// </summary>
    public static class DataAccessFacilities
    {
        /// <summary>
        /// Returns a summarised text of all application lines
        /// </summary>
        /// <param name="service">CRM organisation service</param>
        /// <param name="applicationId">application id to query</param>
        /// <returns>Descriptive text for all application facilities</returns>
        public static string GetApplicationLinesSummary(IOrganizationService service, Guid applicationId)
        {
            // Set-up query
            var query = new QueryExpression(defra_applicationline.EntityLogicalName);
            query.Criteria.AddCondition(defra_applicationline.Fields.defra_applicationId, ConditionOperator.Equal, applicationId);

            // Filter by application
            FilterExpression notNullFilter = new FilterExpression {FilterOperator = LogicalOperator.Or};
            notNullFilter.AddCondition(defra_applicationline.Fields.defra_itemid, ConditionOperator.NotNull);
            notNullFilter.AddCondition(defra_applicationline.Fields.defra_standardruleId, ConditionOperator.NotNull);
            query.Criteria.AddFilter(notNullFilter);

            // Link to standard rule entity
            const string aliasStandarRule = "sr";
            LinkEntity srLink = query.AddLink(defra_standardrule.EntityLogicalName, defra_applicationline.Fields.defra_standardruleId, defra_standardrule.Fields.defra_standardruleId, JoinOperator.LeftOuter);
            srLink.EntityAlias = aliasStandarRule;
            srLink.Columns.AddColumns(defra_standardrule.Fields.defra_name, defra_standardrule.Fields.defra_rulesnamegovuk);

            // Link to item entity
            const string aliasItem = "item";
            LinkEntity itemLink = query.AddLink(defra_item.EntityLogicalName, defra_applicationline.Fields.defra_itemid, defra_item.Fields.defra_itemId, JoinOperator.LeftOuter);
            itemLink.EntityAlias = aliasItem;
            itemLink.Columns.AddColumns(defra_item.Fields.defra_code, defra_item.Fields.defra_officialname);

            // Call CRM
            EntityCollection result = service.RetrieveMultiple(query);

            // Check the results 
            if (result?.Entities == null)
            {
                // No Results
                return string.Empty;;
            }

            foreach (Entity line in result.Entities)
            {
                if (line.Contains($"{aliasStandarRule}.{defra_standardrule.Fields.defra_rulesnamegovuk}"))
                {
                    // TODO
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a summarised text of all permit lines
        /// </summary>
        /// <param name="service">CRM organisation service</param>
        /// <param name="permitId">permit id to query</param>
        /// <returns>Descriptive text for all permit facilities</returns>
        public static string GetPermitLinesSummary(IOrganizationService service, Guid permitId)
        {
            // Set-up query
            var query = new QueryExpression(defra_permit_lines.EntityLogicalName);
            query.Criteria.AddCondition(defra_permit_lines.Fields.defra_permitid, ConditionOperator.Equal, permitId);

            // Filter by application
            FilterExpression notNullFilter = new FilterExpression { FilterOperator = LogicalOperator.Or };
            notNullFilter.AddCondition(defra_permit_lines.Fields.defra_itemid, ConditionOperator.NotNull);
            notNullFilter.AddCondition(defra_permit_lines.Fields.defra_standardruleid, ConditionOperator.NotNull);
            query.Criteria.AddFilter(notNullFilter);

            // Link to standard rule entity
            const string aliasStandarRule = "sr";
            LinkEntity srLink = query.AddLink(defra_standardrule.EntityLogicalName, defra_permit_lines.Fields.defra_standardruleid, defra_standardrule.Fields.defra_standardruleId, JoinOperator.LeftOuter);
            srLink.EntityAlias = aliasStandarRule;
            srLink.Columns.AddColumns(defra_standardrule.Fields.defra_name, defra_standardrule.Fields.defra_rulesnamegovuk);

            // Link to item entity
            const string aliasItem = "item";
            LinkEntity itemLink = query.AddLink(defra_item.EntityLogicalName, defra_permit_lines.Fields.defra_itemid, defra_item.Fields.defra_itemId, JoinOperator.LeftOuter);
            itemLink.EntityAlias = aliasItem;
            itemLink.Columns.AddColumns(defra_item.Fields.defra_code, defra_item.Fields.defra_officialname);

            // TODO
            return null;
        }
    }
}