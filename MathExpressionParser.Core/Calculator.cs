using MathExpression.Core;
using TParser = MathExpressionParser.Core.Parser.Parser;

namespace MathExpressionParser
{
    // Внешний интерфейс для других проектов в решении
    public class Calculator : ICalculator
    {
        private TParser _parser = new TParser();

        public double CalculateExpression(string expression)
        {
            return _parser.Evaluate(expression);
        }
    }
}
