using Core.Configuration;
using Core.Helpers.Extensions;
using Lp.DataAccess;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Lp.Model.Crm;
using Lp.Model.EarlyBound;
using Email = Lp.Model.EarlyBound.Email;
using System.Net;

namespace Defra.Lp.Common.SharePoint
{
    internal class AzureInterface
    {
        private IDictionary<string, string> Config { get; set; }
        private IOrganizationService Service { get; set; }
        private IOrganizationService AdminService { get; set; }
        private ITracingService TracingService { get; set; }

        #region Data Access Layers
        /// <summary>
        /// Application Documents DAL
        /// </summary>
        private DataAcessApplicationDocument _dalApplicationDocument = null;
        private DataAcessApplicationDocument DalApplicationDocument
        {
            get
            {
                if (_dalApplicationDocument == null)
                {
                    _dalApplicationDocument = new DataAcessApplicationDocument(Service, TracingService);
                }

                return _dalApplicationDocument;
            }
        }
        #endregion

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
            TracingService.Trace($"In CreateFolder with Entity Type {application.LogicalName} and Entity Id {application.Id}");
            
            var applicationEntity = Query.RetrieveDataForEntityRef(Service, new[] { defra_application.Fields.defra_name, defra_application.Fields.defra_permitnumber, defra_application.Fields.defra_applicationnumber }, application);

            TracingService.Trace($"Permit Number = {applicationEntity[defra_application.Fields.defra_permitnumber]}; Application Number = {applicationEntity[defra_application.Fields.defra_applicationnumber]}");

            var request = new DocumentRelayRequest
            {
                ApplicationContentType = Config[$"{SharePointSecureConfigurationKeys.ApplicationFolderContentType}"],
                ApplicationNo = SharePointFilters.FilterPath(applicationEntity.GetAttributeValue<string>(defra_application.Fields.defra_applicationnumber)),
                FileBody = string.Empty,
                FileDescription = string.Empty,
                FileName = string.Empty,
                ListName = Config[$"{SharePointSecureConfigurationKeys.PermitListName}"],
                PermitContentType = Config[$"{SharePointSecureConfigurationKeys.PermitFolderContentType}"],
                PermitNo = SharePointFilters.FilterPath(applicationEntity.GetAttributeValue<string>(defra_application.Fields.defra_permitnumber)),
                Customer = string.Empty,
                SiteDetails = string.Empty,
                PermitDetails = string.Empty
            };

            var stringContent = JsonConvert.SerializeObject(request);

            TracingService.Trace($"Data Sent to Logic App URL {Config[$"{SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl}"]}");

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
                var direction = (bool)(attachmentData.GetAttributeValue<AliasedValue>($"email.{Email.Fields.DirectionCode}")).Value;
                var statusCode = (OptionSetValue)(attachmentData.GetAttributeValue<AliasedValue>($"email.{Email.Fields.StatusCode}")).Value;
                if (direction && statusCode.Value != 3)
                {
                    //Outgoing email, do not send the attachment on create
                    TracingService.Trace("Aborted creating attachment for outgoing email attachment that is not sent");
                    return;
                }

                AddInsertFileParametersToRequest(request, attachmentData);
                // Check there is a file to upload
                if (request.HasBody())
                {
                    var resultBody = SendRequest(Config[SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl], JsonConvert.SerializeObject(request));
                    if (resultBody != null)
                    {
                        TracingService.Trace("Returned from LogicApp OK");

                        // Get the response object
                        DocumentRelayResponse response = JsonConvert.DeserializeObject<DocumentRelayResponse>(resultBody);

                        TracingService.Trace($"Returned from LogicApp OK, doc url is: {response.link}");

                        // Create document record
                        CreateApplicationDocument(attachmentData, request.FileDescription, request.FileName, response.link);


                        // Delete Attachment
                        Service.Delete(attachmentData.LogicalName, attachmentData.Id);
                    }
                }
                else
                {
                    TracingService.Trace("No file body found for Attachment. Logic app not called.");
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
                var direction = emailData.GetAttributeValue<bool>(Email.Fields.DirectionCode);
                var statusCode = emailData.GetAttributeValue<OptionSetValue>(Email.Fields.StatusCode);
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
                        emailData[Email.Fields.defra_uploadedtosharepoint] = true;
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

            // Check that Note is regarding an application or a case
            if (IsRegardingValidForNote(annotationData))
            {
                AddInsertFileParametersToRequest(request, annotationData);
                // Check there is a file to upload
                if (request.HasBody())
                {
                    var resultBody = SendRequest(Config[$"{SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl}"], JsonConvert.SerializeObject(request));
                    if (resultBody == null)
                    {
                        TracingService.Trace("Returned from LogicApp, no resultBody");
                        return;
                    }

                    // Get the response object
                    DocumentRelayResponse response = JsonConvert.DeserializeObject<DocumentRelayResponse>(resultBody);

                    TracingService.Trace($"Returned from LogicApp OK, doc url is: {response.link}");
                            
                    // Create document record
                    CreateApplicationDocument(annotationData, request.FileDescription, request.FileName, response.link);

                    // Blank Note Document
                    annotationData[Annotation.Fields.NoteText] = "File has been uploaded to SharePoint.";
                    annotationData[Annotation.Fields.DocumentBody] = string.Empty;
                    Service.Update(annotationData);
                }
                else
                {
                    TracingService.Trace("No file body found for Note. Logic app not called.");
                }
            }
        }

        private bool IsRegardingValidForNote(Entity annotationData)
        {
            // Got an application, thats OK
            if (annotationData.Contains("application.defra_applicationid") 
                && !string.IsNullOrEmpty(((Guid)((AliasedValue)annotationData.Attributes["application.defra_applicationid"]).Value).ToString()))
            {
                TracingService.Trace("Got an application - OK.");
                return true;
            }
            // Got a case, thats OK
            if (annotationData.Contains("case.incidentid")
                && !string.IsNullOrEmpty(((Guid)((AliasedValue)annotationData.Attributes["case.incidentid"]).Value).ToString()))
            {
                TracingService.Trace("Got a case - OK.");
                return true;
            }
            TracingService.Trace("Note not processed as its not a case or an application.");
            return false;
        }

        internal void UpdateMetaData(EntityReference entity, string customer, string siteDetails, string permitDetails)
        {
            TracingService.Trace("In UpdateMetaData with Entity Type {0} and Entity Id {1}", entity.LogicalName, entity.Id.ToString());

            var request = new MetaDataRequest();

            if (entity.LogicalName == defra_application.EntityLogicalName)
            {
                var applicationEntity = Query.RetrieveDataForEntityRef(Service, new[] { Application.Name, Application.PermitNumber, Application.ApplicationNumber, defra_application.Fields.defra_eawmlnumber }, entity);
                if (applicationEntity != null)
                {
                    TracingService.Trace($"Permit Number = {applicationEntity[Application.PermitNumber]}; Application Number = {applicationEntity[Application.ApplicationNumber]}");

                    request.ApplicationNo = SharePointFilters.FilterPath(applicationEntity.GetAttributeValue<string>(Application.ApplicationNumber));
                    request.ListName = Config[$"{SharePointSecureConfigurationKeys.PermitListName}"];
                    request.PermitNo = SharePointFilters.FilterPath(applicationEntity.GetAttributeValue<string>(Application.PermitNumber));
                    request.Customer = customer;
                    request.SiteDetails = siteDetails;
                    request.PermitDetails = permitDetails;
                    request.UpdateType = AzureInterfaceConstants.MetaDataApplicationUpdateType;
                    request.EawmlNo = applicationEntity.GetAttributeValue<string>(defra_application.Fields.defra_eawmlnumber);
                }
                else
                {
                    throw new InvalidPluginExecutionException(string.Format("No Application exists for entity reference {0}", entity.Id.ToString()));
                }
            }
            else if (entity.LogicalName == defra_permit.EntityLogicalName)
            {
                var permitEntity = Query.RetrieveDataForEntityRef(Service, new[] { Permit.Name, Permit.PermitNumber, defra_permit.Fields.defra_eawmlnumber }, entity);
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
                    request.EawmlNo = permitEntity.GetAttributeValue<string>(defra_permit.Fields.defra_eawmlnumber);
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
                if (queryRecord.Contains(Email.Fields.Sender))
                {
                    request.EmailFrom = queryRecord.GetAttributeValue<string>(Email.Fields.Sender);
                }

                if (queryRecord.Contains(Email.Fields.ToRecipients))
                {
                    request.EmailTo = queryRecord.GetAttributeValue<string>(Email.Fields.ToRecipients);
                }

                if (queryRecord.Contains($"email.{Email.Fields.Sender}"))
                {
                    request.EmailFrom = ((string)((AliasedValue)queryRecord.Attributes[$"email.{Email.Fields.Sender}"]).Value);
                }

                if (queryRecord.Contains($"email.{Email.Fields.ToRecipients}"))
                {
                    request.EmailTo = ((string)((AliasedValue)queryRecord.Attributes[$"email.{Email.Fields.ToRecipients}"]).Value);
                } 
                if (queryRecord.Contains(Email.Fields.Subject))
                {
                    // For an email, this is just the filename again
                    request.EmailLink = GetFileName(queryRecord);
                }
                else if (queryRecord.Contains($"email.{Email.Fields.Subject}"))
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
            // to do: change subject based on naming convention 
            var newSubject = GetSubject(queryRecord);
            newSubject = GetNamingConvention(queryRecord, newSubject);

            var crmId = GetCrmId(queryRecord);
            var caseNo = GetCaseFolderName(queryRecord);
            var regarding = GetRegarding(queryRecord);
            var subject = newSubject;
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

        private string GetNamingConvention(Entity queryRecord, string newSubject)
        {
            if (queryRecord.Contains("objectid") && queryRecord["objecttypecode"].ToString() == "defra_application")
            {

                var con = "";


                EntityReference appReg = null;
                EntityReference appTrack = null;
                var appType = -1;
                Guid appSubType = Guid.Empty;

                var app = Service.Retrieve(Application.EntityLogicalName, ((EntityReference)queryRecord["objectid"]).Id, new ColumnSet(defra_application.Fields.defra_regimeid, defra_application.Fields.defra_businesstrackid, defra_application.Fields.defra_applicationtype, defra_application.Fields.defra_application_subtype));
                if (app.Contains(defra_application.Fields.defra_regimeid))
                {
                    appReg = ((EntityReference)app[defra_application.Fields.defra_regimeid]);
                    con += "<condition attribute = 'defra_regime' operator= 'eq'  value ='{" + appReg.Id.ToString() + "}'/>";
                }
                if (app.Contains(defra_application.Fields.defra_businesstrackid))
                {
                    appTrack = ((EntityReference)app[defra_application.Fields.defra_businesstrackid]);
                    con += "<condition attribute = 'defra_track' operator= 'eq'  value = '{" + appTrack.Id.ToString() + "}' />";
                }
                if (app.Contains(defra_application.Fields.defra_applicationtype))
                {
                    appType = (app[defra_application.Fields.defra_applicationtype] as OptionSetValue).Value;
                    con += $"<condition attribute = 'defra_applicationtype' operator= 'eq' value = '{ appType.ToString() }' />";
                }
                if (app.Contains(defra_application.Fields.defra_application_subtype))
                {
                    appSubType = (app[defra_application.Fields.defra_application_subtype] as EntityReference).Id;
                    con += "< condition attribute = 'defra_applicationsubtype' operator= 'eq'  value = '{" + appSubType.ToString() + "}' />";
                }

                var fetchXml = $@"<fetch >
                                      <entity name='defra_documentnamingconventionmapping'>
                                         <filter type='and'>
                                          <condition attribute='defra_name' operator='eq' value='" + newSubject + @"' />{con}
                                        </filter>
                                      </entity>
                                    </fetch>";
                var result = Query.QueryCRMForSingleEntity(Service, fetchXml);

                if (result != null && result.Contains("defra_value"))
                {
                    newSubject = result["defra_value"].ToString();
                }


            }

            return newSubject;
        }

        private EntityReference GetRegardingObjectId(Entity queryRecord)
        {
            EntityReference regardingObjectRef = null;
            // Email
            if (queryRecord.Contains(Email.Fields.RegardingObjectId))
            {
                regardingObjectRef = queryRecord.GetAttributeValue<EntityReference>(Email.Fields.RegardingObjectId);
                TracingService.Trace($"GetRegardingObjectId() regardingObjectRef.LogicalName={regardingObjectRef.LogicalName}");

            }
            // Attachment
            if (queryRecord.Contains($"email.{Email.Fields.RegardingObjectId}"))
            {
                regardingObjectRef = (EntityReference)(queryRecord.GetAttributeValue<AliasedValue>($"email.{Email.Fields.RegardingObjectId}")).Value;
                TracingService.Trace($"GetRegardingObjectId() regardingObjectRef.LogicalName={regardingObjectRef.LogicalName}");
            }

            if (regardingObjectRef == null)
            {
                TracingService.Trace("GetRegardingObjectId() found no regarding object");
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
                title = title.SpRemoveIllegalChars();
           
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
            if (queryRecord.Contains(Annotation.Fields.Id))
            {
                crmId = queryRecord.GetAttributeValue<Guid>(Annotation.Fields.Id).ToString();
            }
            // Email
            if (queryRecord.Contains(Email.Fields.ActivityId))
            {
                crmId = queryRecord.GetAttributeValue<Guid>(Email.Fields.ActivityId).ToString();
            }
            // Attachment
            if (queryRecord.Contains($"email.{Email.Fields.ActivityId}"))
            {
                crmId = ((Guid)((AliasedValue)queryRecord.Attributes[$"email.{Email.Fields.ActivityId}"]).Value).ToString();
            }
            TracingService.Trace("Crm Id: {0}", crmId);
            return crmId;
        }

        private string GetDescription(Entity queryRecord)
        {
            var desc = string.Empty;
            // Emails
            if (queryRecord.Contains(Email.Fields.Subject))
            {
                desc = queryRecord.GetAttributeValue<string>(Email.Fields.Subject);
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
            permitNo = SharePointFilters.FilterPath(permitNo);
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
            }
            if (queryRecord.Contains("case.application.defra_applicationnumber"))
            {
                applicationNo = (string)((AliasedValue)queryRecord.Attributes["case.application.defra_applicationnumber"]).Value;
            }

            applicationNo = SharePointFilters.FilterPath(applicationNo);
            TracingService.Trace("Application No: {0}", applicationNo);
            return applicationNo;
        }

        private string GetSubject(Entity queryRecord)
        {
            var subject = string.Empty;
            // email
            if (queryRecord.Contains(Email.Fields.Subject))
            {
                subject = queryRecord.GetAttributeValue<string>(Email.Fields.Subject);
            }
            // attachment
            if (queryRecord.Contains($"email.{Email.Fields.Subject}"))
            {
                subject = (string)((AliasedValue)queryRecord.Attributes[$"email.{Email.Fields.Subject}"]).Value;
            }
            TracingService.Trace("Subject: {0}", subject);
            return subject;
        }

        private string CreateEmailFileName(Entity queryRecord)
        {
            // For an email, we're going to use the subject as the filename.
            var fileName = queryRecord.GetAttributeValue<string>(Email.Fields.Subject);
            var createdDate = queryRecord.GetAttributeValue<DateTime>(Email.Fields.CreatedOn);
            // Filename needs to have a timestamp so that CRM doesn't overwrite if the
            // user uploads something with the same name from front end. Also need to remove
            // any illegal charcter that SharePoint might complain about
            fileName = fileName.SpRemoveIllegalChars().AppendTimeStamp(createdDate);

            // Give it an HTML ending as we want to view it in SharePoint as HTML
            fileName = fileName + ".html";

            return fileName;
        }

        private string CreateEmailFileNameForAttachment(Entity queryRecord)
        {
            // For an email, we're going to use the subject as the filename.
            var fileName = (string)((AliasedValue)queryRecord.Attributes[$"email.{Email.Fields.Subject}"]).Value;
            var createdDate = (DateTime)((AliasedValue)queryRecord.Attributes[$"email.{Email.Fields.CreatedOn}"]).Value;
            // Filename needs to have a timestamp so that CRM doesn't overwrite if the
            // user uploads something with the same name from front end. Also need to remove
            // any illegal charcter that SharePoint might complain about
            fileName = fileName.SpRemoveIllegalChars().AppendTimeStamp(createdDate);

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
            if (queryRecord.Contains(Email.Fields.CreatedOn))
            {
                // annotation and email
                createdDate = queryRecord.GetAttributeValue<DateTime>(Email.Fields.CreatedOn);
            }
            if (queryRecord.Contains($"email.{Email.Fields.CreatedOn}"))
            {
                // attachments
                createdDate = (DateTime)((AliasedValue)queryRecord.Attributes[$"email.{Email.Fields.CreatedOn}"]).Value;
            }
            if (queryRecord.Contains("filename"))
            {
                // attachments and annotations have a filename
                fileName = queryRecord.GetAttributeValue<string>("filename");

                // Filename needs to have a timestamp so that CRM doesn't overwrite if the
                // user uploads something with the same name from front end. Also need to remove
                // any illegal charcter that SharePoint might complain about
                fileName = fileName.SpRemoveIllegalChars().AppendTimeStamp(createdDate);
            }
            else if (queryRecord.Contains(Email.Fields.Subject))
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
                var desc = queryRecord.GetAttributeValue<string>(Email.Fields.Description);
                desc = string.IsNullOrEmpty(desc) ? string.Empty : desc;
                var sender = queryRecord.GetAttributeValue<string>(Email.Fields.Sender);
                sender = string.IsNullOrEmpty(sender) ? string.Empty : sender;
                var to = queryRecord.GetAttributeValue<string>(Email.Fields.ToRecipients);
                to = string.IsNullOrEmpty(to) ? string.Empty : to;
                var subject = queryRecord.GetAttributeValue<string>(Email.Fields.Subject);
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
            queryAnnotation.ColumnSet.AddColumns(
                Annotation.Fields.Subject, 
                Annotation.Fields.DocumentBody, 
                Annotation.Fields.FileName, 
                Annotation.Fields.Id, 
                Annotation.Fields.NoteText, 
                Annotation.Fields.FileSize,
                Annotation.Fields.IsDocument, 
                Annotation.Fields.CreatedOn,
                Annotation.Fields.CreatedBy,Annotation.Fields.ObjectId);

            // Define filter on Primary key
            queryAnnotation.Criteria.AddCondition(Annotation.Fields.Id, ConditionOperator.Equal, recordId);

            // Add link-entity to defra_application. Outer join as it might be regarding a case or application
            var queryExpressionAnnotationApp = queryAnnotation.AddLink(Application.EntityLogicalName, Annotation.Fields.ObjectId, Application.ApplicationId, JoinOperator.LeftOuter);
            queryExpressionAnnotationApp.EntityAlias = "application";

            // Add columns to Application entity
            queryExpressionAnnotationApp.Columns.AddColumns(
                defra_application.Fields.defra_applicationId,
                defra_application.Fields.defra_name, 
                defra_application.Fields.defra_permitnumber, 
                defra_application.Fields.defra_applicationnumber, 
                defra_application.Fields.StatusCode, 
                defra_application.Fields.OwningUser, 
                defra_application.Fields.OwningTeam);

            // Add link-entity to Case. Outer join as it might be regarding case or application
            var queryExpressionAnnotationIncident = queryAnnotation.AddLink(Incident.EntityLogicalName, Annotation.Fields.ObjectId, Incident.Fields.IncidentId, JoinOperator.LeftOuter);
            queryExpressionAnnotationIncident.EntityAlias = "case";

            // Add columns to Case entity
            queryExpressionAnnotationIncident.Columns.AddColumns(
                Incident.Fields.Title, 
                Incident.Fields.IncidentId, 
                Incident.Fields.CaseTypeCode, 
                Incident.Fields.TicketNumber,
                Incident.Fields.IncidentId,
                Incident.Fields.OwningUser,
                Incident.Fields.OwningTeam);

            // Add link-entity from case to application
            var queryExpressionAnnotationIncidentApplication = queryExpressionAnnotationIncident.AddLink(defra_application.EntityLogicalName, Case.Application, defra_application.Fields.defra_applicationId, JoinOperator.LeftOuter);
            queryExpressionAnnotationIncidentApplication.EntityAlias = "case.application";

            // Add columns to QEannotation_incident_defra_application.Columns
            queryExpressionAnnotationIncidentApplication.Columns.AddColumns(defra_application.Fields.defra_applicationnumber, defra_application.Fields.defra_name, defra_application.Fields.defra_permitnumber, defra_application.Fields.defra_applicationId);

            var results = Service.RetrieveMultiple(queryAnnotation);
            if (results != null && results.Entities.Count >= 1)
            {
                CrmExtensions.TraceEntity(TracingService, results.Entities[0]);
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
            queryActivityMimeAttachment.ColumnSet.AddColumns(ActivityMimeAttachment.Filename, ActivityMimeAttachment.Body, ActivityMimeAttachment.Id);

            // Define filter on Primary Key
            queryActivityMimeAttachment.Criteria.AddCondition(ActivityMimeAttachment.Id, ConditionOperator.Equal, recordId);

            // Add link-entity QEactivitymimeattachment_email. Inner join as it must be regarding an Email
            var queryActivityMimeAttachmentEmail = queryActivityMimeAttachment.AddLink(Email.EntityLogicalName, ActivityMimeAttachment.ObjectId, Email.Fields.ActivityId);
            queryActivityMimeAttachmentEmail.EntityAlias = "email";

            // Add columns to QEactivitymimeattachment_email.Columns
            queryActivityMimeAttachmentEmail.Columns.AddColumns(
                Email.Fields.Description,
                Email.Fields.Subject, 
                Email.Fields.DirectionCode, 
                Email.Fields.ActivityId, 
                Email.Fields.StatusCode, 
                Email.Fields.RegardingObjectId, 
                Email.Fields.Sender, 
                Email.Fields.ToRecipients, 
                Email.Fields.CreatedOn, 
                Email.Fields.OwningUser,
                Email.Fields.OwningTeam);

            // Add Application link-entity and define an alias.
            // Its an outer join as we want to return results even if not regarding an application
            var queryActivityMimeAttachmentEmailApplication = queryActivityMimeAttachmentEmail.AddLink(
                Application.EntityLogicalName, 
                Email.Fields.RegardingObjectId,                                                                                  
                Application.ApplicationId,
                JoinOperator.LeftOuter);

            queryActivityMimeAttachmentEmailApplication.EntityAlias = "application";

            // Add columns to Application Entity
            queryActivityMimeAttachmentEmailApplication.Columns.AddColumns(Application.Name, Application.PermitNumber, Application.ApplicationNumber, defra_application.Fields.defra_applicationId);

            // Add Application link-entity and define an alias.
            // Its an outer join as we want to return results even if not regarding an case
            var queryActivityMimeAttachmentIncident = queryActivityMimeAttachmentEmail.AddLink(Case.EntityLogicalName,
                                                                                                 Email.Fields.RegardingObjectId,                                                                       Case.IncidentId,
                                                                                                 JoinOperator.LeftOuter);
            queryActivityMimeAttachmentIncident.EntityAlias = "case";

            // Add columns to Case Entity
            queryActivityMimeAttachmentIncident.Columns.AddColumns(Case.Title, Case.IncidentId, Case.CaseType, Case.TicketNumber, Incident.Fields.IncidentId);

            // Add link-entity to Application Entity from Case and define an alias
            var queryActivityMimeAttachmentEmailApplicationIncident = queryActivityMimeAttachmentIncident.AddLink(
                Application.EntityLogicalName,
                Case.Application, 
                Application.ApplicationId,
                JoinOperator.LeftOuter);
            queryActivityMimeAttachmentEmailApplicationIncident.EntityAlias = "case.application";

            // Add columns to Application Entity that we got via Case
            queryActivityMimeAttachmentEmailApplicationIncident.Columns.AddColumns(Application.ApplicationNumber, Application.Name, Application.PermitNumber, defra_application.Fields.defra_applicationId);

            var results = Service.RetrieveMultiple(queryActivityMimeAttachment);
            if (results != null && results.Entities.Count >= 1)
            {
                CrmExtensions.TraceEntity(TracingService, results.Entities[0]);
                return results.Entities[0];
            }

            return null;
        }

        private Entity ReturnEmailData(Guid recordId)
        {
            var queryEmail = new QueryExpression(Email.EntityLogicalName);
            queryEmail.TopCount = 1;

            // Add columns for Email entity
            queryEmail.ColumnSet.AddColumns(Email.Fields.Description, Email.Fields.Subject, Email.Fields.ActivityId, Email.Fields.StatusCode, Email.Fields.RegardingObjectId, Email.Fields.DirectionCode, Email.Fields.Sender, Email.Fields.ToRecipients, Email.Fields.CreatedOn, Email.Fields.defra_uploadedtosharepoint);

            // Define filter
            queryEmail.Criteria.AddCondition(Email.Fields.ActivityId, ConditionOperator.Equal, recordId);

            // Add Application link-entity and define an alias.
            // Its an outer join as we want to return results even if not regarding an application
            var queryEmailApplication = queryEmail.AddLink(Application.EntityLogicalName, Email.Fields.RegardingObjectId, Application.ApplicationId, JoinOperator.LeftOuter);
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
            queryEmailIncidentApplication.Columns.AddColumns(Application.ApplicationNumber, Application.Name, Application.PermitNumber, defra_application.Fields.defra_applicationId);

            var results = Service.RetrieveMultiple(queryEmail);
            if (results != null && results.Entities.Count >= 1)
            {
                CrmExtensions.TraceEntity(TracingService, results.Entities[0]);
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

        #region CRM data access

        private void CreateApplicationDocument(Entity attachmentData, string fileDescription, string fileName, string url)
        {
            // Create document source record
            Guid? createdById = attachmentData.GetAttributeId(defra_application.Fields.CreatedBy);
            Guid? applicationId = attachmentData.GetAliasedAttributeId($"application.{defra_application.Fields.defra_applicationId}");
            Guid? caseApplicationId = attachmentData.GetAliasedAttributeId($"case.application.{defra_application.Fields.defra_applicationId}");
            Guid? caseId = attachmentData.GetAliasedAttributeId($"case.{Incident.Fields.IncidentId}");
            Guid? emailId = attachmentData.GetAliasedAttributeId($"email.{Email.Fields.ActivityId}");
            Guid? emailOwningUserId = attachmentData.GetAliasedAttributeId($"email.{Email.Fields.OwningUser}");
            Guid? emailOwningTeamId = attachmentData.GetAliasedAttributeId($"email.{Email.Fields.OwningTeam}");
            Guid? appOwningUserId = attachmentData.GetAliasedAttributeId($"application.{defra_application.Fields.OwningUser}");
            Guid? appOwningTeamId = attachmentData.GetAliasedAttributeId($"application.{defra_application.Fields.OwningTeam}");
            Guid? caseOwningUserId = attachmentData.GetAliasedAttributeId($"case.{Incident.Fields.OwningUser}");
            Guid? caseOwningTeamId = attachmentData.GetAliasedAttributeId($"case.{Incident.Fields.OwningTeam}");


            defra_ApplicationDocumentSource documentSource;

            if (emailId.HasValue)
            {
                documentSource = defra_ApplicationDocumentSource.Email;
            }
            else if (caseId.HasValue)
            {
                documentSource = defra_ApplicationDocumentSource.RFIorS5;
            }
            else
            {
                documentSource = defra_ApplicationDocumentSource.Application;
            }

            DalApplicationDocument.CreateApplicationDocument(
                fileDescription,
                url,
                fileName,
                documentSource,
                applicationId ?? caseApplicationId,
                caseId,
                emailId,
                createdById,
                emailOwningUserId ?? appOwningUserId ?? caseOwningUserId,
                emailOwningTeamId ?? appOwningTeamId ?? caseOwningTeamId);
        }

        #endregion

    }
}