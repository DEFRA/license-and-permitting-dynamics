namespace Core.DataAccess.Base
{
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// Base data access class, provides access to the CRM Org and Trace service
    /// </summary>
    public abstract class DataAccessBase
    {
        protected IOrganizationService OrganisationService { get; }
        protected ITracingService TracingService { get; }

        protected DataAccessBase(IOrganizationService organisationService, ITracingService tracingService)
        {
            OrganisationService = organisationService;
            TracingService = tracingService;
        }
    }
}
