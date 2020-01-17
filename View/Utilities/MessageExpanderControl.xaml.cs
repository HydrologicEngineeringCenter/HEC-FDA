using FdaLogging;
using FdaViewModel.Utilities;
using FdaViewModel.Utilities.Transactions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace View.Utilities
{
    /// <summary>
    /// Interaction logic for MessageExpanderControl.xaml
    /// </summary>
    public partial class MessageExpanderControl : UserControl
    {
        public static readonly DependencyProperty ExpandedHeightProperty = DependencyProperty.Register("ExpandedHeight", typeof(int), typeof(MessageExpanderControl), new FrameworkPropertyMetadata(300, new PropertyChangedCallback(ExpandedHeightCallBack)));
        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(String), typeof(MessageExpanderControl), new FrameworkPropertyMetadata("Errors", new PropertyChangedCallback(HeaderTextCallBack)));
        public static readonly DependencyProperty StatusLevelProperty = DependencyProperty.Register("StatusLevel", typeof(LoggingLevel), typeof(MessageExpanderControl), new FrameworkPropertyMetadata(LoggingLevel.Debug, new PropertyChangedCallback(StatusLevelCallBack)));
        public static readonly DependencyProperty StatusVisibleProperty = DependencyProperty.Register("StatusVisible", typeof(bool), typeof(MessageExpanderControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(StatusVisibleCallBack)));

        private static readonly int LIST_ROW = 0;

        public int ExpandedHeight
        {
            get { return (int)GetValue(ExpandedHeightProperty); }
            set { SetValue(ExpandedHeightProperty, value); }
        }
        public String HeaderText
        {
            get { return (String)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public LoggingLevel StatusLevel
        {
            get { return (LoggingLevel)GetValue(StatusLevelProperty); }
            set { SetValue(StatusLevelProperty, value); }
        }

        public bool StatusVisible
        {
            get { return (bool)GetValue(StatusVisibleProperty); }
            set { SetValue(StatusVisibleProperty, value); }
        }
        public MessageExpanderControl()
        {
            InitializeComponent();
            cmb_Filter.Items.Add("All");
            cmb_Filter.Items.Add(LoggingLevel.Fatal);
            cmb_Filter.Items.Add(LoggingLevel.Error);
            cmb_Filter.Items.Add(LoggingLevel.Warn);
            cmb_Filter.Items.Add(LoggingLevel.Info);
            cmb_Filter.SelectedIndex = 0;
        }
        private void UpdateStatusText(LoggingLevel level)
        {
            switch(level)
            {
                case LoggingLevel.Fatal:
                    {
                        StatusPanel.Height = 25;

                        lbl_StatusMessage.Content = "Could not save because of the following ";
                        lbl_StatusEnding.Content = "fatal errors:";
                        break;
                    }
                case LoggingLevel.Error:
                    {
                        StatusPanel.Height = 25;

                        lbl_StatusMessage.Content = "Saved successfully with the following ";
                        lbl_StatusEnding.Content = "errors:";
                        break;
                    }
                case LoggingLevel.Warn:
                    {
                        StatusPanel.Height = 25;

                        lbl_StatusMessage.Content = "Saved successfully with the follwing ";
                        lbl_StatusEnding.Content = "messages:";
                        break;
                    }
                default:
                    {
                        //anything else will collapse the panel
                        StatusPanel.Height = 0;
                        break;
                    }
            }
        }
        private static void HeaderTextCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MessageExpanderControl owner = d as MessageExpanderControl;
            String headerText = (String)e.NewValue;
            owner.MessagesExpander.Header = headerText;
            
        }
        private static void StatusLevelCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MessageExpanderControl owner = d as MessageExpanderControl;
            LoggingLevel level = (LoggingLevel)e.NewValue;
            owner.UpdateStatusText(level);

        }
        private static void StatusVisibleCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MessageExpanderControl owner = d as MessageExpanderControl;
            bool showStatus = (bool)e.NewValue;
            if (showStatus)
            {
                owner.StatusPanel.Height = 25;
            }
            else
            {
                owner.StatusPanel.Height = 0;
            }

        }
        private static void ExpandedHeightCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MessageExpanderControl owner = d as MessageExpanderControl;
            int maxHeight = (int)e.NewValue;
            owner.MainGrid.RowDefinitions[LIST_ROW].MaxHeight = maxHeight;
            if (maxHeight > 50)
            {
                //this is in order to actually see the bottom of the scrolling
                owner.MessagesExpander.MaxHeight = maxHeight - 50;
            }
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            if (sender == e.OriginalSource)
            {
                MainGrid.RowDefinitions[LIST_ROW].Height = GridLength.Auto;
                MessagesExpander.Margin = new Thickness(5, 5, 5, 5);

            }
        }

        private void MessagesExpander_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender == e.OriginalSource)
            {
                MainGrid.RowDefinitions[LIST_ROW].Height = new GridLength(1,GridUnitType.Star);
                MessagesExpander.Margin = new Thickness(5, 5, 5, 40);
            }

        }

        private void cmb_Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmb_Filter.SelectedItem is LoggingLevel)
            {
                LoggingLevel level = (LoggingLevel)cmb_Filter.SelectedItem;
                if(level == LoggingLevel.Fatal)
                {
                    SetMessageRows(LoggingLevel.Fatal);
                }
                else if (level == LoggingLevel.Error)
                {
                    SetMessageRows(LoggingLevel.Error);
                }
                else if (level == LoggingLevel.Warn)
                {
                    SetMessageRows(LoggingLevel.Warn);
                }
                else if (level == LoggingLevel.Info)
                {
                    SetMessageRows(LoggingLevel.Info);
                }
            }
            else
            {
                //its "all"
                DisplayAllMessages();
            }
        }

        private void DisplayAllMessages()
        {
            if (this.DataContext is IDisplayLogMessages)
            {
                IDisplayLogMessages messageVM = (IDisplayLogMessages)this.DataContext;
                messageVM.DisplayAllMessages();
            }
        }
        private void SetMessageRows(LoggingLevel level)
        {
            if(this.DataContext is IDisplayLogMessages)
            {
                IDisplayLogMessages messageVM = (IDisplayLogMessages)this.DataContext;
                messageVM.FilterRowsByLevel(level);
            }
            //WindowVM vm = (WindowVM)this.DataContext;

            //if (vm != null && vm.Tab != null && vm.Tab.BaseVM != null && vm.Tab.BaseVM is ITransactionsAndMessages)
            //{
            //    MessagesListView.ItemsSource = ((ITransactionsAndMessages)vm.Tab.BaseVM).MessageRows;
            //    MessagesListView.DisplayMemberPath = "Message";
            //}
        }

        //private void UserControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    WindowVM vm = (WindowVM)this.DataContext;

        //    if (vm != null && vm.Tab != null && vm.Tab.BaseVM != null && vm.Tab.BaseVM is ITransactionsAndMessages)
        //    {
        //        MessagesListView.ItemsSource = ((ITransactionsAndMessages)vm.Tab.BaseVM).MessageRows;
        //        MessagesListView.DisplayMemberPath = "Message";
        //    }
        //}
    }

    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    public class LogLevelToTextColorConverter : IValueConverter
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
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Brushes.Black;
        }
    }

}
