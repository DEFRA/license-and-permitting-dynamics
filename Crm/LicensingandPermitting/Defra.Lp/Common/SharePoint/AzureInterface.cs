using Core.Configuration;
using Lp.DataAccess;
using Microsoft.Xrm.Sdk;
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
            try
            {
                Config = adminService.GetConfigurationStringValues(
                     $"{SharePointSecureConfigurationKeys.ApplicationFolderContentType}",
                     $"{SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl}",
                     $"{SharePointSecureConfigurationKeys.MetadataLogicAppUrl}",
                     $"{SharePointSecureConfigurationKeys.PermitFolderContentType}",
                     $"{SharePointSecureConfigurationKeys.PermitListName}");

                // Try to access - error if we need to configure
                var appFolderContentType = Config[$"{SharePointSecureConfigurationKeys.ApplicationFolderContentType}"];
                var documentRelayLogicApp = Config[$"{SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl}"];
                var metadataLogicApp = Config[$"{SharePointSecureConfigurationKeys.MetadataLogicAppUrl}"];
                var permitFolderContentType = Config[$"{SharePointSecureConfigurationKeys.PermitFolderContentType}"];
                var permitListName = Config[$"{SharePointSecureConfigurationKeys.PermitListName}"];
            }
            catch (Exception exc)
            {
                throw new InvalidPluginExecutionException("The Sharepoint integration needs to be configured.");
            }
        }

        internal void CreateFolder(EntityReference application)
        {
            TracingService.Trace(string.Format("In CreateFolder with Entity Type {0} and Entity Id {1}", application.LogicalName, application.Id));

            var request = new DocumentRelayRequest();
            var applicationEntity = Query.RetrieveDataForEntityRef(Service, new string[] { "defra_name", "defra_permitnumber", "defra_applicationnumber" }, application);

            TracingService.Trace(string.Format("Permit Number = {0}; Application Number = {1}", applicationEntity["defra_permitnumber"].ToString(), applicationEntity["defra_applicationnumber"].ToString()));

            request.ApplicationContentType = Config[$"{SharePointSecureConfigurationKeys.ApplicationFolderContentType}"];
            request.ApplicationNo = applicationEntity.GetAttributeValue<string>("defra_applicationnumber").Replace('/', '_');
            request.FileBody = string.Empty;
            request.FileDescription = string.Empty;
            request.FileName = string.Empty;
            request.ListName = Config[$"{SharePointSecureConfigurationKeys.PermitListName}"];
            request.PermitContentType = Config[$"{SharePointSecureConfigurationKeys.PermitFolderContentType}"];
            request.PermitNo = applicationEntity.GetAttributeValue<string>("defra_permitnumber");
            request.Customer = string.Empty;
            request.SiteDetails = string.Empty;
            request.PermitDetails = string.Empty;

            var stringContent = JsonConvert.SerializeObject(request);

            TracingService.Trace(string.Format("Data Sent to Logic App URL {0}", Config[$"{SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl}"]));

            var resultBody = SendRequest(Config[$"{SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl}"], stringContent);
            //var data = JsonConvert.DeserializeObject<MoveSharePointResult>(resultBody);
        }

        internal void UploadFile(EntityReference recordIdentifier, string parentEntity, string parentLookup)
        {
            TracingService.Trace(string.Format("In UploadFile with Entity Type {0} and Entity Id {1}. Parent Entity: {2} Lookup Name: {3}", recordIdentifier.LogicalName, recordIdentifier.Id, parentEntity, parentLookup));
            
            var request = new DocumentRelayRequest();
            var activityId = Guid.Empty;
            Entity annotationData = null;
            Entity attachmentData = null;

            if (parentEntity == "email")
            {
                // This will have been fired against the creation of the Activity Mime Attachment Record
                attachmentData = ReturnAttachmentData(recordIdentifier.Id);
                if (attachmentData == null)
                {
                    throw new InvalidPluginExecutionException("No attachment data record returned from query");
                }
                else
                {
                    bool direction = (bool)(attachmentData.GetAttributeValue<AliasedValue>("email.directioncode")).Value;

                    OptionSetValue statusCode = (OptionSetValue)(attachmentData.GetAttributeValue<AliasedValue>("email.statuscode")).Value;
                    if (direction && statusCode.Value != 3)
                    {
                        //Outgoing email, do not send the attachment on create
                        TracingService.Trace("Aborted creating attachment for outgoing email attachment that is not sent");
                        return;
                    }
                }


                activityId = AddInsertFileParametersToRequest(request, attachmentData);

                //entityMetadataQuery = ReturnEmailMetadataQuery(activityId);

            }
            else
            {
                //This will have been fired against the creation of the Attachment Record
                annotationData = ReturnAnnotationData(recordIdentifier.Id, parentEntity, parentLookup);
                if (annotationData == null)
                {
                    throw new InvalidPluginExecutionException("No annotation data record returned from query");
                }
                else
                {
                    activityId = AddInsertFileParametersToRequest(request, annotationData);
                }
            }

            TracingService.Trace(string.Format("activityId {0}", activityId.ToString()));

            var stringContent = JsonConvert.SerializeObject(request);
            var logicAppUrl = Config[$"{SharePointSecureConfigurationKeys.DocumentRelayLogicAppUrl}"];
            TracingService.Trace(string.Format("Sending data to Logic App URL {0}", logicAppUrl));

            var resultBody = SendRequest(logicAppUrl, stringContent);
            //MoveSharePointResult data = JsonConvert.DeserializeObject<MoveSharePointResult>(resultBody);
            //if (data != null && !string.IsNullOrEmpty(data.SharePointId))
            if (resultBody!= null)
            {
                //TracingService.Trace(string.Format("SharePoint File Id {0}", data.SharePointId));
                if (annotationData != null)
                {
                    //Service.Delete(annotationData.LogicalName, annotationData.Id);
                    annotationData["documentbody"] = string.Empty;
                    Service.Update(annotationData);
                }
                else if (attachmentData != null)
                    TracingService.Trace("Need to delete attachment");
            }
        }

        internal void UpdateMetaData(EntityReference entity, string customer, string siteDetails, string permitDetails)
        {
            TracingService.Trace("In UpdateMetaData with Entity Type {0} and Entity Id {1}", entity.LogicalName, entity.Id.ToString());

            var request = new MetaDataRequest();

            if (entity.LogicalName == "defra_application")
            {
                var applicationEntity = Query.RetrieveDataForEntityRef(Service, new string[] { "defra_name", "defra_permitnumber", "defra_applicationnumber" }, entity);
                if (applicationEntity != null)
                {
                    TracingService.Trace(string.Format("Permit Number = {0}; Application Number = {1}", applicationEntity["defra_permitnumber"].ToString(), applicationEntity["defra_applicationnumber"].ToString()));

                    request.ApplicationNo = applicationEntity.GetAttributeValue<string>("defra_applicationnumber").Replace('/', '_');
                    request.ListName = Config[$"{SharePointSecureConfigurationKeys.PermitListName}"];
                    request.PermitNo = applicationEntity.GetAttributeValue<string>("defra_permitnumber");
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
            else if (entity.LogicalName == "defra_permit")
            {
                var permitEntity = Query.RetrieveDataForEntityRef(Service, new string[] { "defra_name", "defra_permitnumber", "defra_applicationnumber" }, entity);
                if (permitEntity != null)
                {
                    TracingService.Trace(string.Format("Permit Number = {0}; Application Number = {1}", permitEntity["defra_permitnumber"].ToString(), permitEntity["defra_applicationnumber"].ToString()));

                    request.ApplicationNo = string.Empty;
                    request.ListName = Config[$"{SharePointSecureConfigurationKeys.PermitListName}"];
                    request.PermitNo = permitEntity.GetAttributeValue<string>("defra_permitnumber");
                    request.Customer = string.Empty;
                    request.SiteDetails = string.Empty;
                    request.PermitDetails = string.Empty;
                    request.UpdateType = AzureInterfaceConstants.MetaDataPermitUpdateType;
                }
                else
                {
                    throw new InvalidPluginExecutionException(string.Format("No Permit exists for entity reference {0}", entity.Id.ToString()));
                }
            }

            var stringContent = JsonConvert.SerializeObject(request);

            TracingService.Trace(string.Format("Data Sent to Logic App URL {0}", Config[$"{SharePointSecureConfigurationKeys.MetadataLogicAppUrl}"]));

            var resultBody = SendRequest(Config[$"{SharePointSecureConfigurationKeys.MetadataLogicAppUrl}"], stringContent);
            //var data = JsonConvert.DeserializeObject<MoveSharePointResult>(resultBody);
        }

        private Guid AddInsertFileParametersToRequest(DocumentRelayRequest request, Entity queryRecord)
        {
            TracingService.Trace("In AddInsertFileParametersToRequest()");

            var permitNo = string.Empty;
            var applicationNo = string.Empty;
            if (queryRecord.Contains("parent.defra_permitnumber"))
            {
                permitNo = (string)((AliasedValue)queryRecord.Attributes["parent.defra_permitnumber"]).Value;
            }
            if (queryRecord.Contains("parent.defra_applicationnumber"))
            {
                applicationNo = (string)((AliasedValue)queryRecord.Attributes["parent.defra_applicationnumber"]).Value;
                applicationNo = applicationNo.Replace('/', '_');
            }

            TracingService.Trace("Permit No: {0}", permitNo);
            TracingService.Trace("Application No: {0}", applicationNo);

            var fileName = queryRecord.GetAttributeValue<string>("filename");

            TracingService.Trace("Filename: {0}", fileName);
            TracingService.Trace("Logical Name: {0}", queryRecord.LogicalName);

            var body = string.Empty;
            if (queryRecord.LogicalName == "activitymimeattachment")
            {
                fileName = SpRemoveIllegalChars(fileName);
                body = queryRecord.GetAttributeValue<string>("body");
            }
            else
            {
                body = queryRecord.GetAttributeValue<string>("documentbody");
            }

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

            TracingService.Trace(string.Format("Requests: {0}", request));

            return new Guid();
        }

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
            string fetchXml = string.Format(@"<fetch top='1' >
                                                          <entity name='activitymimeattachment' >
                                                             <attribute name='filename' />
                                                            <attribute name='body' />
                                                        <filter>
                                                        <condition attribute='activitymimeattachmentid' operator='eq' value='{0}' />
                                                        </filter>
                                                        <link-entity name='email' from='activityid' to='objectid' alias='email' link-type='inner' >
                                                              <attribute name='directioncode' />
                                                              <attribute name='activityid' />
                                                              <attribute name='statuscode' />
                                                              <attribute name='regardingobjectid' />
                                                              <link-entity name='defra_application' from='defra_applicationid' to='regardingobjectid' link-type='outer' alias='parent' >
                                                                <attribute name='defra_name' />
                                                                <attribute name='defra_permitnumber' />
                                                                <attribute name='defra_applicationnumber' />
                                                              </link-entity>
                                                            </link-entity>
                                                          </entity>
                                                        </fetch>", recordId);

            return Query.QueryCRMForSingleEntity(Service, fetchXml);
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
    }
}