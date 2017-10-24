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
    public enum AzureTarget
    {
        DocumentProcessorLogicApp = 0,
        MetadataAzureFunction = 1,
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

        internal void CreateFolder(EntityReference application)
        {
            TracingService.Trace(string.Format("In CreateFolder with Entity Type {0} and Entity Id {1}", application.LogicalName, application.Id));

            var request = new MoveFileRequest();
            var permitNo = string.Empty;

            var applicationEntity = RetrieveData(new string[] { "defra_name" }, application);

            TracingService.Trace(string.Format("Application Name = {0}", applicationEntity["defra_name"].ToString()));

            request.ContentTypeName = "Document Set LP1";
            request.ListName = "TestLibKal";
            request.PermitNo = applicationEntity["defra_name"].ToString();

            var stringContent = JsonConvert.SerializeObject(request);

            TracingService.Trace(string.Format("Data Sent to Logic App URL {0}", GetAzureLocationUrl(AzureTarget.DocumentProcessorLogicApp)));

            var resultBody = SendRequest(GetAzureLocationUrl(AzureTarget.DocumentProcessorLogicApp), stringContent);
            var data = JsonConvert.DeserializeObject<MoveSharePointResult>(resultBody);
            //CreateDocumentLocationRecord(data, activityId);
        }

        internal void MoveFile(EntityReference recordIdentifier, string parentEntity, string parentLookup)
        {
            TracingService.Trace(string.Format("In MoveFile with Entity Type {0} and Entity Id {1}. Parent Entity: {2} Lookup Name: {3}", recordIdentifier.LogicalName, recordIdentifier.Id, parentEntity, parentLookup));
            
            MoveFileRequest request = new MoveFileRequest();

            //string entityMetadataQuery = null;

            Guid activityId = Guid.Empty;

            string permitNo = string.Empty;
            if (parentEntity == "email")
            {
                //This will have been fired against the cration of the Activity Mime Attachment Record
                
            }
            else
            {
                //This will have been fired against the cration of the Activity Mime Attachment Record
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

                    // entityMetadataQuery = ReturnCustomerNotificationMetadataQuery(activityId, parentEntity);

                        
                    //request.Metadata = GenerateMetadata(annotationData, initialCreation, parentEntity);
                }

            }


            //TracingService.Trace(string.Format("Performing entity metadata query with query {0}", entityMetadataQuery));

            //Entity metadataQueryResult = Query.QueryCRMForSingleEntity(Service, entityMetadataQuery);

            //TracingService.Trace(string.Format("metadataQueryResult {0}", metadataQueryResult.Id.ToString()));
            //if (metadataQueryResult == null)
            //{
            //    throw new Exception("No result returned for entity metadata query");
            //}

            //if (!initialCreation)
            //{
                //activityId = AddMovementFileParametersToRequest(request, metadataQueryResult, parentEntity);
            //}
            TracingService.Trace(string.Format("activityId {0}", activityId.ToString()));

            //request.Metadata = GenerateMetadata(metadataQueryResult, initialCreation, parentEntity);

            string stringContent = JsonConvert.SerializeObject(request);


            // TracingService.Trace(string.Format("Data Sent to Logic App URL {0}", GetAzureLocationUrl(AzureTarget.DocumentProcessorLogicApp)));

            string resultBody = SendRequest(GetAzureLocationUrl(AzureTarget.DocumentProcessorLogicApp), stringContent);

            MoveSharePointResult data = JsonConvert.DeserializeObject<MoveSharePointResult>(resultBody);


            //if (initialCreation)
            //{
                //CreateActivityMetadataRecord(data, activityId);
                CreateDocumentLocationRecord(data, activityId);
            //}
            //else
            //{
                //UpdateActivityMetadataRecord(data, recordIdentifier.Id, activityId);
                //UpdateDocumentLocationRecord(data, recordIdentifier.Id, activityId);
            //}
        }

        private Guid AddInsertFileParametersToRequest(MoveFileRequest request, Entity queryRecord, string permitNo, string parentEntityName, string parentLookup)
        {
            TracingService.Trace(string.Format("In AddInsertFileParametersToRequest. PermitNo: {0}", permitNo));

            string fileName = queryRecord.GetAttributeValue<string>("filename");

            TracingService.Trace(string.Format("Filename: {0}", fileName));
            TracingService.Trace(string.Format("Logical Name: {0}", queryRecord.LogicalName));

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
            //request.AttachmentId = queryRecord.Id;
            request.ContentTypeName = "Document Set LP1";
            request.ListName = "TestLibKal";
            request.Operation = "CreateFile2";

            //if (queryRecord.Attributes.Contains("email.rpa_documenttype"))
            //{

            //    EntityReference documentType = (EntityReference)((AliasedValue)queryRecord.Attributes["email.rpa_documenttype"]).Value;
            //    request.DocumentId = documentType.Name;
            //}

            //return activityId;

            return new Guid();
        }

        private Entity ReturnAnnotationData(Guid recordId, string parentEntityName, string parentLookup)
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

        //private MetadataValues GenerateMetadata(Entity attachment, bool initialCreation, string parentEntity)
        //{
        //    TracingService.Trace("Start Generate Metadata");

        //    MetadataValues returnValue = new MetadataValues();

        //    returnValue.ListName = Query.GetConfigurationValue(AdminService, "SharePointListName");

        //    PropertyGenerator propertyGenerator = new PropertyGenerator(AdminService);

        //    propertyGenerator.AddProperty<string>(returnValue.Fields, "filename", "Title", attachment);

        //    if (!initialCreation)
        //    {
        //        // Activity Metadata fields 
        //        //    propertyGenerator.AddProperty<string>(returnValue.Fields, "rpa_filename", "Title", attachment);
        //        //    propertyGenerator.AddProperty<string>(returnValue.Fields, "rpa_envelopeid", "EnvelopeID", attachment);
        //        //    propertyGenerator.AddProperty<DateTime>(returnValue.Fields, "rpa_postreceiveddate", "Post_x0020_Received", attachment);
        //        //    propertyGenerator.AddProperty<string>(returnValue.Fields, "rpa_sourcesystem", "Source_x0020_System", attachment, true);
        //        //    propertyGenerator.AddProperty<bool>(returnValue.Fields, "rpa_sensitive", "Sensitive", attachment);
        //        //    propertyGenerator.AddProperty<DateTime>(returnValue.Fields, "rpa_scandate", "Scan_x0020_Date", attachment);
        //        //    propertyGenerator.AddProperty<string>(returnValue.Fields, "rpa_envelopeid", "EnvelopeID", attachment);
        //        //    propertyGenerator.AddProperty<string>(returnValue.Fields, "rpa_filesinenvelope", "File_x0020_In_x0020_Envelope", attachment);
        //        //Use full "AddProperty" function so that we can pass in the entity alias, so that the optionset text an be retrieved
        //        //    propertyGenerator.AddProperty<OptionSetValue>(returnValue.Fields, attachment.LogicalName, "rpa_securitymarking", true, "Security_x0020_Markings", attachment, true);

        //        // Document Type fields
        //        //    propertyGenerator.AddProperty<string>(returnValue.Fields, "rpa_documenttypes", "rpa_name", "Document_x0020_Ref", attachment);
        //        //    propertyGenerator.AddProperty<OptionSetValue>(returnValue.Fields, "rpa_documenttypes", "rpa_imageorproforma", "Submission_x0020_Type", attachment);
        //        //    propertyGenerator.AddProperty<string>(returnValue.Fields, "rpa_documenttypes", "rpa_documentcategory", "Doc_x0020_Category", attachment, true);

        //        // Business Unit fields
        //        //    propertyGenerator.AddProperty<string>(returnValue.Fields, "businessunit", "name", "Code_x0020_Words", attachment, true);
        //    }


        //    //string activityId = Query.GetFieldValueAsString(attachment, "activityid");
        //    //string url = Query.GetConfigurationValue(AdminService, "CRMServerUrl") + "?etn=" + parentEntity + "& pagetype=entityrecord&id=" + activityId;
        //    //propertyGenerator.AddProperty<string>(returnValue.Fields, "Activity_x0020_Url", url, false);


        //    //Email fields
        //    //propertyGenerator.AddProperty<string>(returnValue.Fields, "email", "subject", initialCreation, "Activity_x0020_Description", attachment);
        //    //propertyGenerator.AddProperty<Guid>(returnValue.Fields, "email", "activityid", initialCreation, "Activity_x0020_Id", attachment);
        //    //propertyGenerator.AddProperty<DateTime>(returnValue.Fields, "email", "createdon", initialCreation, "Activity_x0020_Date", attachment);
        //    ////    propertyGenerator.AddProperty<bool>(returnValue.Fields, "email", "directioncode", initialCreation, "Direction", attachment);

        //    // Case fields
        //    //propertyGenerator.AddProperty<string>(returnValue.Fields, "case", "ticketnumber", "Case_x0020_Number", attachment);

        //    // Organisation fields
        //    //propertyGenerator.AddProperty<string>(returnValue.Fields, "account", "rpa_sbinumber", "Business_x002d_ID", attachment);
        //    //propertyGenerator.AddProperty<string>(returnValue.Fields, "account", "rpa_sbinumber", "SBI", attachment);
        //    //propertyGenerator.AddProperty<string>(returnValue.Fields, "account", "name", "CRM_x0020_Organisation", attachment);
        //    //propertyGenerator.AddProperty<string>(returnValue.Fields, "account", "rpa_mhsnumber", "MHS_x0020_Number", attachment);
        //    //propertyGenerator.AddProperty<string>(returnValue.Fields, "account", "rpa_capfirmid", "FRN", attachment);

        //    // Contact fields
        //    //propertyGenerator.AddProperty<string>(returnValue.Fields, "contact", "fullname", "CRM_x0020_Contact", attachment);
        //    //propertyGenerator.AddProperty<string>(returnValue.Fields, "contact", "rpa_pinumber", "Personal_x002d_ID", attachment);
        //    //propertyGenerator.AddProperty<string>(returnValue.Fields, "contact", "rpa_capcustomerid", "CRN", attachment);

        //    TracingService.Trace("End Generate Metadata");

        //    return returnValue;
        //}

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
            string configName = string.Empty;

            switch (azureTarget)
            {
                case AzureTarget.DocumentProcessorLogicApp:
                    configName = "DocumentProcessorLogicAppUrl";
                    break;
                case AzureTarget.MetadataAzureFunction:
                    configName = "MetadataAzureFunctionUrl";
                    break;
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

        private void CreateDocumentLocationRecord(MoveSharePointResult result, Guid activityId)
        {
            TracingService.Trace("RESPONSE: Start of Document Location Creation");

            Entity DocumentLocation = new Entity();
            DocumentLocation.LogicalName = "sharepointdocumentlocation";

            

            //AMD.Attributes["rpa_filename"] = result.FileName;
            //AMD.Attributes["rpa_fileabsoluteurl"] = result.FileURL;
            //AMD.Attributes["rpa_docsubcategoryproxy"] = result.AttachmentType + "," + result.AttachmentId;

            //if (result.SharePointId != null && result.SharePointId != "")
            //{
            //    AMD.Attributes["rpa_sharepointfileid"] = result.SharePointId;
            //}

            //if (result.DocumentId != null && result.DocumentId != "")
            //{
            //    AMD.Attributes["rpa_docrefproxy"] = result.DocumentId;
            //}

            //if (result.ActivityName == "email")
            //{
            //    AMD.Attributes["rpa_relatedmetadataid"] = new EntityReference("email", activityId);
            //}
            //else if (result.ActivityName == "rpa_letter")
            //{
            //    AMD.Attributes["rpa_relatedwhitemailid"] = new EntityReference("rpa_letter", activityId);
            //}
            //else if (result.ActivityName == "rpa_customernotification")
            //{
            //    AMD.Attributes["rpa_customernotificationid"] = new EntityReference("rpa_customernotification", activityId);
            //    AMD.Attributes["rpa_direction"] = true;

            //}
            //else if (result.ActivityName == "rpa_memo")
            //{
            //    AMD.Attributes["rpa_internalnoteid"] = new EntityReference("rpa_memo", activityId);
            //}

            Service.Create(DocumentLocation);
            TracingService.Trace("sharepointdocumentlocation created for " + result.ActivityName);
        }

        private Entity RetrieveData(string[] columns, EntityReference target)
        {
            var retrieveRequest = new RetrieveRequest();
            retrieveRequest.ColumnSet = new ColumnSet(columns);
            retrieveRequest.Target = target;
            var retrieveResponse = (RetrieveResponse)Service.Execute(retrieveRequest);
            var entity = retrieveResponse.Entity as Entity;
            return entity;
        }
    }
}