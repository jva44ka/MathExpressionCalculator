using MathExpressionParser.Core.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace MathExpressionParser.Core.Domain
{
    internal class ParserFunction
    {
        private ParserFunction _impl;

        private static IdentityFunction _idFunction;
        private static StrtodFunction _strtodFunction;
        private static Dictionary<string, ParserFunction> _functions =
            new Dictionary<string, ParserFunction>();

        internal ParserFunction()
        {
            _impl = this;
        }

        internal ParserFunction(string data, ref int from,
                                string item, char ch)
        {
            if (item.Length == 0 && ch == CellConstants.startSymb)
            {
                // Функции нет — только выражение в скобках
                _impl = _idFunction;
                return;
            }

            if (_functions.TryGetValue(item, out _impl))
            {
                // Функция есть, и она зарегистрирована
                // (например, pi, exp и т. д.)
                return;
            }

            // Функция не найдена, попытаемся разобрать это как число
            _strtodFunction.Item = item;
            _impl = _strtodFunction;
        }

        internal static void AddFunction(string name, ParserFunction function)
        {
            _functions[name] = function;
        }

        public double GetValue(string data, ref int from)
        {
            return _impl.Evaluate(data, ref from);
        }

        protected virtual double Evaluate(string data, ref int from)
        {
            // Настоящая реализация будет в производных классах
            return 0;
        }
    }
}
