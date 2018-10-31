using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities.Transactions
{
    public abstract class TransactionAndMessageBase:Editors.BaseEditorVM
    {    
        public ObservableCollection<Utilities.Transactions.TransactionRowItem> TransactionRows { get; set; }
        public List<Utilities.MessageRowItem> MessageRows { get; set; }

        public TransactionAndMessageBase():base(null)
        {

        }
        public TransactionAndMessageBase(ChildElement element):base(element,null)
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
