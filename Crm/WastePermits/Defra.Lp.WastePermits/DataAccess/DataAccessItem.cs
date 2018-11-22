using System;
using Core.DataAccess.Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using WastePermits.DataAccess.Interfaces;

namespace WastePermits.DataAccess
{
    public class DataAccessItem : DataAccessBase, IDataAccessItem
    {

        public DataAccessItem(IOrganizationService organisationService, ITracingService tracingService)
            : base(organisationService, tracingService)
        {
        }

        public EntityCollection GetAssessmentsForActivity( Guid activity)
        {
            // Instantiate QueryExpression QEdefra_itemdetail
            var qeIemDetail = new QueryExpression("defra_itemdetail");

            // Add columns to QEdefra_itemdetail.ColumnSet
            qeIemDetail.ColumnSet.AddColumns("defra_itemid", "defra_parentitemid");

            // Define filter QEdefra_itemdetail.Criteria
            qeIemDetail.Criteria.AddCondition("defra_parentitemid", ConditionOperator.Equal, activity.ToString());

            // Add link-entity QEdefra_itemdetail_defra_itemdetailtype
            var qeItemDetailItemDetailType = qeIemDetail.AddLink("defra_itemdetailtype", "defra_itemdetailtypeid", "defra_itemdetailtypeid");

            // Add columns to QEdefra_itemdetail_defra_itemdetailtype.Columns
            qeItemDetailItemDetailType.Columns.AddColumns("defra_name");

            // Query CRM
            return OrganisationService.RetrieveMultiple(qeIemDetail);
        }
    }
}
