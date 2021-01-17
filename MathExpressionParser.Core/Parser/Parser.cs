using MathExpressionParser.Core.Enums;
using MathExpressionParser.Core.Exceptions;
using MathExpressionParser.Core.Extensions;
using System;

namespace MathExpressionParser.Core.Parser
{
    internal class Parser
    {
        string exp; // Ссылка на строку выражения,
        int expIdx; // Текущий индекс в выражении,
        string token; // Текущая лексема.
        LexTypes tokType; // Тип лексемы.

        // Массив для переменных,
        double[] vars = new double[26];

        public Parser()
        {
            // Инициализируем переменные нулевыми значениями.
            for (int i = 0; i < vars.Length; i++)
                vars[i] = 0.0;
        }

        // Входная точка анализатора.
        public double Evaluate(string expstr)
        {
            double result;
            exp = expstr;
            expIdx = 0;
            try
            {
                GetToken();
                if (token == "")
                    throw new ParserException(Errors.NOEXP.AsString()); // Выражение отсутствует,

                EvalExp1(out result); // В этом варианте анализатора
                                      // сначала вызывается
                                      // метод EvalExpl().
                if (token != "") // Последняя лексема должна
                                 // быть нулевой.
                    throw new ParserException(Errors.SYNTAX.AsString());
                return result;
            }
            catch (ParserException exc)
            {
                Console.WriteLine(exc);
                return 0.0;
            }
        }

        // Обрабатываем присвоение,
        void EvalExp1(out double result)
        {
            int varIdx;
            LexTypes ttokType;
            string temptoken;
            if (tokType == LexTypes.VARIABLE)
            {
                // Сохраняем старую лексему,
                temptoken = token;
                ttokType = tokType;
                // Вычисляем индекс переменной,
                varIdx = Char.ToUpper(token[0]) - 'A';
                GetToken();
                if (token != "=")
                {
                    PutBack();// Возвращаем текущую лексему в поток
                              //и восстанавливаем старую,
                              // поскольку отсутствует присвоение.
                    token = temptoken;
                    tokType = ttokType;
                }
                else
                {
                    GetToken();// Получаем следующую часть
                               // выражения ехр.
                    EvalExp2(out result);
                    vars[varIdx] = result;
                    return;
                }
            }

            EvalExp2(out result);
        }

        // Складываем или вычитаем два члена выражения.
        void EvalExp2(out double result)
        {
            string op;
            double partialResult;

            EvalExp3(out result);
            while ((op = token) == "+" || op == "-")
            {
                GetToken();
                EvalExp3(out partialResult);
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
        void EvalExp3(out double result)
        {
            string op;
            double partialResult = 0.0;
            EvalExp5(out result);
            while ((op = token) == "*" || op == "/")
            {
                GetToken();
                EvalExp5(out partialResult);
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

        // выполняем возведение в степень
        /*void EvalExp4(out double result)
        {
            double partialResult, ex;
            int t;
            EvalExp5(out result);
            if (token == "^")
            {
                GetToken();
                EvalExp4(out partialResult);
                ex = result;
                if (partialResult == 0.0)
                {
                    result = 1.0;
                    return;
                }
                for (t = (int)partialResult - 1; t > 0; t--)
                    result = result * (double)ex;
            }
        }*/

        // Выполненяем операцию унарного + или -.
        void EvalExp5(out double result)
        {
            string op;

            op = "";
            if ((tokType == LexTypes.DELIMITER) && token == "+" || token == "-")
            {
                op = token;
                GetToken();
            }
            EvalExp6(out result);
            if (op == "-") result = -result;
        }

        // обрабатываем выражение в круглых скобках
        void EvalExp6(out double result)
        {
            if ((token == "("))
            {
                GetToken();
                EvalExp2(out result);
                if (token != ")")
                    throw new ParserException(Errors.UNBALPARENS.AsString());
                GetToken();
            }
            else Atom(out result);
        }

        // Получаем значение числа или переменной.
        void Atom(out double result)
        {
            switch (tokType)
            {
                case LexTypes.NUMBER:
                    try
                    {
                        result = Double.Parse(token);
                    }
                    catch (FormatException)
                    {
                        result = 0.0;
                        throw new ParserException(Errors.SYNTAX.AsString());
                    }
                    GetToken();
                    return;
                case LexTypes.VARIABLE:
                    result = FindVar(token);
                    GetToken();
                    return;
                default:
                    result = 0.0;
                    throw new ParserException(Errors.SYNTAX.AsString());
            }
        }

        // Возвращаем значение переменной.
        double FindVar(string vname)
        {
            if (!Char.IsLetter(vname[0]))
                throw new ParserException(Errors.SYNTAX.AsString());

            return vars[Char.ToUpper(vname[0]) - 'A'];
        }

        // Возвращаем лексему во входной поток.
        void PutBack()
        {
            for (int i = 0; i < token.Length; i++) expIdx--;
        }

        // получем следующую лексему.
        void GetToken()
        {
            tokType = LexTypes.NONE;
            token = "";
            if (expIdx == exp.Length) return; // Конец выражения.
                                              // Опускаем пробел.
            while (expIdx < exp.Length && char.IsWhiteSpace(exp[expIdx])) ++expIdx;
            // Хвостовой пробел завершает выражение.
            if (expIdx == exp.Length) return;
            if (IsDelim(exp[expIdx]))
            {
                token += exp[expIdx];
                expIdx++;
                tokType = LexTypes.DELIMITER;
            }
            else if (Char.IsLetter(exp[expIdx]))
            {
                // Это переменная?
                while (!IsDelim(exp[expIdx]))
                {
                    token += exp[expIdx];
                    expIdx++;
                    if (expIdx >= exp.Length) break;
                }
                tokType = LexTypes.VARIABLE;
            }
            else if (Char.IsDigit(exp[expIdx]))
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
        // если с -- разделитель.
        bool IsDelim(char c)
        {
            if (("+-/*=()".IndexOf(c) != -1))
                return true;
            return false;
        }
    }
}
