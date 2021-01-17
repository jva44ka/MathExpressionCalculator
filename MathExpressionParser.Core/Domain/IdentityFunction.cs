using System;
using System.Collections.Generic;
using System.Text;

namespace MathExpressionParser.Core.Domain
{
    internal class IdentityFunction : ParserFunction
    {
        protected override double Evaluate(string data, ref int from)
        {
            return Calculator.LoadAndCalculate(data, ref from, Parser.END_ARG);
        }
    }
}
