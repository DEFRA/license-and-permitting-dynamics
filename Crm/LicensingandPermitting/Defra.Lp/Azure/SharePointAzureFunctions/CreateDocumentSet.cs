using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.DocumentSet;
using Microsoft.SharePoint.Client.Taxonomy;
using Newtonsoft.Json;

namespace Defra.Lp.SharePointAzureFunctions
{
    public static class CreateDocumentSet


    {
        [FunctionName("CreateDocumentSet")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"Webhook was triggered!");

            try
            {
                var jsonContent = await req.Content.ReadAsStringAsync();
                log.Info(jsonContent);
                dynamic data = JsonConvert.DeserializeObject(jsonContent);

                // TODO: Rather than using web application settings, we should be using the Azure Key Vault for
                // credentials  

                var connectionString = WebUtility.UrlDecode(WebUtility.UrlDecode(data.SpoSiteName.ToString()));
                var documentSetUrl = string.Empty;
                var applicationFolder = data.ApplicationFolder.ToString();
                var permitFolderName = data.PermitFolder.ToString();

                using (ClientContext clientContext = new ClientContext(connectionString))
                {
                   
                    var dummy = new TaxonomyItem(clientContext, null);

                    var username = ConfigurationManager.ConnectionStrings["UserName"].ConnectionString;
                    var password = ConfigurationManager.ConnectionStrings["Password"].ConnectionString;

                    var securePassword = new SecureString();
                    foreach (char p in password)
                    {
                        securePassword.AppendChar(p);
                    }

                    var credentials = new SharePointOnlineCredentials(username, securePassword);
                    clientContext.Credentials = credentials;

                    log.Info("Got client context and set credentials");
                    log.Info(string.Format("ListName is {0}", data.ListName.ToString()));

                    var list = clientContext.Web.Lists.GetByTitle(data.ListName.ToString());
                    var rootFolder = list.RootFolder;
                    var permitFolderUrl = String.Format("{0}/{1}", data.ListName.ToString(), permitFolderName);
                    var applicationFolderUrl = String.Format("{0}/{1}", permitFolderUrl, applicationFolder);

                    // Get the Permit folder content type
                    var ctPermit = GetByName(list.ContentTypes, data.PermitContentType.ToString());
                    log.Info(string.Format("Permit Content Type Id is {0}", ctPermit.Id.StringValue));

                    // Create permit sub folder inside list root folder if it doesn't exist
                    var permitFolder = CreateSubFolderIfDoesntExist(clientContext, permitFolderName, rootFolder, ctPermit, data.PermitFolder.ToString(),log, data.ListName.ToString());
                    log.Info(string.Format("Folder is {0}", permitFolder.Name));

                   

                    // Get the Application document set content type
                    var ctApplication = GetByName(list.ContentTypes, data.ApplicationContentType.ToString());
                    log.Info(string.Format("Applicaction Content Type Id is {0}", ctApplication.Id.StringValue));

                    // Create the Document Set
                    Folder f;

                    try
                    {
                        var ds = DocumentSet.Create(clientContext, permitFolder, applicationFolder, ctApplication.Id);
                        clientContext.ExecuteQuery();
                        documentSetUrl = ds.Value;
                        log.Info(string.Format("Document Set Id is {0}", documentSetUrl));
                                               
                    }
                    catch (ServerException ex) when (ex.Message.StartsWith("A document, folder or document set with the name") && ex.Message.EndsWith("already exists."))
                    {
                        documentSetUrl = "Document set exists already";
                        log.Info(string.Format("Handling {0} - {1}", ex.Source, ex.Message));
                    }

                    // Create Complince folder
                    try
                    {
                        log.Info("try to create Compliance folder...");
                        var complinceFolder = DocumentSet.Create(clientContext, permitFolder, "Compliance", ctPermit.Id);
                        clientContext.ExecuteQuery();
                        log.Info("Compliance folder created");

                    }
                    catch (ServerException ex) when (ex.Message.StartsWith("A document, folder or document set with the name") && ex.Message.EndsWith("already exists."))
                    {
                        documentSetUrl = "Document set exists already";
                        log.Info(string.Format("Handling {0} - {1}", ex.Source, ex.Message));
                    }
                  
                }

                return req.CreateResponse(HttpStatusCode.OK, "{ \"DocumentSetUrl\" : \"" + documentSetUrl + "\" }");
            }
            catch (Exception ex)
            {
                log.Info(string.Format("{0} Exception {1}", ex.Source, ex.ToString()));
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Critial error creating SharePoint DocumentSet: " + ex.Message);
            }
        }

        private static ContentType GetByName(ContentTypeCollection cts, string name)
        {
            var ctx = cts.Context;
            ctx.Load(cts);
            ctx.ExecuteQuery();

            return Enumerable.FirstOrDefault(cts, ct => ct.Name == name);
        }

        private static Folder CreateSubFolderIfDoesntExist(ClientContext clientContext, string subFolder, Folder folder, ContentType ct, string permitId, TraceWriter log,string parentName)
        {
            
            var web = clientContext.Web;

            log.Info("Try to GetFolderByServerRelativeUrl");
            var result = web.GetFolderByServerRelativeUrl($"{parentName}/{subFolder}");
            clientContext.Load(result);

            try
            {
                clientContext.ExecuteQuery();
            }
            catch(Exception ex) when (ex.Message.Contains("File Not Found."))
            {

                log.Info($"Folder with name {subFolder} dose not exit. Inside catch");
                // Create the folder

                var newFolder = folder.Folders.Add(subFolder);

                // Set the content type 
                newFolder.ListItemAllFields.ParseAndSetFieldValue("ContentTypeId", ct.Id.ToString());
                newFolder.ListItemAllFields.ParseAndSetFieldValue("Permit_x0020_ID", permitId);
                newFolder.ListItemAllFields.Update();
                clientContext.Load(newFolder);
                log.Info($"Try to ExecuteQuery to create {subFolder}");
                clientContext.ExecuteQuery();
                log.Info("Created");
                return newFolder;
            }


            return result;
            

        }
    }
}
