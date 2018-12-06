namespace Lp.Model.Internal
{
    using System;

    /// <summary>
    ///  Class provides a model linking Application Lines to Application Questions
    /// </summary>
    public class ApplicationQuestionsAndLines
    {
        /// <summary>
        /// Primary key for application lines
        /// </summary>
        public Guid ApplicationLineId { get; set; }

        /// <summary>
        /// Primary key for application questions
        /// </summary>
        public Guid ApplicationQuestionId { get; set; }
    }
}
