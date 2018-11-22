using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace Core.DataAccess.Base
{
    public class DataAccessBase
    {
        protected IOrganizationService OrganisationService { get; }
        protected ITracingService TracingService { get; }

        public DataAccessBase(IOrganizationService organisationService, ITracingService tracingService)
        {
            OrganisationService = organisationService;
            TracingService = tracingService;
        }
    }
}
