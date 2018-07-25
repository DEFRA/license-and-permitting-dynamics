
namespace Core.Logs
{
    using System;
    using Microsoft.Xrm.Sdk;

    public class LogManager
    {
        private IOrganizationService CrmOrganisationService { get; set; }
        public LogManager(IOrganizationService crmOrganizationService)
        {
            this.CrmOrganisationService = crmOrganizationService;
        }

        public void LogException(Exception exception, string relatedEntityName, string relatedEntityId)
        {
            
        }
    }
}
