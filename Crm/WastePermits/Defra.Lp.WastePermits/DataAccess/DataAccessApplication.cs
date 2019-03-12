using Core.Helpers.Extensions;
using WastePermits.Model.Internal;

namespace WastePermits.DataAccess
{
    using System;
    using Core.DataAccess.Base;
    using Lp.DataAccess.Interfaces;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Model.EarlyBound;

    /// <summary>
    /// Data access layer for Application related CRM queries
    /// </summary>
    public class DataAccessApplication : DataAccessBase, IDataAccessApplication
    {

        /// <summary>
        /// Constructor sets the CRM services
        /// </summary>
        /// <param name="organisationService"></param>
        /// <param name="tracingService"></param>
        public DataAccessApplication(IOrganizationService organisationService, ITracingService tracingService)
            : base(organisationService, tracingService)
        {
        }

        /// <summary>
        /// Retrieves the Application entity
        /// </summary>
        public Entity GetApplication(Guid applicationId)
        {

            //Retrieve the application if it exists
            TracingService.Trace("Application with Id {0} is being retrieved", applicationId);
            Entity applicatEntity = OrganisationService.Retrieve(
                defra_application.EntityLogicalName,
                applicationId,
                new ColumnSet(
                    defra_application.Fields.defra_dulymadechecklistid, 
                    defra_application.Fields.defra_applicationnumber, 
                    defra_application.Fields.defra_npsdetermination, 
                    defra_application.Fields.defra_locationscreeningrequired, 
                    defra_application.Fields.defra_applicationtype, 
                    defra_application.Fields.defra_application_subtype));

            TracingService.Trace("Application with Id {0} successfully retrieved", applicationId);

            return applicatEntity;
        }

        /// <summary>
        /// Retrieves the Application Type, Sub Type and Owner fields for an application
        /// </summary>
        public ApplicationTypesAndOwners GetApplicationTypeAndOwner(Guid applicationId)
        {
            TracingService.Trace("GetApplicationType() Application with Id {0} is being retrieved", applicationId);
            Entity applicatEntity = OrganisationService.Retrieve(
                defra_application.EntityLogicalName,
                applicationId,
                new ColumnSet(
                    defra_application.Fields.defra_applicationtype,
                    defra_application.Fields.defra_application_subtype,
                    defra_application.Fields.OwningUser,
                    defra_application.Fields.OwningTeam));

            TracingService.Trace("GetApplicationType() Application with Id {0} successfully retrieved", applicationId);

            return new ApplicationTypesAndOwners
            {
                ApplicationType = applicatEntity.GetOptionSetValue(defra_application.Fields.defra_applicationtype),
                ApplicationSubType = applicatEntity.GetAttributeId(defra_application.Fields.defra_application_subtype),
                OwningUser = applicatEntity.GetAttributeId(defra_application.Fields.OwningUser),
                OwningTeam = applicatEntity.GetAttributeId(defra_application.Fields.OwningTeam),
            };
        }
    }
}
