using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Model.Lp.Crm;

namespace Lp.DataAccess
{
    public static class DataAccessEmail
    {
        public static EntityCollection GetEmailAndAttachmentsForId(this IOrganizationService service, Guid emailId)
        {
            // Instantiate QueryExpression QEemail
            var QEemail = new QueryExpression(Email.EntityLogicalName);

            // Add columns to QEemail.ColumnSet
            QEemail.ColumnSet.AddColumns(Email.ActivityId, Email.UploadedToSharePoint, Email.Subject);

            // Define filter QEemail.Criteria. Only want specific email
            QEemail.Criteria.AddCondition(Email.ActivityId, ConditionOperator.Equal, emailId);

            // Add link-entity QEemail_activitymimeattachment
            var QEemail_activitymimeattachment = QEemail.AddLink(ActivityMimeAttachment.EntityLogicalName, Email.ActivityId, ActivityMimeAttachment.ObjectId, JoinOperator.LeftOuter);
            QEemail_activitymimeattachment.EntityAlias = "attachment";

            // Add columns to Attachments
            QEemail_activitymimeattachment.Columns.AddColumns(ActivityMimeAttachment.Id, ActivityMimeAttachment.Filename, ActivityMimeAttachment.Filesize);

            // Define filter for Attachments. Only want those greater than zero
            QEemail_activitymimeattachment.LinkCriteria.AddCondition(ActivityMimeAttachment.Filesize, ConditionOperator.GreaterThan, 0);

            return service.RetrieveMultiple(QEemail);
        }
    }
}