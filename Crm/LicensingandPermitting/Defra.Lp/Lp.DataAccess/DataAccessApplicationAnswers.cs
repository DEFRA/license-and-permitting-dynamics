namespace Lp.DataAccess
{
    using System;
    using Model.EarlyBound;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk;
    using Model.Crm;
    using Core.DataAccess.Base;

    /// <summary>
    /// Data access layer for Application Answers
    /// </summary>
    public class DataAccessApplicationAnswers : DataAccessBase
    {
        private const string QuestionEntityAlias = "question";

        private const string QuestionOptionEntityAlias = "option";

        /// <summary>
        /// Constructor sets the CRM services
        /// </summary>
        /// <param name="organisationService">CRM organisation service</param>
        /// <param name="tracingService">CRM trace service</param>
        public DataAccessApplicationAnswers(IOrganizationService organisationService, ITracingService tracingService) : base(organisationService, tracingService)
        {}

        public Entity GetApplicationAnswer(string applicationQuestionCode, EntityReference application, EntityReference applicationLine)
        {

            var query = new QueryExpression(defra_applicationanswer.EntityLogicalName) {TopCount = 1};

            query.ColumnSet.AddColumns(
                defra_applicationanswer.Fields.defra_answer_option, 
                defra_applicationanswer.Fields.defra_applicationlineid, 
                defra_applicationanswer.Fields.defra_application, 
                defra_applicationanswer.Fields.defra_answertext, 
                defra_applicationanswer.Fields.defra_applicationanswerId,
                defra_applicationanswer.Fields.defra_question);

            query.Criteria.AddCondition(defra_applicationanswer.Fields.defra_application, ConditionOperator.Equal, application.Id);
            query.Criteria.AddCondition(defra_applicationanswer.Fields.StateCode, ConditionOperator.Equal, (int)defra_applicationanswerState.Active);

            if (applicationLine != null)
            {
                query.Criteria.AddCondition(defra_applicationanswer.Fields.defra_applicationlineid, ConditionOperator.Equal, applicationLine.Id);
            }
            

            var linkToQuestion = query.AddLink(defra_applicationquestion.EntityLogicalName, defra_applicationanswer.Fields.defra_question, defra_applicationquestion.Fields.defra_applicationquestionId);
            linkToQuestion.EntityAlias = QuestionEntityAlias;


            linkToQuestion.LinkCriteria.AddCondition(defra_applicationquestion.Fields.StateCode, ConditionOperator.Equal, (int)defra_applicationquestionState.Active);
            linkToQuestion.LinkCriteria.AddCondition(defra_applicationquestion.Fields.defra_shortname, ConditionOperator.Equal, applicationQuestionCode);


            var linkToAnswerOption = query.AddLink(defra_applicationquestionoption.EntityLogicalName, defra_applicationanswer.Fields.defra_answer_option, defra_applicationquestionoption.Fields.defra_applicationquestionoptionId, JoinOperator.LeftOuter);
            linkToAnswerOption.EntityAlias = QuestionOptionEntityAlias;

            linkToAnswerOption.Columns.AddColumns(defra_applicationquestionoption.Fields.defra_option, defra_applicationquestionoption.Fields.defra_shortname);

            linkToAnswerOption.LinkCriteria.AddCondition(defra_applicationquestionoption.Fields.StateCode, ConditionOperator.Equal, (int)defra_applicationquestionoptionState.Active);

            EntityCollection result = OrganisationService.RetrieveMultiple(query);

            if (result?.Entities != null && result.Entities.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        public Entity GetApplicationQuestion(string code)
        {

            var query = new QueryExpression(defra_applicationquestion.EntityLogicalName) {TopCount = 1};
            query.Criteria.AddCondition(defra_applicationquestion.Fields.defra_shortname, ConditionOperator.Equal, code);
            EntityCollection result = OrganisationService.RetrieveMultiple(query);
            if (result?.Entities != null && result.Entities.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        public Entity GetApplicationQuestionOption(string code)
        {
            var query = new QueryExpression(defra_applicationquestionoption.EntityLogicalName) { TopCount = 1 };
            query.Criteria.AddCondition(defra_applicationquestionoption.Fields.defra_shortname, ConditionOperator.Equal, code);
            EntityCollection result = OrganisationService.RetrieveMultiple(query);
            if (result?.Entities != null && result.Entities.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        public Entity GetApplicationQuestionAndOption(string questionCode, string questionOptionCode)
        {
            var query = new QueryExpression(defra_applicationquestionoption.EntityLogicalName);
            query.TopCount = 1;

            query.ColumnSet.AddColumns(defra_applicationquestionoption.Fields.defra_applicationquestionoptionId);
            query.Criteria.AddCondition(defra_applicationquestionoption.Fields.StateCode, ConditionOperator.Equal, (int)defra_applicationquestionoptionState.Active);
            query.Criteria.AddCondition(defra_applicationquestionoption.Fields.defra_shortname, ConditionOperator.Equal, questionOptionCode);

            // link to Question
            var linkToQuestion = query.AddLink(defra_applicationquestion.EntityLogicalName, defra_applicationquestionoption.Fields.defra_applicationquestion, defra_applicationquestion.Fields.defra_applicationquestionId);
            linkToQuestion.EntityAlias = QuestionEntityAlias;

            // Add columns to QEdefra_applicationquestionoption_defra_applicationquestion.Columns
            linkToQuestion.Columns.AddColumns(defra_applicationquestion.Fields.defra_applicationquestionId);

            // Define filter QEdefra_applicationquestionoption_defra_applicationquestion.LinkCriteria
            linkToQuestion.LinkCriteria.AddCondition(defra_applicationquestion.Fields.StateCode, ConditionOperator.Equal, (int)defra_applicationquestionState.Active);
            linkToQuestion.LinkCriteria.AddCondition(defra_applicationquestion.Fields.defra_shortname, ConditionOperator.Equal, questionCode);

            EntityCollection result = OrganisationService.RetrieveMultiple(query);
            if (result?.Entities != null && result.Entities.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        public Guid SetApplicationAnswer(string applicationQuestionCode, string applicationAnswerOptionCode, string applciationAnswerText, EntityReference application, EntityReference applicationLine)
        {
            // 1. Check if answer already exists
            Entity existingApplicationAnswer = GetApplicationAnswer(applicationQuestionCode, application, applicationLine);

            if (existingApplicationAnswer == null)
            {
                // Create Answer
                return CreateApplicationAnswer(applicationQuestionCode, applicationAnswerOptionCode, applciationAnswerText,application, applicationLine);
            }

            // Update Answer
            return UpdateApplicationAnswer(applicationQuestionCode, applicationAnswerOptionCode, applciationAnswerText, existingApplicationAnswer);
        }

        private Guid UpdateApplicationAnswer(string applicationQuestionCode, string applicationAnswerOptionCode,
            string applciationAnswerText, Entity existingApplicationAnswer)
        {
            Entity applicationAnswerToUpdate =
                new Entity(defra_applicationanswer.EntityLogicalName, existingApplicationAnswer.Id);

            // Set Application
            applicationAnswerToUpdate.Attributes.Add(defra_applicationanswer.Fields.defra_application,
                existingApplicationAnswer[defra_applicationanswer.Fields.defra_application]);

            // Set Answer Text
            applicationAnswerToUpdate.Attributes.Add(defra_applicationanswer.Fields.defra_answertext, applciationAnswerText);

            // Set Question
            applicationAnswerToUpdate.Attributes.Add(defra_applicationanswer.Fields.defra_question,
                existingApplicationAnswer[defra_applicationanswer.Fields.defra_question]);


            if (!string.IsNullOrWhiteSpace(applicationAnswerOptionCode))
            {
                Entity answerOption = GetApplicationQuestionAndOption(applicationQuestionCode, applicationAnswerOptionCode);

                if (answerOption == null)
                {
                    throw new InvalidPluginExecutionException(
                        $"Could not find Application Question Option with code {applicationAnswerOptionCode} for application question {applicationQuestionCode}");
                }

                // Set Answer Option
                applicationAnswerToUpdate.Attributes.Add(defra_applicationanswer.Fields.defra_answer_option,
                    answerOption.ToEntityReference());
            }
            OrganisationService.Update(applicationAnswerToUpdate);
            return applicationAnswerToUpdate.Id;
        }

        private Guid CreateApplicationAnswer(
            string applicationQuestionCode, 
            string applicationAnswerOptionCode,
            string applciationAnswerText, 
            EntityReference application, 
            EntityReference applicationLine)
        {
            Entity newAnswer = new Entity(defra_applicationanswer.EntityLogicalName);

            // Set Application
            newAnswer.Attributes.Add(defra_applicationanswer.Fields.defra_application, application);

            // Set Answer Text
            newAnswer.Attributes.Add(defra_applicationanswer.Fields.defra_answertext, applciationAnswerText);

            // Set Application Line
            if (applicationLine != null)
            {
                newAnswer.Attributes.Add(defra_applicationanswer.Fields.defra_applicationlineid, applicationLine);
            }

            if (!string.IsNullOrWhiteSpace(applicationAnswerOptionCode))
            {
                Entity questionAndOptionEntities = GetApplicationQuestionAndOption(applicationQuestionCode, applicationAnswerOptionCode);

                if (questionAndOptionEntities == null)
                {
                    throw new InvalidPluginExecutionException(
                        $"Could not find Application Question Option with code {applicationAnswerOptionCode} for application question {applicationQuestionCode}");
                }

                // Set the Answer Option
                if (questionAndOptionEntities.Contains(defra_applicationquestionoption.Fields
                    .defra_applicationquestionoptionId))
                {
                    EntityReference optionEntityReference = new EntityReference(defra_applicationquestionoption.EntityLogicalName, questionAndOptionEntities.GetAttributeValue<Guid>(defra_applicationquestionoption.Fields.defra_applicationquestionoptionId));
                    newAnswer.Attributes.Add(defra_applicationanswer.Fields.defra_answer_option, optionEntityReference);
                }

                // Set the Question
                string questionAttribute = $"{QuestionEntityAlias}.{defra_applicationquestion.Fields.defra_applicationquestionId}";
                if (questionAndOptionEntities.Contains(questionAttribute))
                {
                    EntityReference questionEntityReference = new EntityReference(defra_applicationquestionoption.EntityLogicalName, (Guid)questionAndOptionEntities.GetAttributeValue<AliasedValue>(questionAttribute).Value);
                    newAnswer.Attributes.Add(defra_applicationanswer.Fields.defra_question, questionEntityReference);
                }
            }
            else
            {
                Entity applicationQuestion = GetApplicationQuestion(applicationQuestionCode);

                if (applicationQuestion == null)
                {
                    throw new InvalidPluginExecutionException(
                        $"Could not find Application Question with code {applicationAnswerOptionCode}");
                }

                // Set Application Question
                newAnswer.Attributes.Add(defra_applicationanswer.Fields.defra_question,
                    applicationQuestion.ToEntityReference());
            }

            return OrganisationService.Create(newAnswer);
        }
    }
}