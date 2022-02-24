using System;

namespace HEC.FDA.ViewModel.Utilities.Transactions
{
    public class Transaction
    {
        public string OriginatorName { get; private set; }
        public string OriginatorType { get; private set; }
        public TransactionEnum Action { get; private set; }
        public string User { get; private set; }
        public string TransactionDate { get; private set; }
        public string Notes { get; private set; }
        public Transaction(string elementName, TransactionEnum action, string note, string originatorType, string transactionDate, string userName)
        {
            OriginatorName = elementName;
            Notes = note;
            Action = action;
            OriginatorType = originatorType;
            TransactionDate = DateTime.Now.ToShortDateString();
            User = System.Environment.UserName;
        }
    }
}
