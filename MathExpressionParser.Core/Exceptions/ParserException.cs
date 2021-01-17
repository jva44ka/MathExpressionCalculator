using System;

namespace MathExpressionParser.Core.Exceptions
{
    //исключение для ошибок для анализатора
    public class ParserException : ApplicationException
    {
        public ParserException(string str) : base(str) { }
        public override string ToString()
        { 
            return Message; 
        }
    }
}
