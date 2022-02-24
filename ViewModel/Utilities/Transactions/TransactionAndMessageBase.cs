using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.Utilities.Transactions
{
    //todo: I think we can get rid of this class.
    public abstract class TransactionAndMessageBase:Editors.BaseEditorVM
    {
        private ObservableCollection<FdaLogging.LogItem> _MessageRows = new ObservableCollection<FdaLogging.LogItem>();

        // public ObservableCollection<Utilities.Transactions.TransactionRowItem> TransactionRows { get; set; }
        //public ObservableCollection<Utilities.MessageRowItem> MessageRows { get; set; }
        public ObservableCollection<FdaLogging.LogItem> MessageRows
        {
            get { return _MessageRows; }
            set { _MessageRows = value; NotifyPropertyChanged("MessageRows"); NotifyPropertyChanged("MessageCount"); }
        }
        public TransactionAndMessageBase():base(null)
        {

        }
        public TransactionAndMessageBase(ChildElement element):base(element,null)
        {
            //if(element == null) { return; }       
            //LoadTransactionsAndMessages(element);
        }
        public bool TransactionsMessagesVisible
        {
            get;
            set;
        }
        public int MessageCount
        {
            get { return _MessageRows.Count; }
        }
        //public override void AddMessage(string error, LoggingLevel level)
        //{

        //    FdaLogging.LogItem mri = new FdaLogging.LogItem(DateTime.Now, error, "", level.ToString(), "", "");
        //    InsertMessage(mri);
        //}
        private void InsertMessage(FdaLogging.LogItem mri)
        {
            ObservableCollection<FdaLogging.LogItem> tempList = new ObservableCollection<FdaLogging.LogItem>();
            tempList.Add(mri);
            foreach (FdaLogging.LogItem row in MessageRows)
            {
                tempList.Add(row);
            }
            MessageRows = tempList;
        }
        //private void LoadTransactionsAndMessages(Utilities.ChildElement element)
        //{
        //    //load the transactions log
        //    TransactionRows = Utilities.Transactions.TransactionHelper.GetTransactionRowItemsForElement(element);

        //    //load the messages log
        //    //MessageRows = Utilities.MessagesVM.GetMessageRowsForElement(element);
        //}
    }
}
