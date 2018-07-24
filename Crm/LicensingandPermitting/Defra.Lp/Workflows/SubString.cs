// -----------------------------------------------------
// Code Activity returns a substring of the given string
// -----------------------------------------------------
namespace Defra.Lp.Workflows
{
    using System.Activities;
    using Microsoft.Xrm.Sdk.Workflow;
    using Core.Helpers.Extensions;

    /// <summary>
    /// Returns a substring of the parameter passed in
    /// </summary>
    public sealed class SubString : WorkFlowActivityBase
    {
        /// <summary>
        /// Run the code activity 
        /// </summary>
        /// <param name="executionContext">Workflow context</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            var result = Text.Get<string>(executionContext)
                .SafeSubstring(StartIndex.Get<int>(executionContext), Length.Get<int>(executionContext),
                    LeftToRight.Get<bool>(executionContext));
            Result.Set(executionContext, result);
        }

        /// <summary>
        /// Text being processed
        /// </summary>
        [Input("Text")]
        [RequiredArgument]
        public InArgument<string> Text { get; set; }

        /// <summary>
        /// Return substring
        /// </summary>
        [Output("Result")]
        [RequiredArgument]
        public OutArgument<string> Result { get; set; }

        /// <summary>
        /// True if substring should be performed left to right, false if otherwise
        /// </summary>
        [Input("From Left To Right")]
        [RequiredArgument]
        [Default("True")]
        public InArgument<bool> LeftToRight { get; set; }

        /// <summary>
        /// Substring start 0 index
        /// </summary>
        [Input("Start Index")]
        [RequiredArgument]
        [Default("0")]
        public InArgument<int> StartIndex { get; set; }

        /// <summary>
        /// Length of substring to be returned
        /// </summary>
        [Input("Length")]
        [RequiredArgument]
        [Default("3")]
        public InArgument<int> Length { get; set; }
    }
}
