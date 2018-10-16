using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities.Transactions
{
    public abstract class TransactionAndMessageBase:BaseViewModel
    {    
        public List<Utilities.Transactions.TransactionRowItem> TransactionRows { get; set; }
        public List<Utilities.MessageRowItem> MessageRows { get; set; }

        public TransactionAndMessageBase():base()
        {

        }
        public TransactionAndMessageBase(ChildElement element):base()
        {
            if(element == null) { return; }       
            LoadTransactionsAndMessages(element);
        }

        private void LoadTransactionsAndMessages(Utilities.ChildElement element)
        {
            //load the transactions log
            TransactionRows = Utilities.Transactions.TransactionHelper.GetTransactionRowItemsForElement(element);

            //load the messages log
            MessageRows = Utilities.MessagesVM.GetMessageRowsForElement(element);
        }
    }
}
