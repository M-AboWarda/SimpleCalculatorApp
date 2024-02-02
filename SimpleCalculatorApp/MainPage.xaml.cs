using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimpleCalculatorApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Btn_Clicked(object sender, RoutedEventArgs e)
        {
            // cleanup 
            Int64 parsed;
            if (!Int64.TryParse(ResultsBlock.Text, out parsed)) { ResultsBlock.Text = "0"; }
            //if((sender as Button).DataContext == "pressed")
            // 

            String input = (sender as Button).Content.ToString();

            if (Int64.TryParse(input, out parsed)) // number pressed
                NumberHandler(input);
            else  // operation pressed
                OperationHandler(input); 

            UpdateFontSizeToFit(OperationBlock, OperationBlockContainer.ActualWidth * 4 / 5);
            CalculateBtn.DataContext = (input == "=")? "pressed" : ""; 
        }
        private void NumberHandler(string digitText)
        {
            if((OperationBlock.Text.Length > 12 && OperationBlock.Text != "") || (ResultsBlock.Text.Length > 15 && OperationBlock.Text == "" && CalculateBtn.DataContext.ToString() != "pressed")) { return; }// limit
            if(OperationBlock.Text == "") { 
                ResultsBlock.Text = (ResultsBlock.Text == "0" || CalculateBtn.DataContext.ToString() == "pressed")? digitText :  ResultsBlock.Text + digitText;
                UpdateFontSizeToFit(ResultsBlock, ResultsContainer.ActualWidth - 20);// 20 Margin
            }
            else
                OperationBlock.Text += digitText;
        }
        private void OperationHandler(string operation)
        {
            if(OperationBlock.Text == "" || OperationBlock.Text.Length == 1)
            {
                OperationBlock.Text = (operation != "=")? operation : OperationBlock.Text;
            }else {
                Calculate(ResultsBlock.Text, OperationBlock.Text.Substring(1, OperationBlock.Text.Length - 1), OperationBlock.Text.Substring(0, 1));
                OperationBlock.Text = (operation != "=")? operation : "";
            }
        }
        private void Calculate(string a, string b, string operation)
        {
            bool success = false;
            switch (operation)
            {
                case "-":
                    success = TrySubtract(a, b);
                    break;
                case "+":
                    success = TryAdd(a, b);
                    break;
                case "/":
                    success = TryDivide(a, b);
                    break;
                case "x":
                    success = TryMultiply(a, b);
                    break;
            }// Interger OverFlow 9 223 372 036 854 775 807
            if (!success) ResultsBlock.Text = (operation == "/") ? "Illegal Zero Division" : "Interger OverFlow";
            UpdateFontSizeToFit(ResultsBlock, ResultsContainer.ActualWidth - 20);// 20 Margin
        }
        private bool TrySubtract(string a, string b) {
            // overflow detection
            if (Add(Int64.MinValue.ToString(), b) >= Int64.Parse(a)) return false; 
            ResultsBlock.Text = Subtract(a,b).ToString();
            return true;
        }
        private Int64 Subtract(string a, string b) { return (Int64.Parse(a) - Int64.Parse(b)); }
        private bool TryAdd(string a, string b) {
            // overflow detection
            if (Subtract(Int64.MaxValue.ToString(), b) <= Int64.Parse(a)) return false;
            ResultsBlock.Text = Add(a,b).ToString(); 
            return true; 
        }
        private Int64 Add(string a, string b) { return (Int64.Parse(a) + Int64.Parse(b)); }
        private bool TryDivide(string a, string b) { 
            if (Int64.Parse(b) == 0) return false; // illegal zero division
            ResultsBlock.Text = (Int64.Parse(a) / Int64.Parse(b)).ToString(); 
            return true;
        }
        private bool TryMultiply(string a, string b) {
            //( a * b == c ) <=> ( c / b == a ) overflow detection
            if (TryDivide(Multiply(a,b).ToString(),b)) { if (ResultsBlock.Text.ToString() != a) return false; }
            ResultsBlock.Text = Multiply(a,b).ToString();
            return true; 
        }
        private Int64 Multiply(string a, string b) { return (Int64.Parse(a) * Int64.Parse(b)); }

        private void ClearBtn_Clicked(object sender, RoutedEventArgs e)
        {
            TextBlock clean = (OperationBlock.Text != "") ? OperationBlock : ResultsBlock;
            clean.Text = (clean == OperationBlock)? "" : "0";
            CalculateBtn.DataContext = "";
        }
        private void UpdateFontSizeToFit(TextBlock box, double containerWidth, int defaultSize = 40)
        {
            box.FontSize = defaultSize;
            box.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            while (box.DesiredSize.Width > containerWidth && box.FontSize > 8)// minimum font size 8
            {
                box.FontSize = box.FontSize - 1;
                box.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            }
        }
    }
}
