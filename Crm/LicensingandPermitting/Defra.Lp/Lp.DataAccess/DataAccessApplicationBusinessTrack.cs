namespace Lp.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk;
    using Core.Helpers.Extensions;
    using Model.EarlyBound;
    using Core.DataAccess.Base;
    using Model.Internal;

    /// <summary>
    /// Data access layer for Application Answer related CRM queries
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
        /// Returns a defra_application_answer record for the given application, application line and question code
        /// </summary>
        /// <param name="applicationQuestionCode">The question code to look up the answer by</param>
        /// <param name="application">The application to lookup the answer by</param>
        /// <param name="applicationLine">The application line to lookup the answer by</param>
        /// <returns></returns>
        public EntityReference GetApplicationBusinessTrackEntityReference(Guid applicationId)
        {          
            return null;
        }
    }
}