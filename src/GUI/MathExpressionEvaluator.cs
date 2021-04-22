using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GUI
{
    public record CalculationResult(string Result, CalculationStatus Status);
    public enum CalculationStatus { Ok, DivisionByZero, Overflow, InvalidExpression }

    public class MathExpressionEvaluator
    {
        private static readonly CultureInfo culture = CultureInfo.GetCultureInfo("cs-CZ");

        public CalculationResult EvaluateExpression(string inputExpression)
        {
            string expr = inputExpression;

            System.Diagnostics.Debug.WriteLine($"evaluating expression '{expr}'");

            // implicit multiplication, "2e" => "2 * e", "2√9" => "2 * √9"
            expr = Regex.Replace(expr, @"([0-9]+)(?=[a-zA-Zπ√⎷])", "$1 * ");

            expr = Regex.Replace(expr, @"\b(π)\b", ' ' + Math.PI.ToString(culture) + ' ');
            expr = Regex.Replace(expr, @"\b(e)\b", ' ' + Math.E.ToString(culture) + ' ');

            // to fix binary minus sometimes parsed as unary minus (e.g. in "2-2^3")
            expr = Regex.Replace(expr, @"(\d)-(\d)", "$1 - $2");

            try {
                expr = evaluatePostfixOperations(expr, @"!");
                expr = evaluateInfixOperations(expr, @"\*\*|\^", rightAssociative: true);
                expr = evaluatePrefixOperations(expr, @"[√⎷]");
                expr = evaluateInfixOperations(expr, @"[×*·÷/]");
                expr = evaluatePrefixOperations(expr, @"-");
                expr = evaluateInfixOperations(expr, @"\+|-");
                expr = evaluatePrefixOperations(expr, @"abs ");
                expr = evaluateInfixOperations(expr, @" mod | root ");
            }
            catch (DivideByZeroException) {
                return new CalculationResult(Result: null, CalculationStatus.DivisionByZero);
            }
            catch (OverflowException) {
                // note: operators on Decimal are always checked for overflow, no `checked` keyword needed
                return new CalculationResult(Result: null, CalculationStatus.Overflow);
            }

            if (!decimal.TryParse(expr, NumberStyles.Float, culture, out decimal result)) {
                return new CalculationResult(Result: null, CalculationStatus.InvalidExpression);
            }

            return new CalculationResult(result.ToString("0.############", culture), CalculationStatus.Ok);

            static string evaluatePrefixOperations(string input, string unaryOperatorsPattern)
            {
                return repeatedRegexReplace(input,
                    findPattern: $@"(?'op'{unaryOperatorsPattern})\s*{numberPattern("arg")}",
                    evaluator: evaluateUnaryExpressionMatch);
            }

            static string evaluatePostfixOperations(string input, string unaryOperatorsPattern)
            {
                return repeatedRegexReplace(input,
                    findPattern: $@"{numberPattern("arg")}\s*(?'op'{unaryOperatorsPattern})",
                    evaluator: evaluateUnaryExpressionMatch);
            }

            static string evaluateInfixOperations(string input, string binaryOperatorsPattern, bool rightAssociative = false)
            {
                return repeatedRegexReplace(input,
                    findPattern: $@"{numberPattern("left")}\s*(?'op'{binaryOperatorsPattern})\s*{numberPattern("right")}",
                    evaluator: evaluateBinaryExpressionMatch,
                    options: rightAssociative ? RegexOptions.RightToLeft : default);
            }

            static string numberPattern(string groupName)
                => $@"(?:(?'{groupName}'-?\b[0-9]+(?:,[0-9]+)?)\b)";

            static string repeatedRegexReplace(
                string input, string findPattern, MatchEvaluator evaluator, RegexOptions options = default)
            {
                string output = input;
                string previousOutput = output;
                do {
                    previousOutput = output;
                    System.Diagnostics.Debug.WriteLine(output);
                    output = Regex.Replace(output, findPattern, evaluator, options);
                }
                while (output != previousOutput);
                return output;
            }

            static string evaluateBinaryExpressionMatch(Match match)
            {
                decimal? left = getOperand(match, "left");
                decimal? right = getOperand(match, "right");
                string @operator = getGroupTextOrNull(match, "op");

                if (left == null || right == null || @operator == null)
                    throw new FormatException($"invalid expression part ({match.Value}):  left={left}  right={right}  op={@operator}");

                return ApplyOperator(@operator, left.Value, right.Value).ToString(culture);
            }

            static string evaluateUnaryExpressionMatch(Match match)
            {
                decimal? arg = getOperand(match, "arg");
                string @operator = getGroupTextOrNull(match, "op");

                if (arg == null || @operator == null)
                    throw new FormatException($"invalid expression part ({match.Value}):  arg={arg}  op={@operator}");

                return ApplyOperator(@operator, arg.Value).ToString(culture);
            }

            static decimal? getOperand(Match match, string groupName)
            {
                return (getGroupTextOrNull(match, groupName) is string operandText &&
                    decimal.TryParse(operandText, out decimal operandNumber))
                    ? operandNumber
                    : null;
            }

            static string getGroupTextOrNull(Match match, string groupName)
                => match.Groups[groupName] is Group { Success: true } group ? group.Value : null;
        }

        private static decimal ApplyOperator(string @operator, decimal left, decimal right)
        {
            return @operator switch {
                "^" or "**" => MathLib.MyMath.Pow(left, right),
                "*" or "×" or "·" => MathLib.MyMath.Mult(left, right),
                "/" or "÷" => MathLib.MyMath.Div(left, right),
                "+" => MathLib.MyMath.Add(left, right),
                "-" => MathLib.MyMath.Sub(left, right),
                " mod " => MathLib.MyMath.Mod(left, right),
                " root " => MathLib.MyMath.Root(right, left),
                _ => throw new ArgumentException($"unknown operator '{@operator}'", nameof(@operator)),
            };
        }

        private static decimal ApplyOperator(string @operator, decimal arg)
        {
            return @operator switch {
                "√" or "⎷" => MathLib.MyMath.Sqrt(arg),
                "abs " => MathLib.MyMath.Abs(arg),
                "-" => MathLib.MyMath.Sub(0, arg),
                "!" => MathLib.MyMath.Fact(arg),
                _ => throw new ArgumentException($"unknown operator '{@operator}'", nameof(@operator)),
            };
        }
    }
}
