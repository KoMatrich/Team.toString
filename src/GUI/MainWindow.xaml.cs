using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace GUI
{
    /**
     * Hlavní okno kalkulačky
     */
    public partial class MainWindow : Window
    {
        // A tag used to mark parts of expressions (Runs in TextField) that should be
        // removed with a single press of DEL, not per character, like "mod" or "abs"
        const string AtomicRunTag = "(atomic)";

        MathExpressionEvaluator exprEvaluator = new();
        (string displayText, CalculationStatus status) lastCalculationResult;

        // true iff the last button pressed was `=`, so the expression field
        // currently contains the result of the calculation (which can be an error)
        private bool CurrentlyShowingResult
            => expressionField.Text == lastCalculationResult.displayText;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DigitButton_Click(object sender, RoutedEventArgs e)
        {
            string digit = ((Button)sender).Tag.ToString();
            TypeDigit(digit);
        }

        private void TypeDigit(string digit)
        {
            if (CurrentlyShowingResult) {
                expressionField.Text = "";
            }

            AppendText(digit);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            DeleteToTheLeft();
        }

        /**
         * Vymaže poslední znak v poli výrazů, případně poslední @c Inline,
         * pokud má @c Tag nastavený na @c AtomicRunTag
         */
        private void DeleteToTheLeft()
        {
            PrepareBeforeInput();

            // TextField content can be split into multiple Inlines (usually Runs),
            // which can be used to position or style different parts differently (e.g. overline for sqrt)

            if (expressionField.Text.Length > 0) {
                var lastInline = GetExpressionParts().LastInline;
                bool atomic = lastInline.Tag as string == AtomicRunTag;

                if (!atomic) {
                    lastInline.ContentEnd.DeleteTextInRun(-1);
                }

                bool emptyInline = lastInline.ContentEnd.CompareTo(lastInline.ContentStart) <= 0;
                if (atomic || emptyInline) {
                    expressionField.Inlines.Remove(lastInline);
                }
            }
        }

        private void PrependText(string text)
        {
            PrepareBeforeInput();
            GetExpressionParts().FirstInline.ContentStart.InsertTextInRun(text);
        }

        private void AppendText(string text)
        {
            PrepareBeforeInput();
            GetExpressionParts().LastInline.ContentEnd.InsertTextInRun(text);
        }

        /**
         * Nahradí obsah pole výrazů částmi specifikovanými v @p parts.
         */
        private void SetExpressionParts(params Inline[] parts)
        {
            PrepareBeforeInput();
            expressionField.Inlines.Clear();
            expressionField.Inlines.AddRange(parts);
        }

        /** 
         * Vrátí všechny části pole výrazů. 
         * Např. <tt>{ Run("4"), Run(" mod "), Run("1+2") }</tt>
         */
        private InlineCollection GetExpressionParts()
        {
            if (expressionField.Inlines.Count == 0)
                expressionField.Inlines.Add("");
            return expressionField.Inlines;
        }

        private void Clear_Click(object sender, RoutedEventArgs e) => ClearAll();

        private void ClearAll()
        {
            // Restore everything to default state
            expressionField.Text = "";
            previousComputation.Text = "";
            previousComputationRow.Height = new GridLength(0);
            lastCalculationResult = default;
        }

        private void PrefixOperator_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentlyShowingResult) {
                PrependText((string)((sender as Button)?.Tag));
            }
            else {
                AppendText((string)((sender as Button)?.Tag));
            }
        }

        private void InfixOperator_Click(object sender, RoutedEventArgs e)
        {
            AppendText((string)((sender as Button)?.Tag));
        }

        private void PostfixOperator_Click(object sender, RoutedEventArgs e)
        {
            AppendText((string)((sender as Button)?.Tag));
        }

        private void PowerOfTen_Click(object sender, RoutedEventArgs e)
        {
            AppendText(string.IsNullOrWhiteSpace(expressionField.Text) ? "10^" : "·10^");
        }

        private void Reciprocal_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(expressionField.Text)) {
                PrependText("1/");
                return;
            }

            EvaluateAndModify(actionIfOk: x => SetExpressionParts(
                new Run("1/") { Tag = AtomicRunTag },
                new Run(x),
                new Run("")
            ));
        }

        private void Abs_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(expressionField.Text)) {
                SetExpressionParts(
                    new Run("abs ") { Tag = AtomicRunTag },
                    new Run("")
                );
                return;
            }

            EvaluateAndModify(actionIfOk: x => SetExpressionParts(
                new Run("abs ") { Tag = AtomicRunTag },
                new Run(x)
            ));
        }

        private void Mod_Click(object sender, RoutedEventArgs e)
        {
            EvaluateAndModify(actionIfOk: x => SetExpressionParts(
                new Run(x),
                new Run(" mod ") { Tag = AtomicRunTag },
                new Run("")
            ));
        }

        private void Negate_Click(object sender, RoutedEventArgs e)
        {
            EvaluateAndModify(actionIfOk: x => SetExpressionParts(
                new Run("-"),
                new Run(x)
            ));
        }

        private void Evaluate_Click(object sender, RoutedEventArgs e)
            => Evaluate();

        private void Sqrt_Click(object sender, RoutedEventArgs e)
        {
            EvaluateAndModify(actionIfOk: x => SetExpressionParts(
                new Run("√") { FontSize = expressionField.FontSize * 1.3 },
                new Run(text: x) { TextDecorations = { TextDecorations.OverLine } },
                new Run(" ")
            ));
        }

        private void NthRoot_Click(object sender, RoutedEventArgs e)
        {
            EvaluateAndModify(actionIfOk: n => SetExpressionParts(
                new Run(text: n) {
                    BaselineAlignment = BaselineAlignment.Superscript,
                },
                //new Run(text: x) { Typography = { Variants = FontVariants.Superscript } },
                new Run(" root ") { Tag = AtomicRunTag, ToolTip = $"n-tá odmocnina (n={n})" },
                new Run("")
            ));
        }

        private void PrepareBeforeInput()
        {
            // Clear the expression field if the previous computation resulted in an error
            if (CurrentlyShowingResult && lastCalculationResult.status != CalculationStatus.Ok) {
                ClearAll();
            }
            // Clear the last calculation result since we are modifying the expression.
            // If we didn't do this, typing e.g. "7" when the expr. field is "7"
            // when previous computation result was "7" would never do anything.
            lastCalculationResult = default;
        }

        /**
         * Vyhodnotí aktuálně zadaný výraz a výsledek (případně chybu) vloží zpět do pole výrazů. 
         * Výsledek i status se také vloží do @c lastCalculationResult.
         */
        private void Evaluate()
        {
            previousComputation.Text = expressionField.Text;
            previousComputationRow.Height = new GridLength(1, GridUnitType.Star);

            CalculationResult result = exprEvaluator.EvaluateExpression(expressionField.Text);
            expressionField.Text = result.Status switch {
                CalculationStatus.DivisionByZero => "Nelze dělit nulou",
                CalculationStatus.Overflow => "Příliš velké číslo",
                CalculationStatus.InvalidExpression => "Chyba",
                _ => result.Result
            };
            lastCalculationResult = (displayText: expressionField.Text, status: result.Status);
        }

        /**
         * Vyhodnotí aktuálně zadaný výraz a v případě úspěchu spustí předanou akci.
         *
         * @param actionIfOk Akce, která se spustí, pokud při výpočtu nenastala chyba,
         *                   a jako argument je předán výsledek jako text.
         */
        private void EvaluateAndModify(Action<string> actionIfOk)
        {
            if (CurrentlyShowingResult && lastCalculationResult.status != CalculationStatus.Ok) {
                return;
            }

            Evaluate();
            if (lastCalculationResult.status == CalculationStatus.Ok) {
                actionIfOk(lastCalculationResult.displayText);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Back) {
                DeleteToTheLeft();
                e.Handled = true;
            }
            else if (e.Key is Key.Delete or Key.C) {
                ClearAll();
                e.Handled = true;
            }
            else if (e.Key is Key.P) {
                TypeDigit("π");
                e.Handled = true;
            }
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            const string directlyEnterableCharacters = "+-*/÷×·!,πe^";

            foreach (char key in e.Text) {
                if (char.IsDigit(key)) {
                    TypeDigit(key.ToString());
                    e.Handled = true;
                }
                else if (directlyEnterableCharacters.Contains(key)) {
                    AppendText(key.ToString());
                    e.Handled = true;
                }
                else if (key == '.') {
                    AppendText(",");
                    e.Handled = true;
                }
            }
        }
    }
}
