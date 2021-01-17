using MathExpressionParser.Core.Constants;

namespace MathExpressionParser.Core.Domain
{
    internal sealed class Cell
    {
        internal double Value { get; set; }
        internal char Action { get; set; }

        internal Cell(double value, char action)
        {
            Value = value;
            Action = action;
        }

        internal int GetPriority()
        {
            switch (this.Action)
            {
                case CellConstants.multiplySymb: return 4;
                case CellConstants.divideSymb: return 3;
                case CellConstants.addSymb:
                case CellConstants.substractSymb: return 2;
            }
            return 0;
        }
    }
}
