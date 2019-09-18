using FdaViewModel.Utilities;
using FdaViewModel.Utilities.Transactions;
using System;
using System.Collections.Generic;
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

namespace Fda.Utilities
{
    /// <summary>
    /// Interaction logic for MessageExpanderControl.xaml
    /// </summary>
    public partial class MessageExpanderControl : UserControl
    {
        public static readonly DependencyProperty ExpandedHeightProperty = DependencyProperty.Register("ExpandedHeight", typeof(int), typeof(MessageExpanderControl), new FrameworkPropertyMetadata(300, new PropertyChangedCallback(ExpandedHeightCallBack)));
        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(String), typeof(MessageExpanderControl), new FrameworkPropertyMetadata("Errors", new PropertyChangedCallback(HeaderTextCallBack)));

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
        public MessageExpanderControl()
        {
            InitializeComponent();
            cmb_Filter.Items.Add("All");
            cmb_Filter.Items.Add(NLog.LogLevel.Fatal);
            cmb_Filter.Items.Add(NLog.LogLevel.Error);
            cmb_Filter.Items.Add(NLog.LogLevel.Warn);
            cmb_Filter.Items.Add(NLog.LogLevel.Info);

        }
        private static void HeaderTextCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MessageExpanderControl owner = d as MessageExpanderControl;
            String headerText = (String)e.NewValue;
            owner.MessagesExpander.Header = headerText;
            
        }
        private static void ExpandedHeightCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MessageExpanderControl owner = d as MessageExpanderControl;
            int maxHeight = (int)e.NewValue;
            owner.MainGrid.RowDefinitions[0].MaxHeight = maxHeight;
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
                MainGrid.RowDefinitions[0].Height = GridLength.Auto;
                MessagesExpander.Margin = new Thickness(5, 5, 5, 5);
            }
        }

        private void MessagesExpander_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender == e.OriginalSource)
            {
                MainGrid.RowDefinitions[0].Height = GridLength.Auto;
                MessagesExpander.Margin = new Thickness(5, 5, 5, 40);
            }

        }

        private void cmb_Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmb_Filter.SelectedItem is NLog.LogLevel)
            {
                NLog.LogLevel level = (NLog.LogLevel)cmb_Filter.SelectedItem;
                if(level == NLog.LogLevel.Fatal)
                {
                    SetMessageRows(NLog.LogLevel.Fatal);
                }
                else if (level == NLog.LogLevel.Error)
                {
                    SetMessageRows(NLog.LogLevel.Error);
                }
                else if (level == NLog.LogLevel.Warn)
                {
                    SetMessageRows(NLog.LogLevel.Warn);
                }
                else if (level == NLog.LogLevel.Info)
                {
                    SetMessageRows(NLog.LogLevel.Info);
                }
            }
            else
            {
                //its "all"
            }
        }

        private void SetMessageRows(NLog.LogLevel level)
        {
            if(this.DataContext is ITransactionsAndMessages)
            {
                ITransactionsAndMessages messageVM = (ITransactionsAndMessages)this.DataContext;
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
}
