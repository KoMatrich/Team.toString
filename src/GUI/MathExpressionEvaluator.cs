using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GUI
{
    /**
     * @class GUI.CalculationResult
     * @brief Výsledek výpočtu - formátovaný výsledek (číslo) a status (chyba).
     */
    public record CalculationResult(string Result, CalculationStatus Status);

    /** Status výpočtu - úspěch, nebo chyba (dělení nulou, přetečení, nebo chyba při zpracování výrazu). */
    public enum CalculationStatus { Ok, DivisionByZero, Overflow, InvalidExpression }

    /** Třída pro výpočet hodnoty matematických výrazů zadaných v textové formě. */
    public class MathExpressionEvaluator
    {
        private static readonly CultureInfo culture = CultureInfo.GetCultureInfo("cs-CZ");

        /**
         * @brief Vyhodnotí daný výraz.
         *
         * @details Podporuje `+`, `-`, `·`, `*`, `×`, `÷`, `/`, dále:
         * - mocniny: `x^n`, `x**n`
         * - druhou odmocninu: `√x`
         * - n-tou odmocninu: `n root x`
         * - unární minus: `-1`
         * - absolutní hodnota: `abs 20-170*2`
         * - modulo: `131 mod 16`
         * - implicitní násobení čísla a konstanty: `2π`
         *
         * Příklad: <tt>EvaluateExpression("1 + 2 * 3π^-2")</tt>
         * 
         * @returns záznam obsahující buď výsledek se statusem Ok, 
         *          nebo výsledek `null` a status označující chybu.
         */
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

        /**
         * Vypočte výraz s binárním operátorem (`a OP b`).
         * 
         * @param operator Binární operátor, jeden z `"+"`, `"-"`, `"/"`, `"÷"`, `"×"`, `"*"`, `"^"`, `"**"`,
         *        `" mod "`, `" root "` (včetně mezer).
         * @param left Levý operand.
         * @param right Pravý operand.
         * @returns Vrací výsledek operace jako číslo Decimal.
         * @throws ArgumentException pokud je použit nepodporovaný operátor.
         */
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

        /**
         * Vypočte výraz s unárním operátorem (`OP a` nebo `a OP`).
         *
         * @param operator Unární operátor, jeden z `"-"`, `"!"`, `"√"`, `"⎷"`, `"abs "` (včetně mezery).
         * @param arg Operand unární operace.
         * @returns výsledek operace jako číslo Decimal.
         * @throws ArgumentException pokud je použit nepodporovaný operátor.
         */
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
