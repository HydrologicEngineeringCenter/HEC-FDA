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
        public MessageExpanderControl()
        {
            InitializeComponent();
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            MainGrid.RowDefinitions[0].Height = GridLength.Auto;

        }

        private void MessagesExpander_Expanded(object sender, RoutedEventArgs e)
        {
            MainGrid.RowDefinitions[0].Height = GridLength.Auto;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            WindowVM vm = (WindowVM)this.DataContext;

            if (vm != null && vm.Tab != null && vm.Tab.BaseVM != null && vm.Tab.BaseVM is ITransactionsAndMessages)
            {
                MessagesListView.ItemsSource = ((ITransactionsAndMessages)vm.Tab.BaseVM).MessageRows;
                MessagesListView.DisplayMemberPath = "Message";
            }
        }
    }
}
