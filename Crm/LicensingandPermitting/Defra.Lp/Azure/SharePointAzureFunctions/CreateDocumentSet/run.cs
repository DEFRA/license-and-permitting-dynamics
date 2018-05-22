using Microsoft.Azure.WebJobs.Host;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.DocumentSet;
using Microsoft.SharePoint.Client.Taxonomy;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;

namespace Defra.Lp.SharePointAzureFunctions
{
    public class CreateDocumentSet
    {
        public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"Webhook was triggered!");

            try
            {
                var jsonContent = await req.Content.ReadAsStringAsync();
                log.Info(jsonContent);
                dynamic data = JsonConvert.DeserializeObject(jsonContent);

                // TODO: Rather than using web application settings, we should be using the Azure Key Vault for
                // credentials  

                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SharePointUrl"].ConnectionString;
                var documentSetUrl = string.Empty;
                var dsName = data.DocumentSetName.ToString();
                var folderPath = data.FolderPath.ToString();

                using (ClientContext clientContext = new ClientContext(connectionString))
                {
                    var dummy = new TaxonomyItem(clientContext, null);

                    var username = System.Configuration.ConfigurationManager.ConnectionStrings["UserName"].ConnectionString;
                    var password = System.Configuration.ConfigurationManager.ConnectionStrings["Password"].ConnectionString;

                    var securePassword = new SecureString();
                    foreach (char p in password)
                    {
                        securePassword.AppendChar(p);
                    }

                    var credentials = new SharePointOnlineCredentials(username, securePassword);
                    clientContext.Credentials = credentials;

                    var list = clientContext.Web.Lists.GetByTitle(data.ListName.ToString());
                    //var parentFolder = list.RootFolder;
                    var folderUrl = String.Format("{0}/{1}", data.ListName.ToString(), folderPath);
                    //var folders = list.RootFolder.Folders;
                    //clientContext.Load(folders, fldrs => fldrs.Include(fldr => fldr.ServerRelativeUrl));
                    //clientContext.ExecuteQuery();
                    //var parentFolder = folders.FirstOrDefault(f => f.ServerRelativeUrl.ToLower() == folderUrl.ToLower());
                    //var web = clientContext.Web;
                    var parentFolder = clientContext.Web.GetFolderByServerRelativeUrl(folderUrl);

                    log.Info(string.Format("ListName is {0}", data.ListName.ToString()));
                    log.Info(string.Format("Folder is {0}", parentFolder));
                    // Gets the document set content type
                    //var ct = clientContext.Web.ContentTypes.GetById("0x0120D520000AB59DAA21D4E4479F41140BE556886A007DD3406F677CDC4DB3C2054F130F37CB");
                    //clientContext.Load(ct);
                    //clientContext.ExecuteQuery();
                    var ct = GetByName(list.ContentTypes, data.ContentTypeName.ToString());

                    log.Info(string.Format("Content Type Id is {0}", ct.Id.StringValue));

                    // Creates the Document Set
                    try
                    {
                        var ds = DocumentSet.Create(clientContext, parentFolder, dsName, ct.Id);
                        clientContext.ExecuteQuery();
                        documentSetUrl = ds.Value;
                        log.Info(string.Format("Document Set Id is {0}", documentSetUrl));
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

        public static ContentType GetByName(ContentTypeCollection cts, string name)
        {
            var ctx = cts.Context;
            ctx.Load(cts);
            ctx.ExecuteQuery();

            return Enumerable.FirstOrDefault(cts, ct => ct.Name == name);
        }
    }
}