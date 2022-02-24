using FdaLogging;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HEC.FDA.View.Commands
{
    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    class ErrorLevelToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                LoggingLevel logLevel = (LoggingLevel)value;

                switch (logLevel)
                {
                    case LoggingLevel.Fatal:
                        {
                            return new SolidColorBrush(Colors.Red);
                        }
                    case LoggingLevel.Error:
                        {
                            return new SolidColorBrush(Colors.Orange);
                        }
                    case LoggingLevel.Warn:
                        {
                            return new SolidColorBrush(Colors.Green);
                        }
                }
            }
                //return new SolidColorBrush(Colors.Black);
            
            //this should be the default color for a button. It is close, but not quite right.
            //the easy way is to clear the buttons background: btn.ClearValue(Button.BackgroundProperty);
            //but i don't have access to the button here. Another way is to set a default button style
            //in the app xaml that we use for all buttons? Then maybe i could reference that here?
            return System.Windows.SystemColors.ControlBrush;
            //System.Windows.Media.Brush defaultButtonColor = new Button().Background;
            //return defaultButtonColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
