using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.FlowTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 9:48:25 AM)]
    public class InflowOutflowEditorVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 9:48:25 AM
        #endregion
        #region Fields
        private string _Name;
        private string _Description;
        private Statistics.UncertainCurveDataCollection _Curve;
        private string _PlotTitle = "Inflow-Outflow Curve";
        private List<Utilities.Transactions.Transaction> _Transactions;

        #endregion
        #region Properties
        public InflowOutflowElement CurrentElement { get; set; }
        public Statistics.UncertainCurveDataCollection Curve
        {
            get { return _Curve; }
            set { _Curve = value; NotifyPropertyChanged(); }
        }
        public string PlotTitle
        {
            get { return _PlotTitle; }
            set { _PlotTitle = value; NotifyPropertyChanged(); }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
        public List<Utilities.Transactions.Transaction> Transactions
        {
            get { return _Transactions; }
            set { _Transactions = value; }
        }
        public int ChangeIndex { get; set; }

        public List<Utilities.MessageItem> Messages { get; set; }
        
        #endregion
        #region Constructors
        public InflowOutflowEditorVM() : base()
        {
            double[] xs = new double[] { 0, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000 };
            //double[] ys = new double[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(2000), new Statistics.None(3000), new Statistics.None(4000), new Statistics.None(5000), new Statistics.None(6000), new Statistics.None(7000), new Statistics.None(8000), new Statistics.None(9000), new Statistics.None(10000), new Statistics.None(11000) };

            Curve = new Statistics.UncertainCurveIncreasing(xs, yValues, true, false,Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        }
        public InflowOutflowEditorVM(InflowOutflowElement elem) : base()
        {
            CurrentElement = elem;
            Name = elem.Name;
            Description = elem.Description;
            Curve = elem.InflowOutflowCurve;

            //load the transactions log
            Utilities.Transactions.TransactionVM logVM = new Utilities.Transactions.TransactionVM();
            List<Utilities.Transactions.Transaction> transactions = logVM.GetTransactionsForElement(elem);
            Transactions = transactions;

            //load the messages log
            List<Utilities.MessageItem> messages =  Utilities.MessagesVM.GetMessagesForElement(elem);
            //i need to filter somehow
            Messages = messages;

        }
        #endregion
        #region Voids
        public override void Undo()
        {
            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(CurrentElement.ChangeTableName());
            if (ChangeIndex < changeTableView.NumberOfRows - 1)
            {
                //disable the undo button somehow?
                InflowOutflowElement prevElement = (InflowOutflowElement)CurrentElement.GetPreviousElementFromChangeTable(ChangeIndex + 1);
                if (prevElement != null)// null if out of range index
                {
                    Name = prevElement.Name;
                    LastEditDate = prevElement.LastEditDate;
                    Description = prevElement.Description;
                    Curve = prevElement.InflowOutflowCurve;
                    ChangeIndex += 1;
                }
            }
        }

        public override void Redo()
        {
            //get the previous state
            if (ChangeIndex > 0)
            {
                InflowOutflowElement nextElement = (InflowOutflowElement)CurrentElement.GetNextElementFromChangeTable(ChangeIndex - 1);
                if (nextElement != null)// null if out of range index
                {
                    Name = nextElement.Name;
                    LastEditDate = nextElement.LastEditDate;
                    Description = nextElement.Description;
                    Curve = nextElement.InflowOutflowCurve;
                    ChangeIndex -= 1;
                }
            }
        }

        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != null, "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != "test", "Name cannot be test.",false);

        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
    }
}
