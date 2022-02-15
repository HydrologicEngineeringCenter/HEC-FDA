using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Utilities.Transactions
{
    public class TransactionEventArgs: EventArgs
    {
        public string Notes { get; private set; }
        public TransactionEnum Action { get; private set; }
        public string TransactionDate { get; private set; }
        public string User { get; private set; }
        public string OriginatorType { get; private set; }
        public string OriginatorName { get; private set; }
        /// <summary>
        /// this stores information that will populate a transaction log
        /// </summary>
        /// <param name="elementName">This describes the name of the element the transaction occured on</param>
        /// <param name="action">This describes the action, edit, create, delete</param>
        /// <param name="note">Any auxilary information, for instance, on a creation, where did the data come from (i.e. the original file path)</param>
        /// <param name="originatorType"></param>
        public TransactionEventArgs(string elementName, TransactionEnum action, string note, [System.Runtime.CompilerServices.CallerFilePath] string originatorType = "")
        {
            OriginatorName = elementName;
            Notes = note;
            Action = action;
            if (System.IO.Path.GetExtension(originatorType) == ".cs")
            {
                OriginatorType = System.IO.Path.GetFileNameWithoutExtension(originatorType);
            }else
            {
                OriginatorType = originatorType;
            }
            TransactionDate = DateTime.Now.ToShortDateString();
            User = System.Environment.UserName;

        }
    }
}
