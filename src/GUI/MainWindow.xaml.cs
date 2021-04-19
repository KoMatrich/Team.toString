using System;
using System.Windows;
using System.Windows.Controls;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DigitButton_Click(object sender, RoutedEventArgs e)
        {
            string digit = ((Button)sender).Tag.ToString();

            if (expressionField.Text == "0") {
                expressionField.Text = "";
            }

            expressionField.Text += digit;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (expressionField.Text.Length > 1) {
                expressionField.Text = expressionField.Text[0..^1];
            }
            else {
                expressionField.Text = "0";
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            expressionField.Text = "0";
        }

        private void PrefixOperator_Click(object sender, RoutedEventArgs e)
        {
            if (expressionField.Text == "0") {
                expressionField.Text = "";
            }

            expressionField.Text += (sender as Button)?.Tag;
        }

        private void InfixOperator_Click(object sender, RoutedEventArgs e)
        {
            expressionField.Text += (sender as Button)?.Tag;
        }

        private void PostfixOperator_Click(object sender, RoutedEventArgs e)
        {
            expressionField.Text += (sender as Button)?.Tag;
        }

        private void Evaluate_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
