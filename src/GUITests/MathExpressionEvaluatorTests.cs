using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using GUI;
using System.Globalization;

namespace GUI.Tests
{
    [TestClass]
    public class MathExpressionEvaluatorTests
    {
        [DataTestMethod]
        [DataRow("0", "0", DisplayName = "zero passthrough")]
        [DataRow("1,123", "1,123", DisplayName = "decimal number passthrough")]
        [DataRow("  07  ", "7", DisplayName = "should strip whitespace and leading zeros")]
        [DataRow("07,020300", "7,0203", DisplayName = "should strip leading and trailing (decimal) zeros")]
        [DataRow("3+20", "23", DisplayName = "addition")]
        [DataRow("3-20", "-17", DisplayName = "subtraction")]
        [DataRow("1,001+0,123", "1,124", DisplayName = "decimal addition")]
        [DataRow("21 mod 5", "1", DisplayName = "modulo")]
        [DataRow("abs 10", "10", DisplayName = "abs of positive constant")]
        [DataRow("abs 4-14", "10", DisplayName = "abs of negative expr")]
        [DataRow("1/1000", "0,001", DisplayName = "division")]
        [DataRow("1000^-1", "0,001", DisplayName = "reciprocal")]
        [DataRow("√49", "7", DisplayName = "square root")]
        [DataRow("3 root 125", "5", DisplayName = "nth root")]
        [DataRow("5!", "120", DisplayName = "factorial")]
        [DataRow("3,22349651679039!", "8", DisplayName = "decimal factorial")]
        public void EvaluateExpression_GivenSimpleValidExpression_CalculatesCorrectResult(
            string expr, string expected)
        {
            MathExpressionEvaluator evaluator = new();

            var (actual, status) = evaluator.EvaluateExpression(expr);
            Assert.AreEqual(CalculationStatus.Ok, status);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("1,234", "1,234", "en-US")]
        [DataRow("1,234", "1,234", "fr-FR")]
        [DataRow("5/2", "2,5", "en-GB")]
        public void EvaluateExpression_GivenValidExpressionInDifferentSystemCultures_CalculatesCorrectResult(
            string expr, string expected, string cultureName)
        {
            var oldCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
            MathExpressionEvaluator evaluator = new();

            var (actual, status) = evaluator.EvaluateExpression(expr);
            CultureInfo.CurrentCulture = oldCulture;

            Assert.AreEqual(CalculationStatus.Ok, status);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("3 * -11 + 4", "-29")]
        [DataRow("-1+0,5^-2*3", "11")]
        [DataRow("7-3^2", "-2")]
        [DataRow("7-√16^2", "-9")]
        [DataRow("-20 mod -2^4", "-4")]
        [DataRow("√9 * 5", "15")]
        [DataRow("√9√121", "33")]
        [DataRow("-2√9", "-6")]
        [DataRow("√3^2", "3")]
        [DataRow("2^4^2", "65536")]
        [DataRow("3!!", "720")]
        [DataRow("3!^3!", "46656")]
        public void EvaluateExpression_GivenComplexValidExpression_CalculatesCorrectResult(
            string expr, string expected)
        {
            MathExpressionEvaluator evaluator = new();

            var (actual, status) = evaluator.EvaluateExpression(expr);
            Assert.AreEqual(CalculationStatus.Ok, status);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("1/0", CalculationStatus.DivisionByZero)]
        [DataRow("0,5^-2/-0", CalculationStatus.DivisionByZero)]
        [DataRow("7-16^^2", CalculationStatus.InvalidExpression)]
        [DataRow("7-+2", CalculationStatus.InvalidExpression)]
        [DataRow("**3", CalculationStatus.InvalidExpression)]
        [DataRow("4//8//9", CalculationStatus.InvalidExpression)]
        [DataRow("1 8", CalculationStatus.InvalidExpression)]
        [DataRow("100!!", CalculationStatus.Overflow)]
        [DataRow("8^7^6", CalculationStatus.Overflow)]
        public void EvaluateExpression_GivenInvalidExpression_ReturnsError(
            string expr, CalculationStatus expectedStatus)
        {
            MathExpressionEvaluator evaluator = new();

            CalculationStatus status = evaluator.EvaluateExpression(expr).Status;
            Assert.AreEqual(expectedStatus, status);
        }
    }
}
