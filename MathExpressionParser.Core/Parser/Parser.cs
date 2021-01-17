using MathExpressionParser.Core.Enums;
using MathExpressionParser.Core.Exceptions;
using MathExpressionParser.Core.Extensions;
using System;

namespace MathExpressionParser.Core.Parser
{
    /*
     * Синтаксический анализатор методом рекурсивного спуска
     */
    internal class Parser
    {
        string exp; // Ссылка на строку выражения,
        int expIdx; // Текущий индекс в выражении,
        string token; // Текущая лексема.
        LexTypes tokType; // Тип лексемы.

        // Входная точка анализатора.
        internal double Evaluate(string expstr)
        {
            double result;
            exp = expstr;
            expIdx = 0;
            try
            {
                GetToken();

                // Выражение отсутствует
                if (token == "")
                    throw new ParserException(Errors.NOEXP.AsString());

                AddOrSubStep(out result);

                //Последняя лексема должна быть нулевой
                if (token != "")
                    throw new ParserException(Errors.SYNTAX.AsString());

                return result;
            }
            catch (ParserException exc)
            {
                Console.WriteLine(exc);
                return 0.0;
            }
        }

        // Складываем или вычитаем два члена выражения.
        private void AddOrSubStep(out double result)
        {
            string op;
            double partialResult;

            MulOrDivStep(out result);
            while ((op = token) == "+" || op == "-")
            {
                GetToken();
                MulOrDivStep(out partialResult);
                switch (op)
                {
                    case "-":
                        result -= partialResult;
                        break;
                    case "+":
                        result += partialResult;
                        break;
                }
            }
        }

        // Выполняем умножение или деление двух множителей.
        private void MulOrDivStep(out double result)
        {
            string op;
            double partialResult = 0.0;
            UnarPlusOrMinusStep(out result);
            while ((op = token) == "*" || op == "/")
            {
                GetToken();
                UnarPlusOrMinusStep(out partialResult);
                switch (op)
                {
                    case "*":
                        result *= partialResult;
                        break;
                    case "/":
                        if (partialResult == 0.0)
                            throw new ParserException(Errors.DIVBYZERO.AsString());
                        result /= partialResult;
                        break;
                }
            }
        }

        // Выполненяем операцию унарного + или -.
        private void UnarPlusOrMinusStep(out double result)
        {
            string op;

            op = "";
            if ((tokType == LexTypes.DELIMITER) && token == "+" || token == "-")
            {
                op = token;
                GetToken();
            }
            BracketsStep(out result);
            if (op == "-") result = -result;
        }

        // Обрабатываем выражение в круглых скобках
        private void BracketsStep(out double result)
        {
            if (token == "(")
            {
                GetToken();
                AddOrSubStep(out result);
                if (token != ")")
                    throw new ParserException(Errors.UNBALPARENS.AsString());
                GetToken();
            }
            else NumberHandle(out result);
        }

        // Получаем значение числа.
        private void NumberHandle(out double result)
        {
            switch (tokType)
            {
                case LexTypes.NUMBER:
                    try
                    {
                        result = double.Parse(token);
                    }
                    catch (FormatException)
                    {
                        result = 0.0;
                        throw new ParserException(Errors.SYNTAX.AsString());
                    }
                    GetToken();
                    return;
                default:
                    result = 0.0;
                    throw new ParserException(Errors.SYNTAX.AsString());
            }
        }

        // Получем следующую лексему.
        private void GetToken()
        {
            tokType = LexTypes.NONE;
            token = "";

            // Конец выражения. Опускаем пробел
            if (expIdx == exp.Length) 
                return;

            while (expIdx < exp.Length && char.IsWhiteSpace(exp[expIdx])) 
                ++expIdx;

            // Хвостовой пробел завершает выражение.
            if (expIdx == exp.Length) 
                return;

            if (IsDelim(exp[expIdx]))
            {
                token += exp[expIdx];
                expIdx++;
                tokType = LexTypes.DELIMITER;
            }
            else if (char.IsDigit(exp[expIdx]))
            {
                // Это число?
                while (!IsDelim(exp[expIdx]))
                {
                    token += exp[expIdx];
                    expIdx++;
                    if (expIdx >= exp.Length) break;
                }
                tokType = LexTypes.NUMBER;
            }
        }

        // Метод возвращает значение true,
        // если с - разделитель.
        private bool IsDelim(char c)
        {
            if ("+-/*()".IndexOf(c) != -1)
                return true;
            return false;
        }
    }
}
