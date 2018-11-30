namespace WastePermits.DataAccess
{
    using System;
    using Core.DataAccess.Base;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Lp.DataAccess.Interfaces;
    using Model.EarlyBound;

    /// <summary>
    /// Data access class provides methods to query Item records
    /// </summary>
    public class DataAccessItem : DataAccessBase, IDataAccessItem
    {

        /// <summary>
        /// Constructor for DAL class
        /// </summary>
        /// <param name="organisationService"></param>
        /// <param name="tracingService"></param>
        public DataAccessItem(IOrganizationService organisationService, ITracingService tracingService)
            : base(organisationService, tracingService) {}

        /// <summary>
        ///  Returns a list of assessment item records that are linked to a given activity
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public EntityCollection GetAssessmentsForActivity( Guid activity)
        {
            // Instantiate QueryExpression QEdefra_itemdetail
            var qeIemDetail = new QueryExpression(defra_itemdetail.EntityLogicalName);

            // Add columns to QEdefra_itemdetail.ColumnSet
            qeIemDetail.ColumnSet.AddColumns(defra_itemdetail.Fields.defra_itemid, defra_itemdetail.Fields.defra_parentitemid);

            // Define filter QEdefra_itemdetail.Criteria
            qeIemDetail.Criteria.AddCondition(defra_itemdetail.Fields.defra_parentitemid, ConditionOperator.Equal, activity.ToString());

            // Add link-entity QEdefra_itemdetail_defra_itemdetailtype
            var qeItemDetailItemDetailType = qeIemDetail.AddLink(defra_itemdetailtype.EntityLogicalName, defra_itemdetail.Fields.defra_itemdetailtypeid, defra_itemdetailtype.Fields.defra_itemdetailtypeId);

            // Add columns to QEdefra_itemdetail_defra_itemdetailtype.Columns
            qeItemDetailItemDetailType.Columns.AddColumns(defra_itemdetailtype.Fields.defra_name);

            // Query CRM
            return OrganisationService.RetrieveMultiple(qeIemDetail);
        }
    }
}
