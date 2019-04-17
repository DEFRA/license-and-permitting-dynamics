namespace Lp.DataAccess
{
    using System;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk;
    using Model.EarlyBound;
    using Core.DataAccess.Base;

    /// <summary>
    /// Data access layer for Business Track Related CRM queries
    /// </summary>
    public class DataAccessApplicationBusinessTrack : DataAccessBase
    {  

        /// <summary>
        /// Constructor sets the CRM services
        /// </summary>
        /// <param name="organisationService">CRM organisation service</param>
        /// <param name="tracingService">CRM trace service</param>
        public DataAccessApplicationBusinessTrack(IOrganizationService organisationService,
            ITracingService tracingService) : base(organisationService, tracingService)
        {
        }

        /// <summary>
        /// Returns the business track the application is linked to
        /// </summary>
        /// <param name="applicationId">The application being sought</param>
        /// <returns>The Business Track for the application, if found</returns>
        public EntityReference GetApplicationBusinessTrackEntityReference(Guid applicationId)
        {
            Entity application = OrganisationService.Retrieve(defra_application.EntityLogicalName, applicationId, new ColumnSet(defra_application.Fields.defra_businesstrackid));

            if (application == null || !application.Contains(defra_application.Fields.defra_businesstrackid))
            {
                // No Business Track to return
                return null;
            }

            // Return the business track lookup
            return application[defra_application.Fields.defra_businesstrackid] as EntityReference;
        }
    }
}