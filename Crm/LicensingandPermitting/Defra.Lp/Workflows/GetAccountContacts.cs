// CRM Code Activity used to return up to 15 contact records with a specific role for a given account
using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Lp.DataAccess;
using Lp.Model.EarlyBound;

namespace Defra.Lp.Workflows
{

    // Main code ativity class    
    public class GetAccountContacts : WorkFlowActivityBase
    {

        #region Properties 

        /// <summary>
        /// The account to filter contacts by
        /// </summary>
        [RequiredArgument]
        [Input("Account")]
        [ReferenceTarget("account")]
        public InArgument<EntityReference> Account { get; set; }

        /// <summary>
        /// Contact Account Role Code
        /// </summary>
        [RequiredArgument]
        [Input("Contact Role")]
        public InArgument<int> ContactRole { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact1")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact1 { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact2")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact2 { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact3")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact3 { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact4")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact4 { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact5")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact5 { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact6")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact6 { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact7")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact7 { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact8")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact8 { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact9")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact9 { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact10")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact10 { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact11")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact11 { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact12")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact12 { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact13")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact13 { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact14")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact14 { get; set; }

        /// <summary>
        /// Contact Found
        /// </summary>
        [Output("Contact15")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact15 { get; set; }


        private ITracingService TracingService { get; set; }
        private IOrganizationService Service { get; set; }

        #endregion
        /// <summary>
        /// Main code activity function
        /// </summary>
        /// <param name="executionContext"></param>
        /// <param name="crmWorkflowContext"></param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {

            if (crmWorkflowContext == null)
            {
                throw new ArgumentNullException(nameof(crmWorkflowContext));
            }

            TracingService = executionContext.GetExtension<ITracingService>();
            Service = crmWorkflowContext.OrganizationService;

            // 1. Validation
            var account = this.Account.Get(executionContext);
            int contactRole = this.ContactRole.Get(executionContext);

            if (account == null)
            {
                TracingService.Trace("Account parameter not set.");

                return;
            }

            if (contactRole < 1)
            {
                TracingService.Trace("Contact Role parameter not set.");
                return;
            }

            TracingService.Trace("Getting Contacts for Account {0} and Contact Role : {1}", account.Id, contactRole);

            // 2. Processing - Query CRM for contacts of a given role and linked to the given account
            DataAccessContact dataAccess = new DataAccessContact(this.Service, this.TracingService);
            EntityReference [] contactEntityReferences = dataAccess.GetAccountContacts(account.Id, (Contact_AccountRoleCode)contactRole);

            // 3. Return contacts found
            if (contactEntityReferences == null)
            {
                // No contacts
                return;
            }

            int contactCount = 0;
            if (contactEntityReferences.Length > contactCount)
            {
                Contact1.Set(executionContext, contactEntityReferences[contactCount++]);
            }

            if (contactEntityReferences.Length > contactCount)
            {
                Contact2.Set(executionContext, contactEntityReferences[contactCount++]);
            }

            if (contactEntityReferences.Length > contactCount)
            {
                Contact3.Set(executionContext, contactEntityReferences[contactCount++]);
            }

            if (contactEntityReferences.Length > contactCount)
            {
                Contact4.Set(executionContext, contactEntityReferences[contactCount++]);
            }

            if (contactEntityReferences.Length > contactCount)
            {
                Contact5.Set(executionContext, contactEntityReferences[contactCount++]);
            }

            if (contactEntityReferences.Length > contactCount)
            {
                Contact6.Set(executionContext, contactEntityReferences[contactCount++]);
            }

            if (contactEntityReferences.Length > contactCount)
            {
                Contact7.Set(executionContext, contactEntityReferences[contactCount++]);
            }

            if (contactEntityReferences.Length > contactCount)
            {
                Contact8.Set(executionContext, contactEntityReferences[contactCount++]);
            }

            if (contactEntityReferences.Length > contactCount)
            {
                Contact9.Set(executionContext, contactEntityReferences[contactCount++]);
            }

            if (contactEntityReferences.Length > contactCount)
            {
                Contact10.Set(executionContext, contactEntityReferences[contactCount++]);
            }

            if (contactEntityReferences.Length > contactCount)
            {
                Contact11.Set(executionContext, contactEntityReferences[contactCount++]);
            }

            if (contactEntityReferences.Length > contactCount)
            {
                Contact12.Set(executionContext, contactEntityReferences[contactCount++]);
            }

            if (contactEntityReferences.Length > contactCount)
            {
                Contact13.Set(executionContext, contactEntityReferences[contactCount++]);
            }

            if (contactEntityReferences.Length > contactCount)
            {
                Contact14.Set(executionContext, contactEntityReferences[contactCount++]);
            }
            if (contactEntityReferences.Length > contactCount)
            {
                Contact15.Set(executionContext, contactEntityReferences[contactCount]);
            }
        }
    }
}
