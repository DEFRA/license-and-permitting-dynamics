using Defra.Lp.Common.ProxyClasses;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Defra.Lp.Common
{
    public enum AzureTarget : int
    {
        DocumentProcessorLogicApp = 0,
        //MetadataAzureFunction = 1,
        //AddUserToSharePointGroupAzureFunction = 2,
        //RemoveUserFromSharePointGroupAzureFunction = 3
    }

    internal class AzureInterface
    {
        private IOrganizationService AdminService { get; set; }
        private IOrganizationService Service { get; set; }
        private ITracingService TracingService { get; set; }

        internal AzureInterface(IOrganizationService adminService, IOrganizationService service, ITracingService tracingService)
        {
            AdminService = adminService;
            Service = service;
            TracingService = tracingService;
        }

        internal void MoveFile(EntityReference recordIdentifier, bool initialCreation, string parentEntity, string parentLookup)
        {
            TracingService.Trace(string.Format("In MoveFile with Entity Type {0} and Entity Id {1}. Initial Creation: {2} Parent Entity: {3} Lookup Name: {4}", recordIdentifier.LogicalName, recordIdentifier.Id, initialCreation, parentEntity, parentLookup));
            
            MoveFileRequest request = new MoveFileRequest();

            string entityMetadataQuery = null;

            Guid activityId = Guid.Empty;

            if (initialCreation)
            {
                string caseNo = string.Empty;
                if (parentEntity == "email")
                {
                    //This will have been fired against the cration of the Activity Mime Attachment Record
                    //Entity attachmentData = ReturnAttachmentData(recordIdentifier.Id);
                    //if (attachmentData == null)
                    //{
                    //    throw new InvalidPluginExecutionException("No attachment data record returned from query");
                    //}
                    //else
                    //{

                    //    bool direction = (bool)(attachmentData.GetAttributeValue<AliasedValue>("email.directioncode")).Value;

                    //    OptionSetValue statusCode = (OptionSetValue)(attachmentData.GetAttributeValue<AliasedValue>("email.statuscode")).Value;
                    //    if (direction && statusCode.Value != 3)
                    //    {
                    //        //Outgoing email, do not send the attachment on create
                    //        TracingService.Trace("Aborted creating attachment for outgoing email attachment that is not sent");
                    //        return;
                    //    }
                    //}
                    //if (attachmentData.Contains("case.ticketnumber"))
                    //{
                    //    caseNo = (string)((AliasedValue)attachmentData.Attributes["case.ticketnumber"]).Value;
                    //}

                    //activityId = AddInsertFileParametersToRequest(request, attachmentData, caseNo, parentEntity);

                    //entityMetadataQuery = ReturnEmailMetadataQuery(activityId);
                }
                else
                {

                    //This will have been fired against the cration of the Activity Mime Attachment Record
                    Entity annotationData = ReturnAnnotationData(recordIdentifier.Id, parentEntity);
                    if (annotationData == null)
                    {
                        throw new InvalidPluginExecutionException("No annotation data record returned from query");
                    }
                    else
                    {
                        if (annotationData.Contains("case.ticketnumber"))
                        {
                            caseNo = (string)((AliasedValue)annotationData.Attributes["case.ticketnumber"]).Value;
                        }

                        activityId = AddInsertFileParametersToRequest(request, annotationData, caseNo, parentEntity);

                       // entityMetadataQuery = ReturnCustomerNotificationMetadataQuery(activityId, parentEntity);

                    }

                }

            }
            else
            {
                //This will have been fired against the cration of the Activity Metadata Record
                //entityMetadataQuery = ReturnAMDQuery(recordIdentifier.Id, parentEntity, parentLookup);
            }

            //TracingService.Trace(string.Format("Performing entity metadata query with query {0}", entityMetadataQuery));

            Entity metadataQueryResult = Query.QueryCRMForSingleEntity(Service, entityMetadataQuery);

            TracingService.Trace(string.Format("metadataQueryResult {0}", metadataQueryResult.Id.ToString()));
            if (metadataQueryResult == null)
            {
                throw new Exception("No result returned for entity metadata query");
            }

            if (!initialCreation)
            {
                //activityId = AddMovementFileParametersToRequest(request, metadataQueryResult, parentEntity);
            }
            TracingService.Trace(string.Format("activityId {0}", activityId.ToString()));
            //request.Metadata = GenerateMetadata(metadataQueryResult, initialCreation, parentEntity);

            string stringContent = JsonConvert.SerializeObject(request);


            // TracingService.Trace(string.Format("Data Sent to Logic App URL {0}", GetAzureLocationUrl(AzureTarget.DocumentProcessorLogicApp)));

            string resultBody = SendRequest(GetAzureLocationUrl(AzureTarget.DocumentProcessorLogicApp), stringContent);

            MoveSharePointResult data = JsonConvert.DeserializeObject<MoveSharePointResult>(resultBody);


            if (initialCreation)
            {
                //CreateActivityMetadataRecord(data, activityId);
            }
            else
            {
                //UpdateActivityMetadataRecord(data, recordIdentifier.Id, activityId);
            }
        }

        private Guid AddInsertFileParametersToRequest(MoveFileRequest request, Entity queryRecord, string caseNo, string parentEntityName)
        {
            TracingService.Trace(string.Format("In AddInsertFileParametersToRequest. CaseNo: {0}", caseNo));

            string fileName = queryRecord.GetAttributeValue<string>("filename");

            string body = string.Empty;
            if (queryRecord.LogicalName == "activitymimeattachment")
            {

                fileName = new Regex(@"\.(?!(\w{3,4}$))").Replace(fileName, "");
                var forbiddenChars = @"#%&*:<>?/{|}~".ToCharArray();
                fileName = new string(fileName.Where(c => !forbiddenChars.Contains(c)).ToArray());
                fileName = Regex.Replace(fileName, @"\s", "");
                if (fileName.Length >= 101)
                {
                    fileName = fileName.Remove(100);
                }

                body = queryRecord.GetAttributeValue<string>("body");
            }
            else
            {
                body = queryRecord.GetAttributeValue<string>("documentbody");
            }

            request.FileBody = body;

            if (!string.IsNullOrEmpty(caseNo))
            {
                request.CaseNo = caseNo;
            }

            AliasedValue activityIdAliasedValue = queryRecord.GetAttributeValue<AliasedValue>("email.activityid");
            Guid activityId = (Guid)activityIdAliasedValue.Value;
            request.ActivityId = activityId.ToString();
            request.ActivityName = parentEntityName;
            request.AttachmentType = queryRecord.LogicalName;
            request.FileName = fileName;
            request.AttachmentId = queryRecord.Id;

            if (queryRecord.Attributes.Contains("email.rpa_documenttype"))
            {

                EntityReference documentType = (EntityReference)((AliasedValue)queryRecord.Attributes["email.rpa_documenttype"]).Value;
                request.DocumentId = documentType.Name;
            }

            return activityId;
        }

        private Entity ReturnAnnotationData(Guid recordId, string parentEntityName)
        {
            string fetchXml = string.Format(@"<fetch top='1' >
                                                          <entity name='annotation' >
                                                            <attribute name='subject' />
                                                            <attribute name='documentbody' />
                                                            <attribute name='filename' />
                                                            <attribute name='annotationid' />
                                                            <order attribute='subject' descending='false' />
                                                            <filter type='and' >
                                                              <condition attribute='annotationid' operator='eq' uitype='annotation' value='{0}' />
                                                            </filter>
                                                            <link-entity name='{1}' from='activityid' to='objectid' alias='email' link-type='inner' >
                                                              <attribute name='activityid' />
                                                              <attribute name='statuscode' />
                                                              <attribute name='rpa_documenttype' />
                                                              <attribute name='regardingobjectid' />
                                                              <filter type='and' >
                                                                <condition attribute='statuscode' operator='not-null' />
                                                              </filter>
                                                              <link-entity name='incident' from='incidentid' to='regardingobjectid' alias='case' link-type='outer' >
                                                                <attribute name='ticketnumber' />
                                                                <filter type='and' >
                                                                  <condition attribute='ticketnumber' operator='not-null' />
                                                                </filter>
                                                              </link-entity>
                                                            </link-entity>
                                                          </entity>
                                                        </fetch>", recordId, parentEntityName);

            return Query.QueryCRMForSingleEntity(Service, fetchXml);
        }

        private string SendRequest(string url, string requestContent)
        {
            TracingService.Trace(string.Format("Sending request to {0}", url));

            using (HttpClient httpclient = new HttpClient())
            {
                HttpRequestMessage httpRequest = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Post,
                    Content = new StringContent(requestContent, Encoding.UTF8, "application/json"),
                };

                HttpResponseMessage response = httpclient.SendAsync(httpRequest).Result;
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        private string GetAzureLocationUrl(AzureTarget azureTarget)
        {
            string configName = "";

            switch (azureTarget)
            {
                case AzureTarget.DocumentProcessorLogicApp:
                    configName = "DocumentProcessorLogicAppUrl";
                    break;
                //case AzureTarget.MetadataAzureFunction:
                //    configName = "MetadataAzureFunctionUrl";
                //    break;
                //case AzureTarget.AddUserToSharePointGroupAzureFunction:
                //    configName = "AddUserToSharePointGroupAzureFunctionUrl";
                //    break;
                //case AzureTarget.RemoveUserFromSharePointGroupAzureFunction:
                //    configName = "RemoveUserFromSharePointGroupAzureFunctionUrl";
                //    break;
                default:
                    throw new Exception("Invalid Azure Target Specified");

            }

            return Query.GetConfigurationValue(AdminService, configName);
        }
    }
}