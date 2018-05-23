using Microsoft.Azure.WebJobs.Host;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;

namespace Defra.Lp.SharePointAzureFunctions
{
    public class UpdateMetaData
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
                var fileId = -1;

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

                    Microsoft.SharePoint.Client.File file = GetSharePointFile(clientContext, (string)data.RecordURL, log);

                    var listItem = file.ListItemAllFields;
                    clientContext.Load(file, c => c.ListItemAllFields.Id);
                    clientContext.ExecuteQuery();
                    fileId = listItem.Id;

                    foreach (dynamic metadataItem in data.Metadata.Fields)
                    {
                        log.Info($"Update field: " + metadataItem.Name);

                        if (metadataItem.Value == null)
                        {
                            listItem[(string)metadataItem.Name] = null;
                        }
                        else if ((bool)metadataItem.ManagedMetadataType)
                        {
                            //updateMetaDataField(clientContext, (string)data.Metadata.ListName, file, listItem, (string)metadataItem.Name, (string)metadataItem.Value, log);
                        }
                        else
                        {
                            listItem[(string)metadataItem.Name] = (string)metadataItem.Value;
                        }

                        listItem.Update();
                    }
                    clientContext.Load(listItem);
                    clientContext.ExecuteQuery();
                }

                return req.CreateResponse(HttpStatusCode.OK, "{ \"SharePointId\" : \"" + fileId + "\" }");
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Critial error updating SharePoint metadata: " + ex.Message);
            }
        }

        public static Microsoft.SharePoint.Client.File GetSharePointFile(ClientContext clientContext, string fileUrl, TraceWriter log)
        {
            log.Info($"Get SharePoint File:" + fileUrl);

            File sourceFile = clientContext.Web.GetFileByServerRelativeUrl(fileUrl);
            clientContext.Load(sourceFile);
            clientContext.ExecuteQuery();

            log.Info($"File Loaded");

            return sourceFile;
        }
    }
}