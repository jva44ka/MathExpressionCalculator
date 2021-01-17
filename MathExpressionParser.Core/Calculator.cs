using MathExpression.Core;
using TParser = MathExpressionParser.Core.Parser.Parser;

namespace MathExpressionParser.Core
{
    public class Calculator : ICalculator
    {
        private TParser _parser = new TParser();

        public double CalculateExpression(string expression)
        {
            return _parser.Evaluate(expression);
        }
    }
}
