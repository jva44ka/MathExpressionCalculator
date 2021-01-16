using MathExpressionParser.Core;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using NUnit.Framework;

namespace MathExpressionParser.Tests
{
    public class Tests
    {
        private static readonly string[] _setForPositiveTests =
        {
            "2 + 2 * 2",
            "3 - 2 + (2 + 5)"
        };

        private readonly Calculator _calculator = new Calculator();

        [SetUp]
        public void Setup()
        { }

        // positive tests
        [Test, TestCaseSource(nameof(_setForPositiveTests))]
        public void PositiveTestSet(string testExpression)
        {
            var parserExpressionRes = _calculator.CalculateExpression(testExpression);
            var rightExpressionRes = CSharpScript.EvaluateAsync<double>(testExpression).Result;
            Assert.AreEqual(parserExpressionRes, rightExpressionRes);
        }
    }
}