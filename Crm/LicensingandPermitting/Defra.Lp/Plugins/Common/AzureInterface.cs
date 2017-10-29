using Defra.Lp.Common.ProxyClasses;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Defra.Lp.Common
{
    internal class AzureInterface
    {
        private Entity Config { get; set; }
        private IOrganizationService Service { get; set; }
        private ITracingService TracingService { get; set; }

        internal AzureInterface(EntityReference config, IOrganizationService service, ITracingService tracingService)
        {
            Service = service;
            TracingService = tracingService;
            Config = Query.RetrieveDataForEntityRef(Service, new string[] { ConfigNames.SharePointLogicAppUrl,
                                                                ConfigNames.SharePointPermitList,
                                                                ConfigNames.AddressbaseFacadeUrl,
                                                                ConfigNames.SharePointFolderContentType}, config);
        }

        internal AzureInterface(Entity config, IOrganizationService service, ITracingService tracingService)
        {
            Config = config;
            Service = service;
            TracingService = tracingService;
        }

        internal void CreateFolder(EntityReference application)
        {
            TracingService.Trace(string.Format("In CreateFolder with Entity Type {0} and Entity Id {1}", application.LogicalName, application.Id));

            var request = new MoveFileRequest();
            var permitNo = string.Empty;

            var applicationEntity = Query.RetrieveDataForEntityRef(Service, new string[] { "defra_name" }, application);

            TracingService.Trace(string.Format("Application Name = {0}", applicationEntity["defra_name"].ToString()));

            request.ContentTypeName = Config.GetAttributeValue<string>(ConfigNames.SharePointFolderContentType);
            request.ListName = "TestLibKal";
            request.PermitNo = applicationEntity.GetAttributeValue<string>("defra_name");

            var stringContent = JsonConvert.SerializeObject(request);

            TracingService.Trace(string.Format("Data Sent to Logic App URL {0}", Config[ConfigNames.SharePointLogicAppUrl].ToString()));

            var resultBody = SendRequest(Config[ConfigNames.SharePointLogicAppUrl].ToString(), stringContent);
            var data = JsonConvert.DeserializeObject<MoveSharePointResult>(resultBody);
        }

        internal void MoveFile(EntityReference recordIdentifier, string parentEntity, string parentLookup)
        {
            TracingService.Trace(string.Format("In MoveFile with Entity Type {0} and Entity Id {1}. Parent Entity: {2} Lookup Name: {3}", recordIdentifier.LogicalName, recordIdentifier.Id, parentEntity, parentLookup));
            
            var request = new MoveFileRequest();
            var activityId = Guid.Empty;
            var permitNo = string.Empty;

            if (parentEntity == "email")
            {
                // This will have been fired against the creation of the Activity Mime Attachment Record
                
            }
            else
            {
                //This will have been fired against the creation of the Attachment Record
                Entity annotationData = ReturnAnnotationData(recordIdentifier.Id, parentEntity, parentLookup);
                if (annotationData == null)
                {
                    throw new InvalidPluginExecutionException("No annotation data record returned from query");
                }
                else
                {
                    if (annotationData.Contains("parent.defra_name"))
                    {
                        permitNo = (string)((AliasedValue)annotationData.Attributes["parent.defra_name"]).Value;
                    }
                    activityId = AddInsertFileParametersToRequest(request, annotationData, permitNo, parentEntity, parentLookup);
                }
            }

            TracingService.Trace(string.Format("activityId {0}", activityId.ToString()));

            var stringContent = JsonConvert.SerializeObject(request);
            var logicAppUrl = Config.GetAttributeValue<string>(ConfigNames.SharePointLogicAppUrl);
            TracingService.Trace(string.Format("Sending data to Logic App URL {0}", logicAppUrl));

            var resultBody = SendRequest(logicAppUrl, stringContent);

            MoveSharePointResult data = JsonConvert.DeserializeObject<MoveSharePointResult>(resultBody);
        }

        private Guid AddInsertFileParametersToRequest(MoveFileRequest request, Entity queryRecord, string permitNo, string parentEntityName, string parentLookup)
        {
            TracingService.Trace(string.Format("In AddInsertFileParametersToRequest. PermitNo: {0}", permitNo));

            var fileName = queryRecord.GetAttributeValue<string>("filename");

            TracingService.Trace(string.Format("Filename: {0}", fileName));
            TracingService.Trace(string.Format("Logical Name: {0}", queryRecord.LogicalName));

            var body = string.Empty;
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

            TracingService.Trace(string.Format("Requests: {0}", request));

            request.FileBody = body;

            if (!string.IsNullOrEmpty(permitNo))
            {
                request.PermitNo = permitNo;
            }

            //AliasedValue activityIdAliasedValue = queryRecord.GetAttributeValue<AliasedValue>("parent." + parentLookup);
            //Guid activityId = (Guid)activityIdAliasedValue.Value;
            //request.ActivityId = activityId.ToString();
            request.ActivityName = parentEntityName;
            //request.AttachmentType = queryRecord.LogicalName;
            request.FileName = fileName;
            request.FileDescription = queryRecord.GetAttributeValue<string>("subject");
            //request.AttachmentId = queryRecord.Id;
            request.ContentTypeName = Config.GetAttributeValue<string>(ConfigNames.SharePointFolderContentType);
            request.ListName = "TestLibKal";  // ToDo: This needs to come from the Config
            request.Operation = string.Empty;

            //if (queryRecord.Attributes.Contains("email.rpa_documenttype"))
            //{

            //    EntityReference documentType = (EntityReference)((AliasedValue)queryRecord.Attributes["email.rpa_documenttype"]).Value;
            //    request.DocumentId = documentType.Name;
            //}

            //return activityId;

            return new Guid();
        }

        private string ReturnDocumentSetName(Entity config)
        {
            var documentSetName = string.Empty;
            var documentLocationRef = Config.GetAttributeValue<EntityReference>(ConfigNames.SharePointPermitList);
            var entity = Query.RetrieveDataForEntityRef(Service, new string[] { "name" }, documentLocationRef);
            if (entity != null)
            {
                documentSetName = entity.GetAttributeValue<string>("name");
            }
            return documentSetName;
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
                                                <attribute name='statuscode' />
                                                <filter type='and' >
                                                <condition attribute='statuscode' operator='not-null' />
                                                </filter>
                                            </link-entity>
                                            </entity>
                                        </fetch>", recordId, parentEntityName, parentLookup);

            return Query.QueryCRMForSingleEntity(Service, fetchXml);
        }

        //private MetadataValues GenerateFileMetadata(Entity attachment)
        //{
        //    TracingService.Trace("Start Generate Metadata");

        //    MetadataValues returnValue = new MetadataValues();
        //    returnValue.ListName = ConfigNames.SharePointPermitList;

        //    PropertyGenerator propertyGenerator = new PropertyGenerator();
        //    propertyGenerator.AddProperty<string>(returnValue.Fields, "filename", "Title", attachment);
        //    propertyGenerator.AddProperty<string>(returnValue.Fields, "subject", "Description", attachment);

        //    TracingService.Trace("End Generate Metadata");

        //    return returnValue;
        //}

        private string SendRequest(string url, string requestContent)
        {
            TracingService.Trace(string.Format("Sending request to {0}", url));

            using (HttpClient httpclient = new HttpClient())
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
    }
}