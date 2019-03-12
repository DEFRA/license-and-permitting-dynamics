namespace Lp.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk;
    using Core.Helpers.Extensions;
    using Model.EarlyBound;
    using Core.DataAccess.Base;
    using Model.Internal;

    /// <summary>
    /// Data access layer for Application Answer related CRM queries
    /// </summary>
    public class DataAccessApplicationAnswers : DataAccessBase
    {

        /// <summary>
        /// Internal alias used for CRM joins
        /// </summary>
        private const string QuestionEntityAlias = "question";

        private const string QuestionOptionEntityAlias = "option";

        /// <summary>
        /// Constructor sets the CRM services
        /// </summary>
        /// <param name="organisationService">CRM organisation service</param>
        /// <param name="tracingService">CRM trace service</param>
        public DataAccessApplicationAnswers(IOrganizationService organisationService,
            ITracingService tracingService) : base(organisationService, tracingService)
        {
        }

        /// <summary>
        /// Returns a defra_application_answer record for the given application, application line and question code
        /// </summary>
        /// <param name="applicationQuestionCode">The question code to look up the answer by</param>
        /// <param name="application">The application to lookup the answer by</param>
        /// <param name="applicationLine">The application line to lookup the answer by</param>
        /// <returns></returns>
        public Entity GetApplicationAnswer(string applicationQuestionCode, EntityReference application,
            EntityReference applicationLine)
        {

            // Prep the CRM query
            var query = new QueryExpression(defra_applicationanswer.EntityLogicalName) { TopCount = 1 };
            query.ColumnSet.AddColumns(
                defra_applicationanswer.Fields.defra_answer_option,
                defra_applicationanswer.Fields.defra_applicationlineid,
                defra_applicationanswer.Fields.defra_application,
                defra_applicationanswer.Fields.defra_answertext,
                defra_applicationanswer.Fields.defra_applicationanswerId,
                defra_applicationanswer.Fields.defra_question);

            // Filter by application and get only active records
            query.Criteria.AddCondition(defra_applicationanswer.Fields.defra_application, ConditionOperator.Equal,
                application.Id);
            query.Criteria.AddCondition(defra_applicationanswer.Fields.StateCode, ConditionOperator.Equal,
                (int)defra_applicationanswerState.Active);

            // If application line is specified, also filter by it. Only used for answers that are application line specific
            if (applicationLine != null)
            {
                query.Criteria.AddCondition(defra_applicationanswer.Fields.defra_applicationlineid,
                    ConditionOperator.Equal, applicationLine.Id);
            }

            // Join with the defra_application_question entity to match against the question code
            var linkToQuestion = query.AddLink(defra_applicationquestion.EntityLogicalName,
                defra_applicationanswer.Fields.defra_question,
                defra_applicationquestion.Fields.defra_applicationquestionId);
            linkToQuestion.EntityAlias = QuestionEntityAlias;
            linkToQuestion.LinkCriteria.AddCondition(defra_applicationquestion.Fields.StateCode,
                ConditionOperator.Equal, (int)defra_applicationquestionState.Active);
            linkToQuestion.LinkCriteria.AddCondition(defra_applicationquestion.Fields.defra_shortname,
                ConditionOperator.Equal, applicationQuestionCode);

            // Join with the defra_application_question_option entity to retrieve the option code if set, hence the outer join here
            var linkToAnswerOption = query.AddLink(defra_applicationquestionoption.EntityLogicalName,
                defra_applicationanswer.Fields.defra_answer_option,
                defra_applicationquestionoption.Fields.defra_applicationquestionoptionId, JoinOperator.LeftOuter);
            linkToAnswerOption.EntityAlias = QuestionOptionEntityAlias;
            linkToAnswerOption.Columns.AddColumns(defra_applicationquestionoption.Fields.defra_option,
                defra_applicationquestionoption.Fields.defra_shortname);
            linkToAnswerOption.LinkCriteria.AddCondition(defra_applicationquestionoption.Fields.StateCode,
                ConditionOperator.Equal, (int)defra_applicationquestionoptionState.Active);

            // Talk to CRM, get the answer record, and return it
            EntityCollection result = OrganisationService.RetrieveMultiple(query);
            if (result?.Entities != null && result.Entities.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        /// <summary>
        /// Returns a defra_application_question record for a given question code
        /// </summary>
        /// <param name="code">Question code to lookup against</param>
        /// <returns>The matching application question record</returns>
        public Entity GetApplicationQuestion(string code)
        {
            // Query CRM for a defra_application_question
            var query = new QueryExpression(defra_applicationquestion.EntityLogicalName)
            {
                TopCount = 1,
                Criteria = { FilterOperator = LogicalOperator.And }
            };

            // Got only the matching active question 
            query.Criteria.AddCondition(defra_applicationquestion.Fields.defra_shortname, ConditionOperator.Equal,
                code);
            query.Criteria.AddCondition(defra_applicationquestion.Fields.StateCode, ConditionOperator.Equal,
                (int)defra_applicationquestionState.Active);

            // Talk to CRM, return the question record if found
            EntityCollection result = OrganisationService.RetrieveMultiple(query);
            if (result?.Entities != null && result.Entities.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        /// <summary>
        /// Returns a defra_application_question_option record based on an option code.
        /// </summary>
        /// <param name="code">Code for the record to be retrieved</param>
        /// <returns></returns>
        private Entity GetApplicationQuestionOption(string code)
        {
            // Query CRM for a defra_application_question_option
            var query = new QueryExpression(defra_applicationquestionoption.EntityLogicalName)
            {
                TopCount = 1,
                Criteria = { FilterOperator = LogicalOperator.And }
            };

            // Got only the matching active question option
            query.Criteria.AddCondition(defra_applicationquestionoption.Fields.defra_shortname, ConditionOperator.Equal,
                code);
            query.Criteria.AddCondition(defra_applicationquestionoption.Fields.StateCode, ConditionOperator.Equal,
                (int)defra_applicationquestionoptionState.Active);

            // Talk to CRM, return the question option record if found
            EntityCollection result = OrganisationService.RetrieveMultiple(query);
            if (result?.Entities != null && result.Entities.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        /// <summary>
        /// Returns an Application Question and Option entity for the given option code and question code
        /// </summary>
        /// <param name="questionCode">Question code for the question to be retrieved</param>
        /// <param name="questionOptionCode">Option code for the question to be retrieved</param>
        /// <returns>Entity containing application.</returns>
        public Entity GetApplicationQuestionAndOption(string questionCode, string questionOptionCode)
        {
            var query = new QueryExpression(defra_applicationquestionoption.EntityLogicalName) { TopCount = 1 };

            query.ColumnSet.AddColumns(defra_applicationquestionoption.Fields.defra_applicationquestionoptionId);
            query.Criteria.AddCondition(defra_applicationquestionoption.Fields.StateCode, ConditionOperator.Equal,
                (int)defra_applicationquestionoptionState.Active);
            query.Criteria.AddCondition(defra_applicationquestionoption.Fields.defra_shortname, ConditionOperator.Equal,
                questionOptionCode);

            // link to Question
            var linkToQuestion = query.AddLink(defra_applicationquestion.EntityLogicalName,
                defra_applicationquestionoption.Fields.defra_applicationquestion,
                defra_applicationquestion.Fields.defra_applicationquestionId);
            linkToQuestion.EntityAlias = QuestionEntityAlias;

            // Add columns to QEdefra_applicationquestionoption_defra_applicationquestion.Columns
            linkToQuestion.Columns.AddColumns(defra_applicationquestion.Fields.defra_applicationquestionId);

            // Define filter QEdefra_applicationquestionoption_defra_applicationquestion.LinkCriteria
            linkToQuestion.LinkCriteria.AddCondition(defra_applicationquestion.Fields.StateCode,
                ConditionOperator.Equal, (int)defra_applicationquestionState.Active);
            linkToQuestion.LinkCriteria.AddCondition(defra_applicationquestion.Fields.defra_shortname,
                ConditionOperator.Equal, questionCode);

            EntityCollection result = OrganisationService.RetrieveMultiple(query);
            if (result?.Entities != null && result.Entities.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        /// <summary>
        /// Functions creates or udpates an Application Answer record based on the application id, question code and option codes
        /// provided in the parameters.
        /// </summary>
        /// <param name="applicationQuestionCode">defra_application_question short name to be used when looking up the question</param>
        /// <param name="applicationAnswerOptionCode">defra_application_question_option short name to be used whn looking up the aswer, this may be null if the answer is plain text</param>
        /// <param name="applciationAnswerText">the plain text answer to be written to the application answer record, this may be null if the question is option only</param>
        /// <param name="application">The application id that will be linked to the answer record</param>
        /// <param name="applicationLine">The application line id that will be linked to the answer record</param>
        /// <param name="createIfNotExists">If true it will create the application record if it does not exist</param>
        /// <returns></returns>
        public Guid SetApplicationAnswer(
            string applicationQuestionCode,
            string applicationAnswerOptionCode,
            string applciationAnswerText,
            EntityReference application,
            EntityReference applicationLine,
            bool createIfNotExists)
        {
            // 1. Check if answer already exists
            Entity existingApplicationAnswer =
                GetApplicationAnswer(applicationQuestionCode, application, applicationLine);

            if (existingApplicationAnswer == null)
            {
                // 2. Create Answer
                if (createIfNotExists)
                {
                    return CreateApplicationAnswer(applicationQuestionCode, applicationAnswerOptionCode,
                        applciationAnswerText, application, applicationLine);
                }

                // We should not create it if it doesn't exist, throw exception
                throw new InvalidPluginExecutionException(
                    $"Could not find application answer record for appliction {application?.Id}, question code {applicationQuestionCode} and application line {applicationLine?.Id}");
            }

            // 3. Update Answer
            return UpdateApplicationAnswer(applicationQuestionCode, applicationAnswerOptionCode, applciationAnswerText,
                existingApplicationAnswer);
        }

        /// <summary>
        /// Updates an application answer record with the given question and option lookup codes
        /// </summary>
        /// <param name="applicationQuestionCode"></param>
        /// <param name="applicationAnswerOptionCode"></param>
        /// <param name="applciationAnswerText"></param>
        /// <param name="existingApplicationAnswer"></param>
        /// <returns></returns>
        private Guid UpdateApplicationAnswer(
            string applicationQuestionCode,
            string applicationAnswerOptionCode,
            string applciationAnswerText,
            Entity existingApplicationAnswer)
        {
            // Prepare the application answer entity to be updated
            Entity applicationAnswerToUpdate = new Entity(defra_applicationanswer.EntityLogicalName,
                existingApplicationAnswer.Id);

            // Set Application Lookup
            applicationAnswerToUpdate.Attributes.Add(defra_applicationanswer.Fields.defra_application,
                existingApplicationAnswer[defra_applicationanswer.Fields.defra_application]);

            // Set Answer Text
            applicationAnswerToUpdate.Attributes.Add(defra_applicationanswer.Fields.defra_answertext,
                applciationAnswerText);

            // Set Question Lookup
            applicationAnswerToUpdate.Attributes.Add(defra_applicationanswer.Fields.defra_question,
                existingApplicationAnswer[defra_applicationanswer.Fields.defra_question]);

            // Set Answer Option?
            if (!string.IsNullOrWhiteSpace(applicationAnswerOptionCode))
            {
                Entity answerOption =
                    GetApplicationQuestionAndOption(applicationQuestionCode, applicationAnswerOptionCode);

                if (answerOption == null)
                {
                    throw new InvalidPluginExecutionException(
                        $"Could not find Application Question Option with code {applicationAnswerOptionCode} for application question {applicationQuestionCode}");
                }

                // Set Answer Option
                applicationAnswerToUpdate.Attributes.Add(defra_applicationanswer.Fields.defra_answer_option,
                    answerOption.ToEntityReference());
            }

            // Talk to crm
            OrganisationService.Update(applicationAnswerToUpdate);
            return applicationAnswerToUpdate.Id;
        }

        /// <summary>
        /// Creates a defra_application_answer record using the given codes
        /// </summary>
        /// <param name="applicationQuestionCode">Question code to set in the newly created applciation answer record</param>
        /// <param name="applicationAnswerOptionCode">Answer Option code to set in the new record, may be null</param>
        /// <param name="applicationAnswerText">Answer plain text to be set, may be null</param>
        /// <param name="application">Application to link to</param>
        /// <param name="applicationLine">Application Line to link to, may be null</param>
        /// <returns>Guid for the newly created application answer record</returns>
        private Guid CreateApplicationAnswer(
            string applicationQuestionCode,
            string applicationAnswerOptionCode,
            string applicationAnswerText,
            EntityReference application,
            EntityReference applicationLine)
        {
            // Prep the new entity for creation
            Entity newAnswer = new Entity(defra_applicationanswer.EntityLogicalName);

            // Set Application
            newAnswer.Attributes.Add(defra_applicationanswer.Fields.defra_application, application);

            // Set Answer Text
            newAnswer.Attributes.Add(defra_applicationanswer.Fields.defra_answertext, applicationAnswerText);

            // Set Application Line
            if (applicationLine != null)
            {
                newAnswer.Attributes.Add(defra_applicationanswer.Fields.defra_applicationlineid, applicationLine);
            }

            // Set Answer Option and Question
            if (!string.IsNullOrWhiteSpace(applicationAnswerOptionCode))
            {

                // Get the corresponding application question option, and the question in one go, save query time by doing one query
                Entity questionAndOptionEntities =
                    GetApplicationQuestionAndOption(applicationQuestionCode, applicationAnswerOptionCode);

                if (questionAndOptionEntities == null)
                {
                    throw new InvalidPluginExecutionException(
                        $"Could not find Application Question Option with code {applicationAnswerOptionCode} for application question {applicationQuestionCode}");
                }

                // Set the Answer Option
                if (questionAndOptionEntities.Contains(defra_applicationquestionoption.Fields
                    .defra_applicationquestionoptionId))
                {
                    EntityReference optionEntityReference =
                        new EntityReference(defra_applicationquestionoption.EntityLogicalName,
                            questionAndOptionEntities.GetAttributeValue<Guid>(defra_applicationquestionoption.Fields
                                .defra_applicationquestionoptionId));
                    newAnswer.Attributes.Add(defra_applicationanswer.Fields.defra_answer_option, optionEntityReference);
                }

                // Set the Question
                string questionAttribute =
                    $"{QuestionEntityAlias}.{defra_applicationquestion.Fields.defra_applicationquestionId}";
                if (questionAndOptionEntities.Contains(questionAttribute))
                {
                    EntityReference questionEntityReference =
                        new EntityReference(defra_applicationquestionoption.EntityLogicalName,
                            (Guid)questionAndOptionEntities.GetAttributeValue<AliasedValue>(questionAttribute).Value);
                    newAnswer.Attributes.Add(defra_applicationanswer.Fields.defra_question, questionEntityReference);
                }
            }
            // Set the answer Question, no answer option
            else
            {
                // Get the question record for the given code
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

            // Tell CRM to create the answer record, and return it's Id
            return OrganisationService.Create(newAnswer);
        }

        /// <summary>
        /// Returns a list of application questions that are applicable to an application 
        /// based on the application lines and items linked to the application
        /// </summary>
        /// <param name="applicationId">Guid for the application to check</param>
        /// <returns>A simplified array of application questions and application line ids that are applicable</returns>
        public List<ApplicationQuestionsAndLines> GetApplicableApplicationQuestions(Guid applicationId)
        {
            // 1. Prepare query, start with application line
            QueryExpression query = new QueryExpression(defra_applicationline.EntityLogicalName);
            query.ColumnSet.AddColumns(defra_applicationline.Fields.defra_applicationlineId);
            query.Criteria.AddCondition(defra_applicationline.Fields.StateCode, ConditionOperator.Equal,
                (int)defra_applicationlineState.Active);
            query.Criteria.AddCondition(defra_applicationline.Fields.defra_applicationId, ConditionOperator.Equal,
                applicationId);

            // 2. Link to Item
            LinkEntity linkItem = query.AddLink(defra_item.EntityLogicalName, defra_item.Fields.defra_itemId,
                defra_item.Fields.defra_itemId);
            linkItem.LinkCriteria.AddCondition(defra_item.Fields.StateCode, ConditionOperator.Equal,
                (int)defra_itemState.Active);

            // 3. Link to Item Application Question linker table
            LinkEntity linkItemApplicationQuestion = linkItem.AddLink(defra_item_application_question.EntityLogicalName,
                defra_item.Fields.defra_itemId, defra_item_application_question.Fields.defra_itemid);
            linkItemApplicationQuestion.LinkCriteria.AddCondition(defra_item_application_question.Fields.StateCode,
                ConditionOperator.Equal, (int)defra_item_application_questionState.Active);
            linkItemApplicationQuestion.Columns.AddColumns(defra_item_application_question.Fields.defra_scope);
            linkItemApplicationQuestion.EntityAlias = "linker";

            // 4. Link to Application Question
            LinkEntity linkQuestion = linkItemApplicationQuestion.AddLink(
                defra_applicationquestion.EntityLogicalName,
                defra_item_application_question.Fields.defra_applicationquestionid,
                defra_applicationquestion.Fields.defra_applicationquestionId);
            linkQuestion.EntityAlias = "question";
            linkQuestion.Columns.AddColumns(defra_applicationquestion.Fields.defra_applicationquestionId);
            linkQuestion.LinkCriteria.AddCondition(defra_applicationquestion.Fields.StateCode, ConditionOperator.Equal,
                (int)defra_applicationquestionState.Active);

            // Talk to CRM, get results
            EntityCollection resultEntities = OrganisationService.RetrieveMultiple(query);

            // No results
            if (resultEntities?.Entities == null || resultEntities.Entities.Count < 1)
            {
                return null;
            }

            // Return simple model result
            string scopeAttributeName = $"{linkItemApplicationQuestion.EntityAlias}.{defra_item_application_question.Fields.defra_scope}";
            string questionIdAttributeName = $"{linkQuestion.EntityAlias}.{defra_applicationquestion.Fields.defra_applicationquestionId}";

            return resultEntities.Entities.Select(entity =>
                new ApplicationQuestionsAndLines
                {
                    ApplicationLineId = entity.GetAttributeId(defra_applicationline.Fields.defra_applicationlineId),
                    ApplicationQuestionId = entity.GetAliasedAttributeId(questionIdAttributeName),
                    Scope = entity.GetAliasedOptionSetValue(scopeAttributeName),
                }).ToList();
        }

        /// <summary>
        /// Returns a list of application answer records currently linked to an application
        /// </summary>
        /// <param name="applicationId">Guid for the application to query</param>
        /// <returns>A simplified model that contains answer records</returns>
        public List<ApplicationAnswer> GetApplicationAnswers(Guid applicationId)
        {
            // Prepare query, quer applications answer key fields
            QueryExpression query = new QueryExpression(defra_applicationanswer.EntityLogicalName);

            query.ColumnSet.AddColumns(
                defra_applicationanswer.Fields.defra_answer_option,
                defra_applicationanswer.Fields.defra_answertext,
                defra_applicationanswer.Fields.defra_applicationanswerId,
                defra_applicationanswer.Fields.defra_applicationlineid,
                defra_applicationanswer.Fields.defra_question);

            query.Criteria.AddCondition(defra_applicationanswer.Fields.StateCode, ConditionOperator.Equal,
                (int)defra_applicationanswerState.Active);
            query.Criteria.AddCondition(defra_applicationanswer.Fields.defra_application, ConditionOperator.Equal,
                applicationId);

            // Talk to CRM, get results
            EntityCollection resultEntities = OrganisationService.RetrieveMultiple(query);

            // No results
            if (resultEntities?.Entities == null || resultEntities.Entities.Count < 1)
            {
                return null;
            }

            // Return simple model result
            return resultEntities.Entities.Select(entity =>
                new ApplicationAnswer
                {
                    ApplicationAnswerId = entity.Id,
                    ApplicationQuestionId = entity.GetAttributeId(defra_applicationanswer.Fields.defra_question),
                    ApplicationQuestionOptionId = entity.GetAttributeId(defra_applicationanswer.Fields.defra_answer_option),
                    ApplicationLineId = entity.GetAttributeId(defra_applicationanswer.Fields.defra_applicationlineid),
                    AnswerText = entity.GetAttributeText(defra_applicationanswer.Fields.defra_answertext),
                }).ToList();
        }

        /// <summary>
        /// Function takes care of removing application answers that are no longer applicable
        /// to an application, and adds new answer records depending on the application lines
        /// linked to the application
        /// </summary>
        /// <param name="applicationId">Guid for the application to process</param>
        public void RefreshApplicationAnswers(Guid applicationId)
        {
            // Get the list of questions that should be there
            List<ApplicationQuestionsAndLines> applicableQuestionsAndLines = GetApplicableApplicationQuestions(applicationId) ?? new List<ApplicationQuestionsAndLines>();

            // And the list of questions that are linked to the application
            List<ApplicationAnswer> currentApplicationAnswers = GetApplicationAnswers(applicationId) ?? new List<ApplicationAnswer>();

            // Now work out which application answers should be removed
            foreach (ApplicationAnswer appAnswer in currentApplicationAnswers)
            {
                // Check if the existing application answer still applies
                if (!HasMatchingApplicationQuestion(applicableQuestionsAndLines, appAnswer))
                {
                    // Application answer no longer applies, deactivate it
                    SetStatusAndState(
                        new EntityReference(defra_applicationanswer.EntityLogicalName, appAnswer.ApplicationAnswerId),
                        (int)defra_applicationanswerState.Inactive,
                        (int)defra_applicationanswer_StatusCode.Inactive);
                }
            }

            // And work out which application answers should be created
            foreach (ApplicationQuestionsAndLines appQuestionAndLine in applicableQuestionsAndLines)
            {
                // Check if answer already exists, create the Answer record if it does not already exist
                if (!HasMatchingApplicationAnswer(currentApplicationAnswers, appQuestionAndLine))
                {
                    ApplicationAnswer newAppAnswer = CreateAnswerRecord(applicationId, appQuestionAndLine);

                    // Save the newly created application answer, so that we don't create a duplicate
                    currentApplicationAnswers.Add(newAppAnswer);
                }
            }
        }

        private static bool HasMatchingApplicationQuestion(List<ApplicationQuestionsAndLines> applicableQuestionsAndLines, ApplicationAnswer appAnswer)
        {
            return applicableQuestionsAndLines.Any(
                q => q.ApplicationQuestionId == appAnswer.ApplicationQuestionId
                // If scope of question is at the Item level, also check the application line
                && (!q.Scope.HasValue || q.Scope.Value == (int)defra_application_task_scope.Item && q.ApplicationLineId == appAnswer.ApplicationLineId));
        }

        private static bool HasMatchingApplicationAnswer(List<ApplicationAnswer> currentApplicationAnswers, ApplicationQuestionsAndLines appQuestionAndLine)
        {
            foreach (var currentApplicationAnswer in currentApplicationAnswers)
            {
                if (currentApplicationAnswer.ApplicationQuestionId != appQuestionAndLine.ApplicationQuestionId)
                {
                    // Not the same question
                    continue;
                }

                if (appQuestionAndLine.Scope == (int) defra_application_task_scope.Item &&
                    currentApplicationAnswer.ApplicationLineId == appQuestionAndLine.ApplicationLineId)
                {
                    // Question is at the item/activity level, check for an exact match against the application line
                    return true;
                }

                if (!appQuestionAndLine.Scope.HasValue || appQuestionAndLine.Scope.Value == (int) defra_application_task_scope.Application)
                {
                    // Question is at the application level, check for answers not linked to an application line
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates an answer record with the given parameters
        /// </summary>
        /// <param name="applicationId">Application to link the answer to</param>
        /// <param name="appQuestionAndLine">Details of the question and line if applicable</param>
        public ApplicationAnswer CreateAnswerRecord(Guid applicationId, ApplicationQuestionsAndLines appQuestionAndLine)
        {
            ApplicationAnswer newAnswer = new ApplicationAnswer
            {
                ApplicationQuestionId = appQuestionAndLine.ApplicationQuestionId,
                ApplicationLineId = appQuestionAndLine.ApplicationLineId
            };

            // Application answer does not exist, create it
            Entity newAnswerEntity = new Entity(defra_applicationanswer.EntityLogicalName);
            newAnswerEntity.Attributes.Add(defra_applicationanswer.Fields.defra_application,
                new EntityReference(defra_application.EntityLogicalName, applicationId));

            if (appQuestionAndLine.ApplicationQuestionId.HasValue)
            {
                newAnswerEntity.Attributes.Add(defra_applicationanswer.Fields.defra_question,
                    new EntityReference(defra_applicationquestion.EntityLogicalName,
                        appQuestionAndLine.ApplicationQuestionId.Value));
            }

            // If Question is at the Item level, link the answer to the corresponding application line
            if (appQuestionAndLine.ApplicationLineId.HasValue
                && appQuestionAndLine.Scope.HasValue
                && appQuestionAndLine.Scope.Value == (int)defra_application_task_scope.Item)
            {
                newAnswerEntity.Attributes.Add(
                    defra_applicationanswer.Fields.defra_applicationlineid,
                    new EntityReference(
                        defra_applicationline.EntityLogicalName,
                        appQuestionAndLine.ApplicationLineId.Value));
            }

            // Talk to CRM, create the answer record
            newAnswer.ApplicationAnswerId = OrganisationService.Create(newAnswerEntity);

            // Return answer
            return newAnswer;
        }
    }
}