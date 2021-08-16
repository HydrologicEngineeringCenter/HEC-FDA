using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Utilities.Transactions
{
    public class TransactionRowItem
    {
        public string Date { get; set; }
        public string Message { get; set; }
        public string User { get; set; }

        public TransactionRowItem(string date, string message, string user)
        {
            Date = date;
            Message = message;
            User = user;
        }
    }
}
