using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HEC.FDA.View.Utilities
{
    /// <summary>
    /// Interaction logic for NullableNumericTextBox.xaml
    /// </summary>
    public partial class NullableDoubleTextBox : TextBox
    {
        public static readonly DependencyProperty DoubleValueProperty = DependencyProperty.Register(nameof(DoubleValue), typeof(double?), typeof(NullableDoubleTextBox));

        public double? DoubleValue
        {
            get { return (double?)GetValue(DoubleValueProperty); }
            set
            {
                SetValue(DoubleValueProperty, value);
                if (!_updating)
                {
                    UpdateText();
                }
            }
        }

        private bool _updating;

        public NullableDoubleTextBox()
        {
            InitializeComponent();
        }

        private void UpdateText()
        {
            //double?::ToString returns a string?, so we need to account for that.
            Text = DoubleValue?.ToString() ?? "";
        }

        private void MyPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
            string value = CombineText(e.Text);

            //Regex regex = new Regex("[^0-9.-]+");
            //e.Handled = regex.IsMatch(e.Text);

            if (double.TryParse(value, out double result))
            //if(regex.IsMatch(e.Text))
            {
                e.Handled = false;
                _updating = true;
                DoubleValue = result;
                _updating = false;
            }
        }

        private string CombineText(string newText)
        {
            string text = Text;
            return text.Substring(0, SelectionStart) + newText + text.Substring(SelectionStart + SelectionLength);
        }

        private void MyPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (double.TryParse(Text, out double result))
            {
                DoubleValue = result;
            }
            else
            {
                DoubleValue = null;
            }
        }
        
        private void MyLoaded(object sender, RoutedEventArgs e)
        {
            UpdateText();
        }
    }
}
