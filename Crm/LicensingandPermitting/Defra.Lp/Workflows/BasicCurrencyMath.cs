// ==================================================================================
//  Project:	Manipulation Library for Microsoft Dynamics CRM 2011
//  File:		BasicMath.cs
//  Summary:	This workflow activity solves basic math equations. 
// ==================================================================================

using System;
using System.Activities;
using Defra.Lp.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Defra.Lp.Workflows
{
    public sealed class BasicCurrencyMath : WorkFlowActivityBase
    {

        /// <summary>
        /// Executes the WorkFlow.
        /// </summary>
        /// <param name="crmWorkflowContext">The <see cref="WorkFlowActivityBase.LocalWorkflowContext"/> which contains the
        /// <param name="executionContext" > <see cref="CodeActivityContext"/>
        /// </param>       
        /// <remarks>
        /// For improved performance, Microsoft Dynamics 365 caches WorkFlow instances.
        /// The WorkFlow's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the WorkFlow. Also, multiple system threads
        /// could execute the WorkFlow at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in WorkFlows.
        /// </remarks>
        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {

            if (executionContext == null)
            {
                throw new ArgumentNullException(nameof(executionContext));
            }

            try
            {
                var tracingService = executionContext.GetExtension<ITracingService>();

                var leftValue = Amount1.Get<Money>(executionContext) ?? new Money();
                var rightValue = Amount2.Get<Money>(executionContext) ?? new Money();
                var symbol = Symbol.Get<string>(executionContext);
                tracingService.Trace("Calculating '{0}' {1} '{2}'", leftValue.Value, symbol, rightValue.Value);
                switch (symbol)
                {
                    case "+":
                        leftValue.Value += rightValue.Value;
                        break;
                    case "-":
                        leftValue.Value -= rightValue.Value;
                        break;
                    case "/":
                        leftValue.Value /= rightValue.Value;
                        break;
                    case "*":
                        leftValue.Value *= rightValue.Value;
                        break;
                    default:
                        throw new ArgumentException($"Symbol {symbol} cannot be processed");
                }

                tracingService.Trace("Result = '{0}'", leftValue.Value);
                Result.Set(executionContext, leftValue);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in Workflow assembly." + ex.Message + ex.StackTrace, ex);
            }
        }


        [Input("Amount1")]
        [Default("0.0")]
        public InArgument<Money> Amount1 { get; set; }


        [Input("Amount2")]
        [Default("0.0")]
        public InArgument<Money> Amount2 { get; set; }

        [Input("Symbol")]
        [Default("+")]
        public InArgument<string> Symbol { get; set; }

        [Output("Result")]
        public OutArgument<Money> Result { get; set; }
    }
}
