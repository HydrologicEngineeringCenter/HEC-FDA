using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HEC.FDA.View.Utilities
{
    public class NumericTextBoxWithClickHighlighting: TextBox
    {
        public NumericTextBoxWithClickHighlighting()
        {
            AddHandler(PreviewMouseLeftButtonDownEvent,
              new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
            AddHandler(GotKeyboardFocusEvent,
              new RoutedEventHandler(SelectAllText), true);
            AddHandler(MouseDoubleClickEvent,
              new RoutedEventHandler(SelectAllText), true);
            AddHandler(PreviewTextInputEvent,
                new TextCompositionEventHandler(txt_XValue_PreviewTextInput), true);
        }

        private static void SelectivelyIgnoreMouseButton(object sender,
                                                         MouseButtonEventArgs e)
        {
            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
            {
                var textBox = (TextBox)parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    // If the text box is not yet focussed, give it the focus and
                    // stop further processing of this click event.
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }

        private void txt_XValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tBox = (TextBox)sender;
            if (Convert.ToChar(e.Text) == (char)8) { e.Handled = false; }//allow backspace
            else if (e.Text == " ") { e.Handled = true; }//don't allow spaces
            else if (e.Text == "-" && tBox.SelectionStart == 0 && tBox.Text.IndexOf("-") == -1) { e.Handled = false; }//allow negative
            else if (e.Text == "." && tBox.Text.IndexOf(".") == -1) { e.Handled = false; }//allow decimal
            else if (!char.IsDigit(Convert.ToChar(e.Text))) { e.Handled = true; }//numeric only


        }
    }
}
