using Core.Configuration;
using Core.Helpers.Extensions;
using Lp.DataAccess;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Model.Lp.Crm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Defra.Lp.Common.SharePoint
{
    internal class AzureInterface
    {
        private IDictionary<string, string> Config { get; set; }
        private IOrganizationService Service { get; set; }
        private IOrganizationService AdminService { get; set; }
        private ITracingService TracingService { get; set; }

        internal AzureInterface(IOrganizationService adminService, IOrganizationService service, ITracingService tracingService)
        {
            AdminService = adminService;
            Service = service;
            TracingService = tracingService;

            // Read the settings
            Config = adminService.GetConfigurationStringValues(
                    $"{SharePointSecureConfigurationKeys.ApplicationFolderContentType}",
                    $"{SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl}",
                    $"{SharePointSecureConfigurationKeys.MetadataLogicAppUrl}",
                    $"{SharePointSecureConfigurationKeys.PermitFolderContentType}",
                    $"{SharePointSecureConfigurationKeys.PermitListName}");

            // Validate configuration records exist
            if (Config == null
                || !Config.ContainsKey(SharePointSecureConfigurationKeys.ApplicationFolderContentType)
                || !Config.ContainsKey(SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl)
                || !Config.ContainsKey(SharePointSecureConfigurationKeys.MetadataLogicAppUrl)
                || !Config.ContainsKey(SharePointSecureConfigurationKeys.PermitFolderContentType)
                || !Config.ContainsKey(SharePointSecureConfigurationKeys.PermitListName))

            {
                throw new InvalidPluginExecutionException("The Sharepoint integration needs to be configured.");
            }

            TracingService.Trace("Configuration read successfully.");
        }

        internal void CreateFolder(EntityReference application)
        {
            TracingService.Trace(string.Format("In CreateFolder with Entity Type {0} and Entity Id {1}", application.LogicalName, application.Id));

            var request = new DocumentRelayRequest();
            var applicationEntity = Query.RetrieveDataForEntityRef(Service, new[] { Application.Name, Application.PermitNumber, Application.ApplicationNumber }, application);

            TracingService.Trace(string.Format("Permit Number = {0}; Application Number = {1}", applicationEntity[Application.PermitNumber].ToString(), applicationEntity[Application.ApplicationNumber].ToString()));

            request.ApplicationContentType = Config[$"{SharePointSecureConfigurationKeys.ApplicationFolderContentType}"];
            request.ApplicationNo = applicationEntity.GetAttributeValue<string>(Application.ApplicationNumber).Replace('/', '_');
            request.FileBody = string.Empty;
            request.FileDescription = string.Empty;
            request.FileName = string.Empty;
            request.ListName = Config[$"{SharePointSecureConfigurationKeys.PermitListName}"];
            request.PermitContentType = Config[$"{SharePointSecureConfigurationKeys.PermitFolderContentType}"];
            request.PermitNo = applicationEntity.GetAttributeValue<string>(Application.PermitNumber);
            request.Customer = string.Empty;
            request.SiteDetails = string.Empty;
            request.PermitDetails = string.Empty;

            var stringContent = JsonConvert.SerializeObject(request);

            TracingService.Trace(string.Format("Data Sent to Logic App URL {0}", Config[$"{SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl}"]));

            SendRequest(Config[$"{SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl}"], stringContent);
        }

        internal void UploadFile(EntityReference recordIdentifier)
        {
            TracingService.Trace("In UploadFile with Entity Type {0} and Entity Id {1}", recordIdentifier.LogicalName, recordIdentifier.Id);

            switch (recordIdentifier.LogicalName)
            {
                case Email.EntityLogicalName:
                    UploadEmail(recordIdentifier.Id);
                    break;
                case ActivityMimeAttachment.EntityLogicalName:
                    UploadAttachment(recordIdentifier.Id);
                    break; 
                default:
                    // Default is annotation for Application or Case
                    UploadAnnotation(recordIdentifier.Id);
                    break;
            }
        }

        private void UploadAttachment(Guid recordId)
        {
            var request = new DocumentRelayRequest();
            // Email attachment has been created. Query CRM to get the email attachment data
            var attachmentData = ReturnAttachmentData(recordId);
            if (attachmentData == null)
            {
                throw new InvalidPluginExecutionException("No attachment data record returned from query");
            }

            var regardingObjectId = GetRegardingObjectId(attachmentData);
            if (regardingObjectId != null && (regardingObjectId.LogicalName == Application.EntityLogicalName || regardingObjectId.LogicalName == Case.EntityLogicalName))
            {
                var direction = (bool)(attachmentData.GetAttributeValue<AliasedValue>("email.directioncode")).Value;
                var statusCode = (OptionSetValue)(attachmentData.GetAttributeValue<AliasedValue>("email.statuscode")).Value;
                if (direction && statusCode.Value != 3)
                {
                    //Outgoing email, do not send the attachment on create
                    TracingService.Trace("Aborted creating attachment for outgoing email attachment that is not sent");
                    return;
                }

                AddInsertFileParametersToRequest(request, attachmentData);

                var resultBody = SendRequest(Config[$"{SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl}"], JsonConvert.SerializeObject(request));
                if (resultBody != null)
                {
                    TracingService.Trace("Returned from LogicApp OK");
                    if (attachmentData != null)
                    {
                        // Delete attachment from CRM
                        attachmentData[ActivityMimeAttachment.Body] = string.Empty;
                        Service.Update(attachmentData);
                    }
                }
            }
            else
            {
                TracingService.Trace("Only attachments for emails regarding Applications, RFIs or Schedule 5s are currently sent to SharePoint");
            }
        }

        private void UploadEmail(Guid recordId)
        {
            var request = new DocumentRelayRequest();

            // Processing an email record. Need the information so we can upload the email to SharePoint
            var emailData = ReturnEmailData(recordId);
            if (emailData == null)
            {
                throw new InvalidPluginExecutionException("No email data record returned from query");
            }

            var regardingObjectId = GetRegardingObjectId(emailData);
            if (regardingObjectId != null && (regardingObjectId.LogicalName == Application.EntityLogicalName || regardingObjectId.LogicalName == Case.EntityLogicalName))
            {
                var direction = emailData.GetAttributeValue<bool>(Email.DirectionCode);
                var statusCode = emailData.GetAttributeValue<OptionSetValue>(Email.StatusCode);
                if (direction && statusCode.Value != 3)
                {
                    //Outgoing email, do not send the email on create
                    TracingService.Trace("Aborted creating email for outgoing email that is not sent");
                    return;
                }

                AddInsertFileParametersToRequest(request, emailData);

                var resultBody = SendRequest(Config[$"{SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl}"], JsonConvert.SerializeObject(request));
                if (resultBody != null)
                {
                    TracingService.Trace("Returned from LogicApp OK");
                    if (emailData != null)
                    {
                        // Set uploaded to SharePoint flag!
                        emailData[Email.UploadedToSharePoint] = true;
                        Service.Update(emailData);
                    }
                }
            }
            else
            {
                TracingService.Trace("Only emails regarding Applications, RFIs or Schedule 5s are sent to SharePoint");
            }
        }

        private void UploadAnnotation(Guid recordId)
        {
            var request = new DocumentRelayRequest();

            // Creation of of an Annotation record on a Application or Case. 
            var annotationData = ReturnAnnotationData(recordId);
            if (annotationData == null)
            {
                throw new InvalidPluginExecutionException("No annotation data record returned from query");
            }

            AddInsertFileParametersToRequest(request, annotationData);

            var resultBody = SendRequest(Config[$"{SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl}"], JsonConvert.SerializeObject(request));
            if (resultBody != null)
            {
                TracingService.Trace("Returned from LogicApp OK");
                if (annotationData != null)
                {
                    // Delete annotation from CRM and a note to say thats what we've done!
                    annotationData[Annotation.NoteText] = "File has been uploaded to SharePoint.";
                    annotationData[Annotation.DocumentBody] = string.Empty;
                    Service.Update(annotationData);
                }
            }
        }

        internal void UpdateMetaData(EntityReference entity, string customer, string siteDetails, string permitDetails)
        {
            TracingService.Trace("In UpdateMetaData with Entity Type {0} and Entity Id {1}", entity.LogicalName, entity.Id.ToString());

            var request = new MetaDataRequest();

            if (entity.LogicalName == Application.EntityLogicalName)
            {
                var applicationEntity = Query.RetrieveDataForEntityRef(Service, new[] { Application.Name, Application.PermitNumber, Application.ApplicationNumber }, entity);
                if (applicationEntity != null)
                {
                    TracingService.Trace($"Permit Number = {applicationEntity[Application.PermitNumber]}; Application Number = {applicationEntity[Application.ApplicationNumber]}");

                    request.ApplicationNo = applicationEntity.GetAttributeValue<string>(Application.ApplicationNumber).Replace('/', '_');
                    request.ListName = Config[$"{SharePointSecureConfigurationKeys.PermitListName}"];
                    request.PermitNo = applicationEntity.GetAttributeValue<string>(Application.PermitNumber);
                    request.Customer = customer;
                    request.SiteDetails = siteDetails;
                    request.PermitDetails = permitDetails;
                    request.UpdateType = AzureInterfaceConstants.MetaDataApplicationUpdateType;
                }
                else
                {
                    throw new InvalidPluginExecutionException(string.Format("No Application exists for entity reference {0}", entity.Id.ToString()));
                }
            }
            else if (entity.LogicalName == Permit.EntityLogicalName)
            {
                var permitEntity = Query.RetrieveDataForEntityRef(Service, new[] { Permit.Name, Permit.PermitNumber }, entity);
                if (permitEntity != null)
                {
                    TracingService.Trace(string.Format("Permit Number = {0}", permitEntity[Permit.PermitNumber]));

                    request.ApplicationNo = string.Empty;
                    request.ListName = Config[$"{SharePointSecureConfigurationKeys.PermitListName}"];
                    request.PermitNo = permitEntity.GetAttributeValue<string>(Permit.PermitNumber);
                    request.Customer = customer;
                    request.SiteDetails = siteDetails;
                    request.PermitDetails = permitDetails;
                    request.UpdateType = AzureInterfaceConstants.MetaDataPermitUpdateType;
                }
                else
                {
                    throw new InvalidPluginExecutionException(string.Format("No Permit exists for entity reference {0}", entity.Id.ToString()));
                }
            }

            var stringContent = JsonConvert.SerializeObject(request);

            TracingService.Trace($"Data Sent to Logic App URL {Config[$"{SharePointSecureConfigurationKeys.MetadataLogicAppUrl}"]}");

            SendRequest(Config[$"{SharePointSecureConfigurationKeys.MetadataLogicAppUrl}"], stringContent);
        }

        private void AddEmailParametersToRequest(DocumentRelayRequest request, Entity queryRecord)
        {
            TracingService.Trace("Adding Email Parameters to Request");

            request.EmailTo = string.Empty;
            request.EmailFrom = string.Empty;

            // Set Email stuff when we have an email
            if (queryRecord.LogicalName == Email.EntityLogicalName || queryRecord.LogicalName == ActivityMimeAttachment.EntityLogicalName)
            {
                if (queryRecord.Contains(Email.Sender))
                {
                    request.EmailFrom = queryRecord.GetAttributeValue<string>(Email.Sender);
                }

                if (queryRecord.Contains(Email.ToRecipients))
                {
                    request.EmailTo = queryRecord.GetAttributeValue<string>(Email.ToRecipients);
                }

                if (queryRecord.Contains("email.sender"))
                {
                    request.EmailFrom = ((string)((AliasedValue)queryRecord.Attributes["email.sender"]).Value);
                }

                if (queryRecord.Contains("email.torecipients"))
                {
                    request.EmailTo = ((string)((AliasedValue)queryRecord.Attributes["email.torecipients"]).Value);
                } 
                if (queryRecord.Contains(Email.Subject))
                {
                    // For an email, this is just the filename again
                    request.EmailLink = GetFileName(queryRecord);
                }
                else if (queryRecord.Contains("email.subject"))
                {
                    // Annotation needs the filename created for the email from subject and created on date
                    request.EmailLink = CreateEmailFileNameForAttachment(queryRecord);
                }

            }
            // Max size of 250 in SharePoint
            request.EmailFrom = request.EmailFrom.TruncateIfNeeded(250);
            request.EmailTo = request.EmailTo.TruncateIfNeeded(250);
        }

        private void AddInsertFileParametersToRequest(DocumentRelayRequest request, Entity queryRecord)
        {
            TracingService.Trace("In AddInsertFileParametersToRequest()");
            TracingService.Trace("Logical Name: {0}", queryRecord.LogicalName);

            var permitNo = GetPermitNumber(queryRecord);
            var applicationNo = GetApplicationNumber(queryRecord);
            var fileName = GetFileName(queryRecord);
            var body = GetBody(queryRecord);
            var subject = GetSubject(queryRecord);
            var crmId = GetCrmId(queryRecord);
            var caseNo = GetCaseFolderName(queryRecord);
            var regarding = GetRegarding(queryRecord);

            request.ApplicationContentType = Config[$"{SharePointSecureConfigurationKeys.ApplicationFolderContentType}"];
            request.ApplicationNo = applicationNo;
            request.FileBody = body;
            request.FileDescription = subject;
            request.FileName = fileName;
            request.ListName = Config[$"{SharePointSecureConfigurationKeys.PermitListName}"];
            request.PermitContentType = Config[$"{SharePointSecureConfigurationKeys.PermitFolderContentType}"];
            request.PermitNo = permitNo;
            request.Customer = string.Empty;      // Not currently set by Document relay, enhancement
            request.SiteDetails = string.Empty;   // Not currently set by Document relay, enhancement
            request.PermitDetails = string.Empty; // Not currently set by Document relay, enhancement
            request.CrmId = crmId;
            request.CaseNo = caseNo;
            request.EmailRegarding = regarding;

            AddEmailParametersToRequest(request, queryRecord);

            TracingService.Trace(string.Format("Requests: {0}", request));
        }

        private EntityReference GetRegardingObjectId(Entity queryRecord)
        {
            EntityReference regardingObjectRef = null;
            // Email
            if (queryRecord.Contains(Email.RegardingObjectId))
            {
                regardingObjectRef = queryRecord.GetAttributeValue<EntityReference>(Email.RegardingObjectId);
            }
            // Attachment
            if (queryRecord.Contains("email.regardingobjectid"))
            {
                regardingObjectRef = (EntityReference)(queryRecord.GetAttributeValue<AliasedValue>("email.regardingobjectid")).Value;
            }
            return regardingObjectRef;
        }

        private string GetRegarding(Entity queryRecord)
        {
            string regarding;
            // Set the email regarding field to Schedule 5 or RFI otherwise assume its an application
            if (queryRecord.Contains("case.casetypecode"))
            {
                var caseTypeCode = (OptionSetValue)(queryRecord.GetAttributeValue<AliasedValue>("case.casetypecode")).Value;
                regarding = Query.GetCRMOptionsetText(AdminService, Case.EntityLogicalName, Case.CaseType, caseTypeCode.Value);
            }
            else
            {
                regarding = "Application";
            }
            TracingService.Trace("Regarding: {0}", regarding);
            return regarding;
        }

        private string GetCaseFolderName(Entity queryRecord)
        {
            var caseFolderName = string.Empty;
            if (queryRecord.Contains("case.casetypecode") && queryRecord.Contains("case.title") && queryRecord.Contains("case.ticketnumber"))
            {
                var caseType = string.Empty;
                var caseNo = string.Empty;
                var title = string.Empty;
                var caseTypeCode = (OptionSetValue)(queryRecord.GetAttributeValue<AliasedValue>("case.casetypecode")).Value;
                caseType = Query.GetCRMOptionsetText(AdminService, Case.EntityLogicalName, Case.CaseType, caseTypeCode.Value);

                title = (string)((AliasedValue)queryRecord.Attributes["case.title"]).Value;
                title = SpRemoveIllegalChars(title, false); // Used for folder name. Spaces ok.
           
                caseNo = (string)((AliasedValue)queryRecord.Attributes["case.ticketnumber"]).Value;
            
                caseFolderName = string.Format("{0} - {1} ({2})", caseType, title, caseNo);
                TracingService.Trace("Case Folder Name: {0}", caseFolderName);
            }
            return caseFolderName;
        }

        private string GetCrmId(Entity queryRecord)
        {
            var crmId = string.Empty;
            // Annotation
            if (queryRecord.Contains(Annotation.Id))
            {
                crmId = queryRecord.GetAttributeValue<Guid>(Annotation.Id).ToString();
            }
            // Email
            if (queryRecord.Contains(Email.ActivityId))
            {
                crmId = queryRecord.GetAttributeValue<Guid>(Email.ActivityId).ToString();
            }
            // Attachment
            if (queryRecord.Contains("email.activityid"))
            {
                crmId = ((Guid)((AliasedValue)queryRecord.Attributes["email.activityid"]).Value).ToString();
            }
            TracingService.Trace("Crm Id: {0}", crmId);
            return crmId;
        }

        private string GetDescription(Entity queryRecord)
        {
            var desc = string.Empty;
            // Emails
            if (queryRecord.Contains(Email.Subject))
            {
                desc = queryRecord.GetAttributeValue<string>(Email.Subject);
            }
            // Attachments
            if (queryRecord.Contains("email.subject"))
            {
                desc = (string)((AliasedValue)queryRecord.Attributes["email.subject"]).Value;
            }
            TracingService.Trace("Description: {0}", desc);
            return desc;
        }

        private string GetPermitNumber(Entity queryRecord)
        {
            var permitNo = string.Empty;
            //if (queryRecord.Contains("parent.defra_permitnumber"))
            //{
            //    permitNo = (string)((AliasedValue)queryRecord.Attributes["parent.defra_permitnumber"]).Value;
            //}
            if (queryRecord.Contains("application.defra_permitnumber"))
            {
                permitNo = (string)((AliasedValue)queryRecord.Attributes["application.defra_permitnumber"]).Value;
            }
            if (queryRecord.Contains("case.application.defra_permitnumber"))
            {
                permitNo = (string)((AliasedValue)queryRecord.Attributes["case.application.defra_permitnumber"]).Value;
            }
            TracingService.Trace("Permit No: {0}", permitNo);
            return permitNo;
        }

        private string GetApplicationNumber(Entity queryRecord)
        {
            var applicationNo = string.Empty;
            //if (queryRecord.Contains("parent.defra_applicationnumber"))
            //{
            //    applicationNo = (string)((AliasedValue)queryRecord.Attributes["parent.defra_applicationnumber"]).Value;
            //    applicationNo = applicationNo.Replace('/', '_');
            //}
            if (queryRecord.Contains("application.defra_applicationnumber"))
            {
                applicationNo = (string)((AliasedValue)queryRecord.Attributes["application.defra_applicationnumber"]).Value;
                applicationNo = applicationNo.Replace('/', '_');
            }
            if (queryRecord.Contains("case.application.defra_applicationnumber"))
            {
                applicationNo = (string)((AliasedValue)queryRecord.Attributes["case.application.defra_applicationnumber"]).Value;
                applicationNo = applicationNo.Replace('/', '_');
            }
            TracingService.Trace("Application No: {0}", applicationNo);
            return applicationNo;
        }

        private string GetSubject(Entity queryRecord)
        {
            var subject = string.Empty;
            // email
            if (queryRecord.Contains(Email.Subject))
            {
                subject = queryRecord.GetAttributeValue<string>(Email.Subject);
            }
            // attachment
            if (queryRecord.Contains("email.subject"))
            {
                subject = (string)((AliasedValue)queryRecord.Attributes["email.subject"]).Value;
            }
            TracingService.Trace("Subject: {0}", subject);
            return subject;
        }

        private string CreateEmailFileName(Entity queryRecord)
        {
            // For an email, we're going to use the subject as the filename.
            var fileName = queryRecord.GetAttributeValue<string>(Email.Subject);
            var createdDate = queryRecord.GetAttributeValue<DateTime>(Email.CreatedOn);
            // Filename needs to have a timestamp so that CRM doesn't overwrite if the
            // user uploads something with the same name from front end. Also need to remove
            // any illegal charcter that SharePoint might complain about
            fileName = SpRemoveIllegalChars(fileName).AppendTimeStamp(createdDate);

            // Give it an HTML ending as we want to view it in SharePoint as HTML
            fileName = fileName + ".html";

            return fileName;
        }

        private string CreateEmailFileNameForAttachment(Entity queryRecord)
        {
            // For an email, we're going to use the subject as the filename.
            var fileName = (string)((AliasedValue)queryRecord.Attributes["email.subject"]).Value;
            var createdDate = (DateTime)((AliasedValue)queryRecord.Attributes["email.createdon"]).Value;
            // Filename needs to have a timestamp so that CRM doesn't overwrite if the
            // user uploads something with the same name from front end. Also need to remove
            // any illegal charcter that SharePoint might complain about
            fileName = SpRemoveIllegalChars(fileName).AppendTimeStamp(createdDate);

            // Give it an HTML ending as we want to view it in SharePoint as HTML
            fileName = fileName + ".html";

            return fileName;
        }

        private string GetFileName(Entity queryRecord)
        {
            // Filename will be in subject for emails for filename field
            // for annotations and attachments
            var fileName = string.Empty;
            var createdDate = DateTime.Now;
            // Use created date for timestamp to avoid querying sharepoint in logic app
            if (queryRecord.Contains(Email.CreatedOn))
            {
                // annotation and email
                createdDate = queryRecord.GetAttributeValue<DateTime>(Email.CreatedOn);
            }
            if (queryRecord.Contains("email.createdon"))
            {
                // attachments
                createdDate = (DateTime)((AliasedValue)queryRecord.Attributes["email.createdon"]).Value;
            }
            if (queryRecord.Contains("filename"))
            {
                // attachments and annotations have a filename
                fileName = queryRecord.GetAttributeValue<string>("filename");

                // Filename needs to have a timestamp so that CRM doesn't overwrite if the
                // user uploads something with the same name from front end. Also need to remove
                // any illegal charcter that SharePoint might complain about
                fileName = SpRemoveIllegalChars(fileName).AppendTimeStamp(createdDate);
            }
            else if (queryRecord.Contains(Email.Subject))
            {
                // For an email, we're going to use the subject as the filename.
                fileName = CreateEmailFileName(queryRecord);
            }
            TracingService.Trace("Filename: {0}", fileName);
            return fileName;
        }

        private string GetBody(Entity queryRecord)
        {
            string body;
            if (queryRecord.LogicalName == ActivityMimeAttachment.EntityLogicalName)
            {
                // For an attachment the file is in body          
                body = queryRecord.GetAttributeValue<string>("body");
            }
            else if (queryRecord.LogicalName == Email.EntityLogicalName)
            {
                // For an email the body is in description. It needs to be wrapped in 
                // a HTML Header and Body tags as we want to view it like HTML in SharePoint.
                // We're also adding in the To, From and Subject
                var desc = queryRecord.GetAttributeValue<string>(Email.Description);
                desc = string.IsNullOrEmpty(desc) ? string.Empty : desc;
                var sender = queryRecord.GetAttributeValue<string>(Email.Sender);
                sender = string.IsNullOrEmpty(sender) ? string.Empty : sender;
                var to = queryRecord.GetAttributeValue<string>(Email.ToRecipients);
                to = string.IsNullOrEmpty(to) ? string.Empty : to;
                var subject = queryRecord.GetAttributeValue<string>(Email.Subject);
                subject = string.IsNullOrEmpty(subject) ? string.Empty : subject;
                body = string.Format("<html><head></head><body><div><p><b>From:</b>&nbsp;{1}</p><p><b>To:</b>&nbsp;{2}</p><p><b>Subject:</b>&nbsp;{3}</p></div>{0}</body></html>",
                                      desc, sender, to, subject);

                // It needs to be Base64 encoded so it matches body and documentbody
                body = body.Base64Encode();
            }
            else
            {
                // For an annotation the file is in document body
                body = queryRecord.GetAttributeValue<string>("documentbody");
            }
            TracingService.Trace("Got body");
            return body;
        }

        private Entity ReturnAnnotationData(Guid recordId)
        {
            // Instantiate QueryExpression QEannotation
            var queryAnnotation = new QueryExpression(Annotation.EntityLogicalName);
            queryAnnotation.TopCount = 1;

            // Add columns to annotation entity
            queryAnnotation.ColumnSet.AddColumns(Annotation.Subject, Annotation.DocumentBody, Annotation.Filename, Annotation.Id, Annotation.NoteText, Annotation.FileSize, Annotation.IsDocument, Annotation.CreatedOn);

            // Define filter on Primary key
            queryAnnotation.Criteria.AddCondition(Annotation.Id, ConditionOperator.Equal, recordId);

            // Add link-entity to defra_application. Outer join as it might be regarding a case or application
            var queryExpressionAnnotationApp = queryAnnotation.AddLink(Application.EntityLogicalName, Annotation.RegardingObjectId, Application.ApplicationId, JoinOperator.LeftOuter);
            queryExpressionAnnotationApp.EntityAlias = "application";

            // Add columns to Application entity
            queryExpressionAnnotationApp.Columns.AddColumns(Application.ApplicationId, Application.Name, Application.PermitNumber, Application.ApplicationNumber, Application.StatusCode);

            // Add link-entity to Case. Outer join as it might be regarding case or application
            var queryExpressionAnnotationIncident = queryAnnotation.AddLink(Case.EntityLogicalName, Annotation.RegardingObjectId, Case.IncidentId, JoinOperator.LeftOuter);
            queryExpressionAnnotationIncident.EntityAlias = "case";

            // Add columns to Case entity
            queryExpressionAnnotationIncident.Columns.AddColumns(Case.Title, Case.IncidentId, Case.CaseType, Case.TicketNumber);

            // Add link-entity from case to application
            var queryExpressionAnnotationIncidentApplication = queryExpressionAnnotationIncident.AddLink(Application.EntityLogicalName, Case.Application, Application.ApplicationId, JoinOperator.LeftOuter);
            queryExpressionAnnotationIncidentApplication.EntityAlias = "case.application";

            // Add columns to QEannotation_incident_defra_application.Columns
            queryExpressionAnnotationIncidentApplication.Columns.AddColumns(Application.ApplicationNumber, Application.Name, Application.PermitNumber);

            var results = Service.RetrieveMultiple(queryAnnotation);
            if (results != null && results.Entities.Count >= 1)
            {
                return results.Entities[0];
            }

            return null;
        }

        private Entity ReturnAttachmentData(Guid recordId)
        {
            // Instantiate QueryExpression QEactivitymimeattachment
            var queryActivityMimeAttachment = new QueryExpression(ActivityMimeAttachment.EntityLogicalName);
            queryActivityMimeAttachment.TopCount = 1;

            // Add columns to ActivityMimeAttachment Entity
            queryActivityMimeAttachment.ColumnSet.AddColumns(ActivityMimeAttachment.Filename, ActivityMimeAttachment.Body);

            // Define filter on Primary Key
            queryActivityMimeAttachment.Criteria.AddCondition(ActivityMimeAttachment.Id, ConditionOperator.Equal, recordId);

            // Add link-entity QEactivitymimeattachment_email. Inner join as it must be regarding an Email
            var queryActivityMimeAttachmentEmail = queryActivityMimeAttachment.AddLink(Email.EntityLogicalName, 
                                                                                  ActivityMimeAttachment.ObjectId,                                                             Email.ActivityId);
            queryActivityMimeAttachmentEmail.EntityAlias = "email";

            // Add columns to QEactivitymimeattachment_email.Columns
            queryActivityMimeAttachmentEmail.Columns.AddColumns(Email.Description, Email.Subject, Email.DirectionCode, Email.ActivityId, Email.StatusCode, Email.RegardingObjectId, Email.Sender, Email.ToRecipients, Email.CreatedOn);

            // Add Application link-entity and define an alias.
            // Its an outer join as we want to return results even if not regarding an application
            var queryActivityMimeAttachmentEmailApplication = queryActivityMimeAttachmentEmail.AddLink(
                Application.EntityLogicalName, 
                Email.RegardingObjectId,                                                                                  
                Application.ApplicationId,
                JoinOperator.LeftOuter);

            queryActivityMimeAttachmentEmailApplication.EntityAlias = "application";

            // Add columns to Application Entity
            queryActivityMimeAttachmentEmailApplication.Columns.AddColumns(Application.Name, Application.PermitNumber, Application.ApplicationNumber);

            // Add Application link-entity and define an alias.
            // Its an outer join as we want to return results even if not regarding an case
            var queryActivityMimeAttachmentIncident = queryActivityMimeAttachmentEmail.AddLink(Case.EntityLogicalName,
                                                                                                 Email.RegardingObjectId,                                                                       Case.IncidentId,
                                                                                                 JoinOperator.LeftOuter);
            queryActivityMimeAttachmentIncident.EntityAlias = "case";

            // Add columns to Case Entity
            queryActivityMimeAttachmentIncident.Columns.AddColumns(Case.Title, Case.IncidentId, Case.CaseType, Case.TicketNumber);

            // Add link-entity to Application Entity from Case and define an alias
            var queryActivityMimeAttachmentEmailApplicationIncident = queryActivityMimeAttachmentIncident.AddLink(
                Application.EntityLogicalName,
                Case.Application, 
                Application.ApplicationId,
                JoinOperator.LeftOuter);
            queryActivityMimeAttachmentEmailApplicationIncident.EntityAlias = "case.application";

            // Add columns to Application Entity that we got via Case
            queryActivityMimeAttachmentEmailApplicationIncident.Columns.AddColumns(Application.ApplicationNumber, Application.Name, Application.PermitNumber);

            var results = Service.RetrieveMultiple(queryActivityMimeAttachment);
            if (results != null && results.Entities.Count >= 1)
            {
                return results.Entities[0];
            }

            return null;
        }

        private Entity ReturnEmailData(Guid recordId)
        {
            var queryEmail = new QueryExpression(Email.EntityLogicalName);
            queryEmail.TopCount = 1;

            // Add columns for Email entity
            queryEmail.ColumnSet.AddColumns(Email.Description, Email.Subject, Email.ActivityId, Email.StatusCode, Email.RegardingObjectId, Email.DirectionCode, Email.Sender, Email.ToRecipients, Email.CreatedOn, Email.UploadedToSharePoint);

            // Define filter
            queryEmail.Criteria.AddCondition(Email.ActivityId, ConditionOperator.Equal, recordId);

            // Add Application link-entity and define an alias.
            // Its an outer join as we want to return results even if not regarding an application
            var queryEmailApplication = queryEmail.AddLink(Application.EntityLogicalName, Email.RegardingObjectId, Application.ApplicationId, JoinOperator.LeftOuter);
            queryEmailApplication.EntityAlias = "application";

            // Add columns for Application link entity
            queryEmailApplication.Columns.AddColumns(Application.ApplicationNumber, Application.Name, Application.PermitNumber);

            // Add link-entity to Case and define an alias.
            // Its an outer join as we want to return results even if not regarding a case
            var queryEmailIncident = queryEmail.AddLink(Case.EntityLogicalName, Case.RegardingObjectId, Case.IncidentId, JoinOperator.LeftOuter);
            queryEmailIncident.EntityAlias = "case";

            // Add columns for Case link entity
            queryEmailIncident.Columns.AddColumns(Case.CaseType, Case.Title, Case.IncidentId, Case.TicketNumber);

            // Add link-entity from Case to Application and define an alias
            var queryEmailIncidentApplication = queryEmailIncident.AddLink(Application.EntityLogicalName, Case.Application, Application.ApplicationId, JoinOperator.LeftOuter);
            queryEmailIncidentApplication.EntityAlias = "case.application";

            // Add columns to for Application entity regarding the Case
            queryEmailIncidentApplication.Columns.AddColumns(Application.ApplicationNumber, Application.Name, Application.PermitNumber);

            var results = Service.RetrieveMultiple(queryEmail);
            if (results != null && results.Entities.Count >= 1)
            {
                return results.Entities[0];
            }

            return null;
        }

        private string SendRequest(string url, string requestContent)
        {
            TracingService.Trace("Sending request to {0}", url);

            using (var httpclient = new HttpClient())
            {
                var httpRequest = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Post,
                    Content = new StringContent(requestContent, Encoding.UTF8, "application/json"),
                };

                var response = httpclient.SendAsync(httpRequest).Result;
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        private string SpRemoveIllegalChars(string fileName)
        {
            return SpRemoveIllegalChars(fileName, true);
        }

        private string SpRemoveIllegalChars(string fileName, bool removeWhiteSpace)
        {
            fileName = new Regex(@"\.(?!(\w{3,4}$))").Replace(fileName, "");
            var forbiddenChars = @"#%&*:<>?/{|}~".ToCharArray();
            fileName = new string(fileName.Where(c => !forbiddenChars.Contains(c)).ToArray());
            if (removeWhiteSpace)
            {
                fileName = Regex.Replace(fileName, @"\s", ""); // Removes whitespace
            }
            if (fileName.Length >= 76)
            {
                fileName = fileName.Remove(75);
            }
            return fileName;
        }

        /// <summary>
        /// Issue SendFileToSharePoint Message which will trigger the SendSingleAttachmentToSharePoint Plugin.
        /// Need to process as a series of async plugin requests because we don't know how many attachments there might be
        /// so could exceed the limit of 2 minutes.
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        internal void SendFileToSharePointActionRequest(string entityName, Guid id)
        {
            var actionRequest = new OrganizationRequest(PluginMessages.SendFileToSharePoint)
            {
                [PluginInputParams.TargetEntityName] = entityName,
                [PluginInputParams.TargetEntityId] = id.ToString()
            };
            Service.Execute(actionRequest);
        }
    }
}