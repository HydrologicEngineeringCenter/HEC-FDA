using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities.Transactions
{
    public interface ITransactionsAndMessages
    {
         ObservableCollection<TransactionRowItem> TransactionRows { get; set; }
         List<MessageRowItem> MessageRows { get; set; }
        bool TransactionsMessagesVisible { get; set; }





    }
}
