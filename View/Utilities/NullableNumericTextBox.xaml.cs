using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HEC.FDA.View.Utilities
{
    /// <summary>
    /// Interaction logic for NullableNumericTextBox.xaml
    /// </summary>
    public partial class NullableNumericTextBox : UserControl
    {
        public static readonly DependencyProperty DoubleValueProperty = DependencyProperty.Register(nameof(DoubleValue), typeof(double?), typeof(NullableNumericTextBox), new PropertyMetadata(DoubleValueChangedCallback));

        public double? DoubleValue
        {
            get { return (double?)GetValue(DoubleValueProperty); }
            set { SetValue(DoubleValueProperty, value); }
        }

        public NullableNumericTextBox()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            
                    e.Handled = true;
            

            //Regex regex = new Regex("[^0-9.-]+");
            //e.Handled = regex.IsMatch(e.Text);
            if ( sender is TextBox txt)
            {
                string value = combined_text(txt, e.Text);
                if (double.TryParse(value, out double result))
                {
                    e.Handled = false;
                    DoubleValue = result;
                }
                
            }
            //get datacontext and set the value?
        }

        private static void DoubleValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NullableNumericTextBox owner = (NullableNumericTextBox)d;

            owner.DoubleValue = (double)e.NewValue;
            owner.txtBox.Text = owner.DoubleValue.ToString();
            //if (double.TryParse(val, out double result))
            //{
            //    //e.Handled = false;
            //    DoubleValue = result;
            //}
            //else
            //{
            //    DoubleValue = null;
            //}

        }

        private string combined_text(TextBox tb, string newText)
        {
            return tb.Text.Substring(0, tb.SelectionStart) + newText + tb.Text.Substring(tb.SelectionStart + tb.SelectionLength);
        }

        private void txtBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //e.Handled = true;


            //Regex regex = new Regex("[^0-9.-]+");
            //e.Handled = regex.IsMatch(e.Text);
            if (sender is TextBox txt)
            {
                //string value = combined_text(sender as TextBox, e.Text);

                if (double.TryParse(txt.Text, out double result))
                {
                    //e.Handled = false;
                    DoubleValue = result;
                }
                else
                {
                    DoubleValue = null;
                }

            }
        }
    }
}
