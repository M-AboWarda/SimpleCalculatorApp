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
            // 

            String input = (sender as Button).Content.ToString();

            if (Int64.TryParse(input, out parsed)) // number pressed
                NumberHandler(input);
            else  // operation pressed
                OperationHandler(input); 

            UpdateFontSizeToFit(OperationBlock, OperationBlockContainer.ActualWidth * 4 / 5);
        }
        private void NumberHandler(string digitText)
        {
            if((OperationBlock.Text.Length > 15 && OperationBlock.Text != "") || (ResultsBlock.Text.Length > 15 && OperationBlock.Text == "")) { return; }// limit
            if(OperationBlock.Text == "") { 
                ResultsBlock.Text = (ResultsBlock.Text == "0")? digitText :  ResultsBlock.Text + digitText;
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
                OperationBlock.Text = (operation != "=")? operation : "";}
        }
        private void Calculate(string a, string b, string operation)
        {
            switch (operation)
            {
                case "-":
                    TrySubtract(a, b);
                    break;
                case "+":
                    TryAdd(a, b);
                    break;
                case "/":
                    TryDivide(a, b);
                    break;
                case "x":
                    TryMultiply(a, b);
                    break;
            }
            UpdateFontSizeToFit(ResultsBlock, ResultsContainer.ActualWidth - 20);// 20 Margin
        }
        private bool TrySubtract(string a, string b) { ResultsBlock.Text = (Int64.Parse(a) - Int64.Parse(b)).ToString(); return true; }
        private bool TryAdd(string a, string b) { ResultsBlock.Text = (Int64.Parse(a) + Int64.Parse(b)).ToString(); return true; }
        private bool TryDivide(string a, string b) { ResultsBlock.Text = (Int64.Parse(a) / Int64.Parse(b)).ToString(); return true; }
        private bool TryMultiply(string a, string b) { ResultsBlock.Text = (Int64.Parse(a) * Int64.Parse(b)).ToString(); return true; }

        private void UpdateFontSizeToFit(TextBlock box, double containerWidth, int defaultSize = 40)
        {
            box.FontSize = defaultSize;
            box.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            while(box.DesiredSize.Width > containerWidth && box.FontSize > 8)// minimum font size 8
            {
                box.FontSize = box.FontSize - 1;
                box.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            }
        }

        private void ClearBtn_Clicked(object sender, RoutedEventArgs e)
        {
            TextBlock clean = (OperationBlock.Text != "") ? OperationBlock : ResultsBlock;
            clean.Text = (clean == OperationBlock)? "" : "0";            
        }
    }
}
