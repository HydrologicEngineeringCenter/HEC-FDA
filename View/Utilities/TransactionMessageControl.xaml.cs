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
    /// Interaction logic for TransactionMessageControl.xaml
    /// </summary>
    public partial class TransactionMessageControl : UserControl
    {
        public TransactionMessageControl()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is FdaViewModel.Utilities.Transactions.ITransactionsAndMessages)
            {
                FdaViewModel.Utilities.Transactions.ITransactionsAndMessages vm = (FdaViewModel.Utilities.Transactions.ITransactionsAndMessages)this.DataContext;
                if (vm != null)
                {
                    if (vm.TransactionRows != null)
                    {
                        vm.TransactionRows.CollectionChanged += NotifyTransactionAdded;
                    }
                }
            }
        }

        private void NotifyTransactionAdded(object sender, EventArgs e)
        {
            //TransactionsAndMessagesTabControl.items
            //Tab_TransactionLog.Background = Brushes.Red;
            //Tab_TransactionLog.Header += "!";
            //Tab_TransactionLog.cont
            //DataGrid_TransactionLog.rows
            //double height = MainGrid.ActualHeight;
            //if(height < 200)
            //{
            //    MainGrid.Height += 30;
            //}
        }

    }
}
