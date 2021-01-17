using MathExpressionParser.Core.Constants;
using MathExpressionParser.Core.Domain;
using System.Collections.Generic;

namespace MathExpressionParser.Core.Algorithm
{
    internal static class CellMerger
    {
        internal static bool CanMergeCells(Cell leftCell, Cell rightCell)
        {
            return leftCell.GetPriority() >= rightCell.GetPriority();
        }

        internal static void MergeCells(Cell leftCell, Cell rightCell)
        {
            switch (leftCell.Action)
            {
                case CellConstants.multiplySymb:
                    leftCell.Value *= rightCell.Value;
                    break;
                case CellConstants.divideSymb:
                    leftCell.Value /= rightCell.Value;
                    break;
                case CellConstants.addSymb:
                    leftCell.Value += rightCell.Value;
                    break;
                case CellConstants.substractSymb:
                    leftCell.Value -= rightCell.Value;
                    break;
            }
            leftCell.Action = rightCell.Action;
        }

        internal static double Merge(Cell current, ref int index, List<Cell> listToMerge,
                    bool mergeOneOnly = false)
        {
            while (index < listToMerge.Count)
            {
                Cell next = listToMerge[index++];
                while (!CanMergeCells(current, next))
                {
                    Merge(next, ref index, listToMerge, true /* mergeOneOnly */);
                }
                MergeCells(current, next);
                if (mergeOneOnly)
                {
                    return current.Value;
                }
            }
            return current.Value;
        }
    }
}
