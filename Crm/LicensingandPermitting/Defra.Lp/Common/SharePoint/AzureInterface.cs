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
            var applicationEntity = Query.RetrieveDataForEntityRef(Service, new string[] { Application.Name, Application.PermitNumber, Application.ApplicationNumber }, application);

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

        internal void UploadFile(EntityReference recordIdentifier, string parentEntity, string parentLookup)
        {
            TracingService.Trace(string.Format("In UploadFile with Entity Type {0} and Entity Id {1}. Parent Entity: {2} Lookup Name: {3}", recordIdentifier.LogicalName, recordIdentifier.Id, parentEntity, parentLookup));
            
            var request = new DocumentRelayRequest();
            Entity annotationData = null;
            Entity attachmentData = null;
            Entity emailData = null;

            if (parentEntity == Email.EntityLogicalName)
            {
                // This will have been fired against the creation of the Activity Mime Attachment Record. We want to
                // process the email attachments and upload to SharePoint.
                attachmentData = ReturnAttachmentData(recordIdentifier.Id);
                if (attachmentData == null)
                {
                    throw new InvalidPluginExecutionException("No attachment data record returned from query");
                }

                bool direction = (bool)(attachmentData.GetAttributeValue<AliasedValue>("email.directioncode")).Value;

                OptionSetValue statusCode = (OptionSetValue)(attachmentData.GetAttributeValue<AliasedValue>("email.statuscode")).Value;
                if (direction && statusCode.Value != 3)
                {
                    //Outgoing email, do not send the attachment on create
                    TracingService.Trace("Aborted creating attachment for outgoing email attachment that is not sent");
                    return;
                }

                AddInsertFileParametersToRequest(request, attachmentData);
            }
            else if (parentEntity == Case.EntityLogicalName)
            {
                // Creation of an Annotation record on a Case
            }
            else
            {
                if (recordIdentifier.LogicalName == Email.EntityLogicalName)
                {
                    // Processing an email record. Need the information so we can upload the email to SharePoint
                    emailData = ReturnEmailData(recordIdentifier.Id);
                    if (emailData == null)
                    {
                        throw new InvalidPluginExecutionException("No email data record returned from query");
                    }
                    AddInsertFileParametersToRequest(request, emailData);
                }
                else
                {
                    // Creation of of an Annotation record on a Application
                    annotationData = ReturnAnnotationData(recordIdentifier.Id, parentEntity, parentLookup);
                    if (annotationData == null)
                    {
                        throw new InvalidPluginExecutionException("No annotation data record returned from query");
                    }

                    AddInsertFileParametersToRequest(request, annotationData);
                }
            }

            var stringContent = JsonConvert.SerializeObject(request);
            var logicAppUrl = Config[$"{SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl}"];
            TracingService.Trace($"Sending data to Logic App URL {logicAppUrl}");

            var resultBody = SendRequest(logicAppUrl, stringContent);

            if (resultBody!= null)
            {
                //TracingService.Trace(string.Format("SharePoint File Id {0}", data.SharePointId));
                if (annotationData != null)
                {
                    annotationData["documentbody"] = string.Empty;
                    Service.Update(annotationData);
                }
                else if (attachmentData != null)
                {
                    TracingService.Trace("Need to delete attachment");
                    attachmentData["body"] = string.Empty;
                    Service.Update(attachmentData);
                }   
            }
        }

        internal void UpdateMetaData(EntityReference entity, string customer, string siteDetails, string permitDetails)
        {
            TracingService.Trace("In UpdateMetaData with Entity Type {0} and Entity Id {1}", entity.LogicalName, entity.Id.ToString());

            var request = new MetaDataRequest();

            if (entity.LogicalName == Application.EntityLogicalName)
            {
                var applicationEntity = Query.RetrieveDataForEntityRef(Service, new string[] { Application.Name, Application.PermitNumber, Application.ApplicationNumber }, entity);
                if (applicationEntity != null)
                {
                    TracingService.Trace($"Permit Number = {applicationEntity[Application.PermitNumber]}; Application Number = {applicationEntity[Application.ApplicationNumber].ToString()}");

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
                var permitEntity = Query.RetrieveDataForEntityRef(Service, new string[] { Permit.Name, Permit.PermitNumber }, entity);
                if (permitEntity != null)
                {
                    TracingService.Trace(string.Format("Permit Number = {0}", permitEntity[Permit.PermitNumber].ToString()));

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

            request.EmailId = string.Empty;
            request.EmailRegarding = string.Empty;

            // Set Email Activity Id when we have an email
            if (queryRecord.LogicalName == Email.EntityLogicalName && queryRecord.Contains(Email.ActivityId))
            {
                request.EmailId = queryRecord.GetAttributeValue<Guid>(Email.ActivityId).ToString();
            }

            // Set Email Activity Id when we have an Attachment
            if (queryRecord.LogicalName == ActivityMimeAttachment.EntityLogicalName && queryRecord.Contains("email.activityid"))
            {
                request.EmailId = ((Guid)((AliasedValue)queryRecord.Attributes["email.activityid"]).Value).ToString();
            }

            if (queryRecord.LogicalName == Email.EntityLogicalName)
            {
                // Set the email regarding field to Schedule 5 or RFI otherwise assume its an application
                if (queryRecord.Contains("case.casetypecode"))
                {
                    var caseTypeCode = (OptionSetValue)(queryRecord.GetAttributeValue<AliasedValue>("case.casetypecode")).Value;
                    request.EmailRegarding = Query.GetCRMOptionsetText(AdminService, Case.EntityLogicalName, Case.CaseType, caseTypeCode.Value);
                }
                else
                {
                    request.EmailRegarding = "Application";
                }
            }
        }

        private void AddInsertFileParametersToRequest(DocumentRelayRequest request, Entity queryRecord)
        {
            TracingService.Trace("In AddInsertFileParametersToRequest()");
            TracingService.Trace("Logical Name: {0}", queryRecord.LogicalName);

            var permitNo = GetPermitNumber(queryRecord);
            var applicationNo = GetApplicationNumber(queryRecord);
            var fileName = GetFileName(queryRecord);
            var body = GetBody(queryRecord);
            var description = GetDescription(queryRecord);

            request.ApplicationContentType = Config[$"{SharePointSecureConfigurationKeys.ApplicationFolderContentType}"];
            request.ApplicationNo = applicationNo;
            request.FileBody = body;
            request.FileDescription = queryRecord.GetAttributeValue<string>("subject");
            request.FileName = fileName;
            request.ListName = Config[$"{SharePointSecureConfigurationKeys.PermitListName}"];
            request.PermitContentType = Config[$"{SharePointSecureConfigurationKeys.PermitFolderContentType}"];
            request.PermitNo = permitNo;
            request.Customer = string.Empty;
            request.SiteDetails = string.Empty;
            request.PermitDetails = string.Empty;

            AddEmailParametersToRequest(request, queryRecord);

            TracingService.Trace(string.Format("Requests: {0}", request));

            return;
        }

        private string GetDescription(Entity queryRecord)
        {
            var desc = string.Empty;
            if (queryRecord.Contains("subject"))
            {
                desc = (string)queryRecord.GetAttributeValue<string>("subject");
            }
            queryRecord.GetAttributeValue<string>("subject");
            TracingService.Trace("Description: {0}", desc);
            return desc;
        }

        private string GetPermitNumber(Entity queryRecord)
        {
            var permitNo = string.Empty;
            if (queryRecord.Contains("parent.defra_permitnumber"))
            {
                permitNo = (string)((AliasedValue)queryRecord.Attributes["parent.defra_permitnumber"]).Value;
            }
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
            if (queryRecord.Contains("parent.defra_applicationnumber"))
            {
                applicationNo = (string)((AliasedValue)queryRecord.Attributes["parent.defra_applicationnumber"]).Value;
                applicationNo = applicationNo.Replace('/', '_');
            }
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

        private string GetFileName(Entity queryRecord)
        {
            // Filename will be in subject for emails for filename field
            // for annotations and attachments
            var fileName = string.Empty;
            if (queryRecord.Contains("filename"))
            {
                fileName = queryRecord.GetAttributeValue<string>("filename");
            }
            else if (queryRecord.Contains(Email.Subject))
            {
                // For an email, we're going to use the subject as the filename.
                fileName = queryRecord.GetAttributeValue<string>(Email.Subject);
                // Give it an HTML ending as we want to view it in SharePoint as HTML
                fileName = fileName + ".html";
            }
            // Filename needs to have a timestamp so that CRM doesn't overwrite if the
            // user uploads something with the same name from front end. Also need to remove
            // any illegal charcter that SharePoint might complain about
            fileName = SpRemoveIllegalChars(fileName.AppendTimeStamp());

            TracingService.Trace("Filename: {0}", fileName);

            return fileName;
        }

        private string GetBody(Entity queryRecord)
        {
            var body = string.Empty;
            if (queryRecord.LogicalName == ActivityMimeAttachment.EntityLogicalName)
            {
                // For an attachment the file is in body          
                body = queryRecord.GetAttributeValue<string>("body");
            }
            else if (queryRecord.LogicalName == Email.EntityLogicalName)
            {
                // For an email the body is in description. It needs to be wrapped in 
                // a HTML Header and Body tags as we want to view it like HTML in SharePoint.
                body = string.Format("<html><head></head><body>{0}</body></html>", queryRecord.GetAttributeValue<string>(Email.Description));

                // It needs to be Base64 encoded so it matches body and documentbody
                body = body.Base64Encode();
            }
            else
            {
                // For an annotation the file is in document body
                body = queryRecord.GetAttributeValue<string>("documentbody");
            }
            var length = body.Length > 200 ? 200 : body.Length - 1;
            TracingService.Trace("Body (first {1} chars): {0}", body.Substring(length), length.ToString());
            return body;
        }

        //private Entity ReturnAnnotationData(Guid recordId, string parentEntityName, string parentLookup)
        //{
        //    // Instantiate QueryExpression QEannotation
        //    var QEannotation = new QueryExpression("annotation");
        //    QEannotation.TopCount = 1;

        //    // Add columns to QEannotation.ColumnSet
        //    QEannotation.ColumnSet.AddColumns("subject", "documentbody", "filename", "annotationid");
        //    QEannotation.AddOrder("subject", OrderType.Ascending);

        //    // Define filter QEannotation.Criteria
        //    QEannotation.Criteria.AddCondition("annotationid", ConditionOperator.Equal, "4e0d7f5e-0a64-e811-a958-000d3ab31ad6");

        //    // Add link-entity QEannotation_defra_application
        //    var QEannotation_defra_application = QEannotation.AddLink("defra_application", "objectid", "defra_applicationid");
        //    QEannotation_defra_application.EntityAlias = "parent";

        //    // Add columns to QEannotation_defra_application.Columns
        //    QEannotation_defra_application.Columns.AddColumns("defra_applicationid", "defra_name", "defra_permitnumber", "defra_applicationnumber", "statuscode");

        //    // Define filter QEannotation_defra_application.LinkCriteria
        //    QEannotation_defra_application.LinkCriteria.AddCondition("statuscode", ConditionOperator.NotNull);
        //}

        private Entity ReturnAnnotationData(Guid recordId, string parentEntityName, string parentLookup)
        {
            var fetchXml = string.Format(@"<fetch top='1' >
                                            <entity name='annotation' >
                                            <attribute name='subject' />
                                            <attribute name='documentbody' />
                                            <attribute name='filename' />
                                            <attribute name='annotationid' />
                                            <order attribute='subject' descending='false' />
                                            <filter type='and' >
                                                <condition attribute='annotationid' operator='eq' uitype='annotation' value='{0}' />
                                            </filter>
                                            <link-entity name='{1}' from='{2}' to='objectid' alias='parent' link-type='inner' >
                                                <attribute name='{2}' />
                                                <attribute name='defra_name' />
                                                <attribute name='defra_permitnumber' />
                                                <attribute name='defra_applicationnumber' />
                                                <attribute name='statuscode' />
                                                <filter type='and' >
                                                <condition attribute='statuscode' operator='not-null' />
                                                </filter>
                                            </link-entity>
                                            </entity>
                                        </fetch>", recordId, parentEntityName, parentLookup);

            return Query.QueryCRMForSingleEntity(Service, fetchXml);
        }

        private Entity ReturnAttachmentData(Guid recordId)
        {
            // Instantiate QueryExpression QEactivitymimeattachment
            var QEactivitymimeattachment = new QueryExpression(ActivityMimeAttachment.EntityLogicalName);
            QEactivitymimeattachment.TopCount = 1;

            // Add columns to ActivityMimeAttachment Entity
            QEactivitymimeattachment.ColumnSet.AddColumns(ActivityMimeAttachment.Filename, ActivityMimeAttachment.Body);

            // Define filter on Primary Key
            QEactivitymimeattachment.Criteria.AddCondition(ActivityMimeAttachment.Id, ConditionOperator.Equal, recordId);

            // Add link-entity QEactivitymimeattachment_email. Inner join as it must be regarding an Email
            var QEactivitymimeattachment_email = QEactivitymimeattachment.AddLink(Email.EntityLogicalName, 
                                                                                  ActivityMimeAttachment.ObjectId, 
                                                                                  Email.ActivityId);
            QEactivitymimeattachment_email.EntityAlias = "email";

            // Add columns to QEactivitymimeattachment_email.Columns
            QEactivitymimeattachment_email.Columns.AddColumns(Email.DirectionCode, Email.ActivityId, Email.StatusCode, Email.RegardingObjectId);

            // Add Application link-entity and define an alias.
            // Its an outer join as we want to return results even if not regarding an application
            var QEactivitymimeattachment_email_defra_application = QEactivitymimeattachment_email.AddLink(Application.EntityLogicalName, 
                                                                                                          Email.RegardingObjectId, 
                                                                                                          Application.ApplicationId,
                                                                                                          JoinOperator.LeftOuter);
            QEactivitymimeattachment_email_defra_application.EntityAlias = "application";

            // Add columns to Application Entity
            QEactivitymimeattachment_email_defra_application.Columns.AddColumns(Application.Name, Application.PermitNumber, Application.ApplicationNumber);

            // Add Application link-entity and define an alias.
            // Its an outer join as we want to return results even if not regarding an case
            var QEactivitymimeattachment_email_incident = QEactivitymimeattachment_email.AddLink(Case.EntityLogicalName,
                                                                                                 Email.RegardingObjectId,
                                                                                                 Case.IncidentId,
                                                                                                 JoinOperator.LeftOuter);
            QEactivitymimeattachment_email_incident.EntityAlias = "case";

            // Add columns to Case Entity
            QEactivitymimeattachment_email_incident.Columns.AddColumns(Case.Title, Case.IncidentId);

            // Add link-entity to Application Entity from Case and define an alias
            var QEactivitymimeattachment_email_incident_defra_application = QEactivitymimeattachment_email_incident.AddLink(Application.EntityLogicalName,
                                                                                                                            Case.Application, 
                                                                                                                            Application.ApplicationId,
                                                                                                                            JoinOperator.LeftOuter);
            QEactivitymimeattachment_email_incident_defra_application.EntityAlias = "case.application";

            // Add columns to Application Entity that we got via Case
            QEactivitymimeattachment_email_incident_defra_application.Columns.AddColumns(Application.ApplicationNumber, Application.Name, Application.PermitNumber);

            var results = Service.RetrieveMultiple(QEactivitymimeattachment);
            if (results != null && results.Entities.Count >= 1)
            {
                return results.Entities[0];
            }

            return null;
        }

        private Entity ReturnEmailData(Guid recordId)
        {
            var QEemail = new QueryExpression(Email.EntityLogicalName);
            QEemail.TopCount = 1;

            // Add columns for Email entity
            QEemail.ColumnSet.AddColumns(Email.Description, Email.Subject, Email.ActivityId, Email.StatusCode, Email.RegardingObjectId, Email.DirectionCode);

            // Define filter
            QEemail.Criteria.AddCondition(Email.ActivityId, ConditionOperator.Equal, recordId);

            // Add Application link-entity and define an alias.
            // Its an outer join as we want to return results even if not regarding an application
            var QEemail_defra_application = QEemail.AddLink(Application.EntityLogicalName, Email.RegardingObjectId, Application.ApplicationId, JoinOperator.LeftOuter);
            QEemail_defra_application.EntityAlias = "application";

            // Add columns for Application link entity
            QEemail_defra_application.Columns.AddColumns(Application.ApplicationNumber, Application.Name, Application.PermitNumber);

            // Add link-entity to Case and define an alias.
            // Its an outer join as we want to return results even if not regarding a case
            var QEemail_incident = QEemail.AddLink(Case.EntityLogicalName, Case.RegardingObjectId, Case.IncidentId, JoinOperator.LeftOuter);
            QEemail_incident.EntityAlias = "case";

            // Add columns for Case link entity
            QEemail_incident.Columns.AddColumns(Case.CaseType, Case.Title, Case.IncidentId);

            // Add link-entity from Case to Application and define an alias
            var QEemail_incident_defra_application = QEemail_incident.AddLink(Application.EntityLogicalName, Case.Application, Application.ApplicationId, JoinOperator.LeftOuter);
            QEemail_incident_defra_application.EntityAlias = "case.application";

            // Add columns to for Application entity regarding the Case
            QEemail_incident_defra_application.Columns.AddColumns(Application.ApplicationNumber, Application.Name, Application.PermitNumber);

            var results = Service.RetrieveMultiple(QEemail);
            if (results != null && results.Entities.Count >= 1)
            {
                return results.Entities[0];
            }

            return null;
        }

        private string SendRequest(string url, string requestContent)
        {
            TracingService.Trace(string.Format("Sending request to {0}", url));

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
            fileName = new Regex(@"\.(?!(\w{3,4}$))").Replace(fileName, "");
            var forbiddenChars = @"#%&*:<>?/{|}~".ToCharArray();
            fileName = new string(fileName.Where(c => !forbiddenChars.Contains(c)).ToArray());
            fileName = Regex.Replace(fileName, @"\s", "");
            if (fileName.Length >= 101)
            {
                fileName = fileName.Remove(100);
            }
            return fileName;
        }

        /// <summary>
        /// Issue SendFileToSharePoint Message which will trigger the SendSingleAttachmentToSharePoint Plugin.
        /// Need to process as a series of async plugin requests because we don't know how many attachments there might be
        /// so could exceed the limit of 2 minutes.
        /// </summary>
        /// <param name="entity"></param>
        internal void SendFileToSharePointActionRequest(string entityName, Guid id)
        {
            var actionRequest = new OrganizationRequest(PluginMessages.SendFileToSharePoint)
            {
                [PluginInputParams.TargetEntityName] = entityName,
                [PluginInputParams.TargetEntityId] = id.ToString()
            };
            var actionResponse = Service.Execute(actionRequest);
        }
    }
}