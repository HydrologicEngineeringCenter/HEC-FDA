using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities.Transactions
{
    public class TransactionVM: BaseViewModel
    {
        #region Notes
        #endregion
        #region Fields
        public List<Transaction> _transactions = new List<Transaction>();
        #endregion
        #region Properties
        public List<Transaction> Transactions
        {
            get { return _transactions; }
        }
        #endregion
        #region Constructors
        public TransactionVM()
        {
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                if (Storage.Connection.Instance.TableNames().Contains("Transactions"))
                {
                    if (!Storage.Connection.Instance.IsOpen) { Storage.Connection.Instance.Open(); }
                    DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable("Transactions");
                    object[] row = null;
                    for(int i = 0; i<dtv.NumberOfRows;i++)
                    {
                        row = dtv.GetRow(i);
                        _transactions.Add(new Transaction((string)row[0], (TransactionEnum)Enum.Parse(typeof(TransactionEnum), (string)row[2]), (string)row[5], (string)row[1], (string)row[3], (string)row[4]));
                    }
                }
            }
        }

        /// <summary>
        /// This could be made into a static call and get rid of the constructor above
        /// </summary>
        /// <returns></returns>
        public List<Transaction> GetTransactionsForElement(ChildElement element)
        {
            List<Transaction> retVals = new List<Transaction>();
            foreach(Transaction tran in _transactions)
            {
                if(tran.OriginatorName == element.Name && tran.OriginatorType == nameof(FlowTransforms.InflowOutflowElement))
                {
                    retVals.Add(tran);
                }
            }

            return retVals;
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
    }
}
