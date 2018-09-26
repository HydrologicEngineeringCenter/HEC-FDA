using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FdaViewModel.StageTransforms
{
    public class RatingCurveEditorVM : BaseViewModel
    {
        #region Notes
        #endregion
        #region Fields
        private string _Name = "";
        private string _Description = "";
        private bool _ReadOnly = false;
        private readonly string _Title = "Rating Curve";
        private Statistics.UncertainCurveDataCollection _Curve;

        //private ICommand _UndoCommand;
        //private ICommand _RedoCommand;
        //private ICommand _SaveWhileEditing;

        private RatingCurveElement _savedElement;

        #endregion
        #region Properties

        //public ICommand UndoCommand
        //{
        //    get { return _UndoCommand ?? (_UndoCommand = new Utilities.CommandHandler(() => UndoClicked(),true)); }
        //}
        //public ICommand RedoCommand
        //{
        //    get { return _RedoCommand ?? (_RedoCommand = new Utilities.CommandHandler(() => RedoClicked(), true)); }
        //}
        //public ICommand SaveWhileEditingCommand
        //{
        //    get { return _SaveWhileEditing ?? (_SaveWhileEditing = new Utilities.CommandHandler(() => SaveWhileEditing(), true)); }
        //}
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
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value; NotifyPropertyChanged(); }
        }
        public string PlotTitle
        {
            get { return _Title; }
        }
        public Statistics.UncertainCurveDataCollection Curve
        {
            get { return _Curve; }
            set { _Curve = value; NotifyPropertyChanged(); }
        }
        public int ChangeIndex { get; set; }
        RatingCurveElement CurrentElement { get; set; }
        #endregion
        #region Constructors
        /// <summary>
        /// Used when creating a new Rating
        /// </summary>
        public RatingCurveEditorVM() : base()
        {

            //double[] xValues = new double[] { 100, 200, 500, 1000, 2000 };
            //Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(1), new Statistics.None(2), new Statistics.None(5), new Statistics.None(10), new Statistics.None(20) };
            double[] xValues = new double[] { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(95), new Statistics.None(96), new Statistics.None(97), new Statistics.None(99), new Statistics.None(104), new Statistics.None(109), new Statistics.None(110), new Statistics.None(114), new Statistics.None(116), new Statistics.None(119), new Statistics.None(120), new Statistics.None(121) };
            Curve = new Statistics.UncertainCurveIncreasing(xValues, yValues,true,true,Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        }
        /// <summary>
        /// Used when editing an existing curve
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="curve"></param>
        public RatingCurveEditorVM(RatingCurveElement elem) : base()
        {
            CurrentElement = elem;
            ChangeIndex = 0;// elem.ChangeIndex; 
            Name = elem.Name;
            Description = elem.Description;
            Curve = elem.RatingCurve;



        }
        #endregion
        #region Voids

        public override void Undo()
        {
            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(CurrentElement.ChangeTableName());
            if (ChangeIndex < changeTableView.NumberOfRows - 1)
            {
               //disable the undo button somehow?
                RatingCurveElement prevElement = (RatingCurveElement)CurrentElement.GetPreviousElementFromChangeTable(ChangeIndex + 1);
                if (prevElement != null)// null if out of range index
                {
                    Name = prevElement.Name;
                    LastEditDate = prevElement.LastEditDate;
                    Description = prevElement.Description;
                    Curve = prevElement.RatingCurve;
                    ChangeIndex += 1;
                }
            }
        }

        public override void Redo()
        {
            //get the previous state
            if (ChangeIndex >0)
            {
                RatingCurveElement nextElement = (RatingCurveElement)CurrentElement.GetNextElementFromChangeTable(ChangeIndex - 1);
                if (nextElement != null)// null if out of range index
                {
                    Name = nextElement.Name;
                    LastEditDate = nextElement.LastEditDate;
                    Description = nextElement.Description;
                    Curve = nextElement.RatingCurve;
                    ChangeIndex -= 1;
                }                
            }
        }

        public override void SaveWhileEditing()
        {
            //if (_savedElement != null)
            //{
            //    _savedElement.Remove(this, new EventArgs());
            //}
            ////create an element
            //Statistics.LogPearsonIII lpiii = new Statistics.LogPearsonIII(Mean, StandardDeviation, Skew, SampleSize);//are the default probabilities editable in the model?
            //AnalyticalFrequencyElement afe = new AnalyticalFrequencyElement(Name, Description, lpiii, _Owner);
            //_savedElement = afe;
            ////save the element
            //_Owner.AddElement(afe);

            ////AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(afe.Name, Utilities.Transactions.TransactionEnum.CreateNew, "Initial Name: " + afe.Name + " Description: " + afe.Description + " Mean: " + afe.Distribution.GetMean + " Standard Deviation: " + afe.Distribution.GetStDev + " Skew: " + afe.Distribution.GetG + " EYOR: " + afe.Distribution.GetSampleSize, nameof(AnalyticalFrequencyElement)));

        }

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
        }
        public override void Save()
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
