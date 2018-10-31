using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities.Transactions
{
    public class TransactionHelper
    {
        public static List<Transaction> GetAllTransactions()
        {
            List<Transaction> transactions = new List<Transaction>();
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                if (Storage.Connection.Instance.TableNames().Contains("Transactions"))
                {
                    if (!Storage.Connection.Instance.IsOpen) { Storage.Connection.Instance.Open(); }
                    DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable("Transactions");
                    object[] row = null;
                    for (int i = 0; i < dtv.NumberOfRows; i++)
                    {
                        row = dtv.GetRow(i);
                        transactions.Add(new Transaction((string)row[0], (TransactionEnum)Enum.Parse(typeof(TransactionEnum), (string)row[2]), (string)row[5], (string)row[1], (string)row[3], (string)row[4]));
                    }
                }
            }
            return transactions;
        }

        public static List<Transaction> GetTransactionsForElement(BaseFdaElement element)
        {
            List<Transaction> retVals = new List<Transaction>();
            List<Transaction> allTransactions = GetAllTransactions();
            foreach (Transaction tran in allTransactions)
            {
                string fullType = element.GetType().ToString();
                string originatorType = fullType.Substring(fullType.LastIndexOf(".") + 1);
                if (tran.OriginatorName == element.Name && tran.OriginatorType == originatorType) //FlowTransforms.InflowOutflowElement))
                {
                    retVals.Add(tran);
                }
            }

            return retVals;
        }

        public static ObservableCollection<TransactionRowItem> GetTransactionRowItemsForElement(BaseFdaElement element)
        {
            ObservableCollection<TransactionRowItem> retval = new ObservableCollection<TransactionRowItem>();
            List<Transaction> transactionsForElement = GetTransactionsForElement(element);
            foreach(Transaction t in transactionsForElement)
            {
                retval.Add(new TransactionRowItem(t.TransactionDate, t.Notes, t.User));
            }
            return retval;
        }


        public static void LoadTransactionsAndMessages(ITransactionsAndMessages editor, BaseFdaElement element)
        {
            //load the transactions log
            editor.TransactionRows = Utilities.Transactions.TransactionHelper.GetTransactionRowItemsForElement(element);

            //load the messages log
            editor.MessageRows = Utilities.MessagesVM.GetMessageRowsForElement(element);
            // if(TransactionRows.Count>0 || MessageRows.Count > 0)
            {
                editor.TransactionsMessagesVisible = true;
            }
        }

    }
}
