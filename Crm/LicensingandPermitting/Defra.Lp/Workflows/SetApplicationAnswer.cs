namespace Lp.Workflows
{
    using System;
    using System.Activities;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using Defra.Lp.Workflows;
    using DataAccess;
    using Model.EarlyBound;

    /// <summary>
    /// Code activity sets an application answer by looking up the question and option codes
    /// Creates the application answer if it does not already exist based on boolean parameter
    /// </summary>
    public class SetApplicationAnswer: WorkFlowActivityBase
    {
        
        [RequiredArgument]
        [Input("Question Code")]
        public InArgument<string> QuestionCode { get; set; }
        
        [Input("Answer Option Code")]
        public InArgument<string> AnswerOptionCode { get; set; }

        [Input("Answer Option Text")]
        public InArgument<string> AnswerOptionText { get; set; }

        [RequiredArgument]
        [Input("Answer Application ")]
        [ReferenceTarget(defra_application.EntityLogicalName)]
        public InArgument<EntityReference> AnswerApplication { get; set; }

        [RequiredArgument]
        [Input("Answer Application Line")]
        [ReferenceTarget(defra_applicationline.EntityLogicalName)]
        public InArgument<EntityReference> AnswerApplicationLine { get; set; }

        [Input("Create Answer If Not Exists")]
        public InArgument<bool> CreateAnswerIfNotExists { get; set; }
        
        [Output("Application Answer")]
        [ReferenceTarget(defra_applicationanswer.EntityLogicalName)]
        public OutArgument<EntityReference> ApplicationAnswer { get; set; }

        /// <summary>
        /// Main Execution Method
        /// </summary>
        /// <param name="executionContext">Activity Execution Context</param>
        /// <param name="crmWorkflowContext">Includes organisation service and trace service</param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            // 1. Validation
            if (crmWorkflowContext == null)
            {
                throw new ArgumentNullException(nameof(crmWorkflowContext));
            }
            
            // 2. Processing
            DataAccessApplicationAnswers dal = new DataAccessApplicationAnswers(crmWorkflowContext.OrganizationService, executionContext.GetExtension<ITracingService>());

            Guid answerId = dal.SetApplicationAnswer(
                QuestionCode.Get<string>(executionContext),
                AnswerOptionCode.Get<string>(executionContext),
                AnswerOptionText.Get<string>(executionContext),
                AnswerApplication.Get<EntityReference>(executionContext),
                AnswerApplicationLine.Get<EntityReference>(executionContext),
                true);

            // 3. Return
            ApplicationAnswer.Set(executionContext, new EntityReference(defra_applicationanswer.EntityLogicalName, answerId));
        }
    }
}
