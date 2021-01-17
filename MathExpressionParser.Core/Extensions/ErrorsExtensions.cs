using MathExpressionParser.Core.Enums;
using System;

namespace MathExpressionParser.Core.Extensions
{
    internal static class ErrorsExtensions
    {
        internal static string AsString(this Errors error)
        {
            switch(error)
            {
                case Errors.SYNTAX:
                    return "Синтаксическая оошибка";
                case Errors.UNBALPARENS:
                    return "Дисбаланс скобок";
                case Errors.NOEXP:
                    return "Выражение отсутствет";
                case Errors.DIVBYZERO:
                    return "Деление на нуль";
                default:
                    throw new ArgumentException("Неизвестный вид ошибки");
            }
        }
    }
}
