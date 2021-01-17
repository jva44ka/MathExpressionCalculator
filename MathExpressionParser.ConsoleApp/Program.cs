using System;

namespace MathExpressionParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitSymbAsString = "q";
            string inputExpression = string.Empty;

            var calculator = new Calculator();
            double result = default;

            Console.WriteLine("Введите \"q\" если захотите выйти \nВведите выражение");
            inputExpression = Console.ReadLine();

            while (inputExpression != exitSymbAsString)
            {
                result = calculator.CalculateExpression(inputExpression);
                Console.WriteLine("Результат: " + result);

                Console.WriteLine("Введите выражение");
                inputExpression = Console.ReadLine();
            }
        }
    }
}
