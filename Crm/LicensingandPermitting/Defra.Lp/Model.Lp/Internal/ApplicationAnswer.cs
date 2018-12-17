namespace Lp.Model.Internal
{
    using System;

    /// <summary>
    ///  Class provides a model for Application Answers
    /// </summary>
    public class ApplicationAnswer
    {

        /// <summary>
        /// Primary key for application answer
        /// </summary>
        public Guid ApplicationAnswerId { get; set; }

        /// <summary>
        /// Primary key for application lines
        /// </summary>
        public Guid? ApplicationLineId { get; set; }

        /// <summary>
        /// Primary key for application questions
        /// </summary>
        public Guid? ApplicationQuestionId { get; set; }

        /// <summary>
        /// Answer text field
        /// </summary>
        public string AnswerText { get; set; }

        /// <summary>
        /// Primary key for application question option
        /// </summary>
        public Guid? ApplicationQuestionOptionId { get; set; }
    }
}
