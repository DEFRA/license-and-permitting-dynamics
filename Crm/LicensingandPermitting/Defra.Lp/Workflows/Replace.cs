// <copyright file="Replace.cs" company="">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author></author>
// <date>12/4/2017 12:00:54 PM</date>
// <summary>Code activity performs a search and replace on a string.</summary>

using System;
using System.Text;
using System.ServiceModel;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Defra.Lp.Workflows
{


    // Class performs a search and replace on a string  
    public class Replace : WorkFlowActivityBase
    {

        [Input("Text")]
        public InArgument<string> Text { get; set; }

        [Output("Result")]
        public OutArgument<string> Result { get; set; }

        [Input("Old Value")]
        public InArgument<string> Old { get; set; }

        [Input("New Value")]
        public InArgument<string> New { get; set; }

        [Input("Case Sensitive")]
        [Default("False")]
        public InArgument<bool> CaseSensitive { get; set; }


        /// <summary>
        /// Code activity performs a search and replace on a string
        /// </summary>
        /// <param name="executionContext">The code activity context</param>
        /// <param name="crmWorkflowContext">The CRM workflow context</param>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            var tracingService = executionContext.GetExtension<ITracingService>();
            tracingService.Trace("Replace starting...");

            try
            {
                if (crmWorkflowContext == null)
                {
                    throw new ArgumentNullException(nameof(crmWorkflowContext));
                }

                string text = Text.Get<string>(executionContext);
                string old = Old.Get<string>(executionContext);

                string @new = New.Get<string>(executionContext) ?? String.Empty;
                tracingService.Trace("Replace old '{0}' with new '{1}' on text '{3}'", old, @new, text);
                string result = string.Empty;
                if (!CaseSensitive.Get<bool>(executionContext))
                {
                    if (!String.IsNullOrEmpty(text) && !String.IsNullOrEmpty(old))
                    {
                        result = text.Replace(old, @new);
                    }
                }
                else
                {
                    result = CompareAndReplace(text, old, @new, StringComparison.CurrentCultureIgnoreCase);
                }
                Result.Set(executionContext, result);

            }
            catch (Exception ex)
            {
                // Todo: Log the Error
                tracingService.Trace("Exception: " + ex);
                throw ex;
            }
        }


        private static string CompareAndReplace(string text, string old, string @new, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(old))
            {
                return text;
            }

            var result = new StringBuilder();
            var oldLength = old.Length;
            var pos = 0;
            var next = text.IndexOf(old, comparison);

            while (next > 0)
            {
                result.Append(text, pos, next - pos);
                result.Append(@new);
                pos = next + oldLength;
                next = text.IndexOf(old, pos, comparison);
            }

            result.Append(text, pos, text.Length - pos);
            return result.ToString();
        }


    }

}
