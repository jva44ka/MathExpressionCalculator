using MathExpressionParser.Core.Constants;

namespace MathExpressionParser.Core.Algorithm
{
    internal static class CellUtility
    {
        internal static bool ValidAction(char ch)
        {
            return ch == CellConstants.multiplySymb || 
                ch == CellConstants.divideSymb || 
                ch == CellConstants.addSymb || 
                ch == CellConstants.substractSymb;
        }
    }
}
