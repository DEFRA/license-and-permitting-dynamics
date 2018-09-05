// <copyright file="ProcessAttachmentsOnEmailSend.cs" company="">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author></author>
// <date>11/27/2017 7:50:15 AM</date>
// <summary>Implements the UploadEmailAndAttachmentsToSharePoint Code Activity.</summary>
using Defra.Lp.Common.SharePoint;
using Lp.DataAccess;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Model.Lp.Crm;
using System;
using System.Activities;

namespace Defra.Lp.Workflows
{
    /// <summary>
    /// This code activity is used to upload an email and its attachments to SharePoint
    /// when the email is marked as sent. 
    /// </summary>
    public class UploadEmailAndAttachmentsToSharePoint : CodeActivity
    {
        private ITracingService TracingService { get; set; }
        private IWorkflowContext Context { get; set; }
        private IOrganizationServiceFactory ServiceFactory { get; set; }
        private IOrganizationService Service { get; set; }
        private IOrganizationService AdminService { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            TracingService = executionContext.GetExtension<ITracingService>();
            Context = executionContext.GetExtension<IWorkflowContext>();
            ServiceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            Service = ServiceFactory.CreateOrganizationService(Context.UserId);
            AdminService = ServiceFactory.CreateOrganizationService(null);

            ProcessAttachments();
        }

        private void ProcessAttachments()
        {   
            var azureInterface = new AzureInterface(AdminService, Service, TracingService);
            var results = Service.GetEmailAndAttachmentsForId(Context.PrimaryEntityId);

            // Call the plugin to upload the email and all its attachments as files to SharePoint. 
            if (results != null && results.Entities.Count > 0)
            {
                var emailUploaded = false;
                if (results.Entities[0].Contains(Email.UploadedToSharePoint))
                {
                    // annotation and email
                    emailUploaded = results.Entities[0].GetAttributeValue<bool>(Email.UploadedToSharePoint);
                }

                if (!emailUploaded)
                {
                    TracingService.Trace("Requesting action for Email upload.");
                    azureInterface.SendFileToSharePointActionRequest(Context.PrimaryEntityName, Context.PrimaryEntityId);
                }
                else
                {
                    TracingService.Trace("Email already uploaded to SharePoint");
                }

                // Now process the attachments
                TracingService.Trace("Processing {0} attachments.", results.Entities.Count.ToString());
                foreach (Entity attachment in results.Entities)
                {
                    var filesize = 0;
                    if (attachment.Contains("attachment.filesize"))
                    {
                        filesize = (int)((AliasedValue)attachment.Attributes["attachment.filesize"]).Value;
                    }
                    var filename = string.Empty;
                    if (attachment.Contains("attachment.filename"))
                    {                     
                        filename = (string)((AliasedValue)attachment.Attributes["attachment.filename"]).Value;
                    }
                    TracingService.Trace("Attachment Id={0}, filename={1}, size={2}.", attachment.Id.ToString(), filename, filesize.ToString());
                    if (filesize > 0)
                    {
                        var attachmentId = (Guid)((AliasedValue)attachment.Attributes["attachment.activitymimeattachmentid"]).Value;
                        // Using an action because we don't know how many attachments we'll have. Could take more than process
                        // limit of 2 minutes so using action trigger async plugin.
                        azureInterface.SendFileToSharePointActionRequest(ActivityMimeAttachment.EntityLogicalName, attachmentId);

                        TracingService.Trace("{0} request sent", PluginMessages.SendFileToSharePoint);
                    }
                    else
                    {
                        TracingService.Trace("Attachment has zero filesize. Do not upload.", PluginMessages.SendFileToSharePoint, attachment.Id.ToString());
                    }
                }
            }
        }
    }
}