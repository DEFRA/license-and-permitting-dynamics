namespace Lp.DataAccess
{
    using System;
    using Model.EarlyBound;
    using Microsoft.Xrm.Sdk;
    using Core.DataAccess.Base;

    /// <summary>
    /// Data access layer for Application Lines
    /// </summary>
    public class DataAcessApplicationDocument : DataAccessBase
    {
        /// <summary>
        /// Constructor just passes the services on
        /// </summary>
        /// <param name="organisationService"></param>
        /// <param name="tracingService"></param>
        public DataAcessApplicationDocument(IOrganizationService organisationService,
            ITracingService tracingService) : base(organisationService, tracingService)
        {
        }


        /// <summary>
        /// Creates an application document record
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="source"></param>
        /// <param name="applicationId"></param>
        /// <param name="documentName"></param>
        /// <param name="documentLink"></param>
        /// <param name="caseId"></param>
        /// <param name="emailId"></param>
        /// <param name="createdById"></param>
        /// <returns></returns>
        public Guid CreateApplicationDocument(
            string documentName, 
            string documentLink, 
            string fileName, 
            defra_ApplicationDocumentSource source, 
            Guid? applicationId, 
            Guid? caseId, 
            Guid? emailId,
            Guid? createdById)
        {
            TracingService.Trace($"CreateApplicationDocument() documentName={documentName}, documentLink={documentLink}, fileName={fileName}, source={source}, applicationId={applicationId}, caseId={caseId}, emailId={emailId}");
            // Prep the entity
            Entity appDocumentEntity = new Entity(defra_applicationdocument.EntityLogicalName);

            if (applicationId.HasValue)
            {
                appDocumentEntity.Attributes.Add(defra_applicationdocument.Fields.defra_applicationid, new EntityReference(defra_application.EntityLogicalName, applicationId.Value));
            }

            if (caseId.HasValue)
            {
                appDocumentEntity.Attributes.Add(defra_applicationdocument.Fields.defra_caseid, new EntityReference(Incident.EntityLogicalName, caseId.Value));
            }

            if (emailId.HasValue)
            {
                appDocumentEntity.Attributes.Add(defra_applicationdocument.Fields.defra_emailid, new EntityReference(Email.EntityLogicalName, emailId.Value));
            }

            if (createdById.HasValue)
            {
                appDocumentEntity.Attributes.Add(defra_applicationdocument.Fields.CreatedBy, new EntityReference(Incident.EntityLogicalName, createdById.Value));
                appDocumentEntity.Attributes.Add(defra_applicationdocument.Fields.CreatedOnBehalfBy, new EntityReference(Incident.EntityLogicalName, createdById.Value));
            }

            appDocumentEntity.Attributes.Add(defra_applicationdocument.Fields.defra_name, documentName);
            appDocumentEntity.Attributes.Add(defra_applicationdocument.Fields.defra_filename, fileName);
            appDocumentEntity.Attributes.Add(defra_applicationdocument.Fields.defra_url, documentLink);

            // Get CRM to Create record
            return OrganisationService.Create(appDocumentEntity);
        }
    }
}