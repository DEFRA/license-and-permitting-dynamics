using Lp.Model.EarlyBound;

namespace WastePermits.DataAccess
{
    using System;
    using Core.DataAccess.Base;
    using Lp.DataAccess.Interfaces;
    using Lp.Model.Crm;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    public class DataAccessApplication : DataAccessBase, IDataAccessApplication
    {

        public DataAccessApplication(IOrganizationService organisationService, ITracingService tracingService)
            : base(organisationService, tracingService)
        {
        }

        /// <summary>
        /// Retrieves the Application entity
        /// </summary>
        public Entity GetApplication(Guid applicationId)
        {

            //Retrieve the duly made record if exists
            TracingService.Trace("Application with Id {0} is being retrieved", applicationId);
            Entity applicatEntity = OrganisationService.Retrieve(
                Application.EntityLogicalName,
                applicationId,
                new ColumnSet("defra_dulymadechecklistid", "defra_applicationnumber", "defra_npsdetermination", "defra_locationscreeningrequired", Application.ApplicationType, Application.ApplicationSubType));

            //Initiate the updated application entity
            TracingService.Trace("Application with Id {0} successfully retrieved", applicationId);

            return applicatEntity;
        }
    }
}
