using MathExpressionParser.Core.Constants;
using MathExpressionParser.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MathExpressionParser.Core.Algorithm
{
    internal class CellCalculator
    {
        internal static double LoadAndCalculate(string data, ref int from,
                                                char to = CellConstants.endSymb)
        {
            if (from >= data.Length || data[from] == to)
            {
                throw new ArgumentException(
                  "Loaded invalid data: " + data);
            }
            List<Cell> listToMerge = new List<Cell>(16);
            StringBuilder item = new StringBuilder();

            do // основной цикл обработки на первом этапе
            {
                char ch = data[from++];
                if (StillCollecting(item.ToString(), ch, to))
                { // символ все еще относится к предыдущему операнду
                    item.Append(ch);
                    if (from < data.Length && data[from] != to)
                    {
                        continue;
                    }
                }

                // Я получил следующую лексему. Вызов getValue() ниже может
                // рекурсивно вызывать loadAndCalculate(). Это произойдет,
                // если извлеченный элемент является функцией или если
                // следующий элемент начинается с START_ARG '('.
                ParserFunction func = new ParserFunction(
                  data, ref from, item.ToString(), ch);
                double value = func.GetValue(data, ref from);

                char action = CellUtility.ValidAction(ch) ? ch :
                  UpdateAction(data, ref from, ch, to);
                listToMerge.Add(new Cell(value, action));
                item.Clear();

            } while (from < data.Length && data[from] != to);

            if (from < data.Length && (data[from] == CellConstants.endSymb ||
              data[from] == to))
            { // это происходит при рекурсивном вызове:
              // перемещение на один символ вперед
                from++;
            }

            Cell baseCell = listToMerge[0];
            int index = 1;

            return CellMerger.Merge(baseCell, ref index, listToMerge);
        }

        internal static char UpdateAction(string item, ref int from,
  char ch, char to)
        {
            if (from >= item.Length || item[from] == CellConstants.endSymb ||
              item[from] == to)
            {
                return CellConstants.endSymb;
            }

            int index = from;
            char res = ch;
            while (!CellUtility.ValidAction(res) && index < item.Length)
            { // смотрим на следующий символ в строке,
              // пока не найдем допустимое действие
                res = item[index++];
            }

            from = CellUtility.ValidAction(res) ? index : index > from ? index – 1 : from;
            return res;
        }

        private static bool StillCollecting(string item, char ch, char to)
        {
            char stopCollecting = (to == CellConstants.endSymb || to == END_LINE) ?
                                   END_ARG : to;
            return (item.Length == 0 && (ch == '-' || ch == END_ARG)) ||
                  !(ValidAction(ch) || ch == START_ARG || ch == stopCollecting);
        }
    }
}
