using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using FdaViewModel.Utilities;
using Statistics;

namespace FdaViewModel.FlowTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 9:48:25 AM)]
    public class InflowOutflowEditorVM : Utilities.Transactions.TransactionAndMessageBase,Utilities.ISaveUndoRedo
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
        private List<Utilities.Transactions.TransactionRowItem> _Transactions;
        private ObservableCollection<Utilities.UndoRedoRowItem> _UndoRedoRows;
        #endregion
        #region Properties
     
        public ObservableCollection<Utilities.UndoRedoRowItem> UndoRedoRows
        {
            get { return _UndoRedoRows; }
            set { _UndoRedoRows = value; NotifyPropertyChanged(); }
        }


        public OwnedElement CurrentElement { get; set; }
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
  
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
 
        #endregion
        #region Constructors
        public Action<Utilities.ISaveUndoRedo> SaveAction { get; set; }

        public InflowOutflowEditorVM(Action<Utilities.ISaveUndoRedo> saveAction, Action<BaseViewModel> ownerValidationRules) : base()//InflowOutflowOwnerElement owner):base() //Action saveAction) : base()
        {

            ownerValidationRules(this);
     
            double[] xs = new double[] { 0, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000 };
            //double[] ys = new double[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(2000), new Statistics.None(3000), new Statistics.None(4000), new Statistics.None(5000), new Statistics.None(6000), new Statistics.None(7000), new Statistics.None(8000), new Statistics.None(9000), new Statistics.None(10000), new Statistics.None(11000) };

            SaveAction = saveAction;
            Curve = new Statistics.UncertainCurveIncreasing(xs, yValues, true, false,Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        }
        public InflowOutflowEditorVM(InflowOutflowElement elem, Action<Utilities.ISaveUndoRedo> saveAction, Action<BaseViewModel> ownerValidationRules) : base(elem)
        {
            ownerValidationRules(this);

            CurrentElement = elem;
            CurrentElement.ChangeIndex = 0;

            AssignValuesToThisFromElement(elem);

            SaveAction = saveAction;

            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(CurrentElement.ChangeTableName());

            UpdateUndoRedoVisibility(changeTableView, CurrentElement.ChangeIndex);
            UndoRedoRows = CreateUndoRedoRows(changeTableView,0,1);

        }

        #endregion
        #region Voids
         
        public override void Undo()
        {
            UndoElement(this);
        }

        public override void Redo()
        {
            RedoElement(this);
        }

        public override void SaveWhileEditing()
        {
            SaveAction(this);

        }

        private void AssignValuesToThisFromElement(InflowOutflowElement elem)
        {
            Name = elem.Name;
            LastEditDate = elem.LastEditDate;
            Description = elem.Description;
            Curve = elem.InflowOutflowCurve;
        }

        #endregion
            #region Functions
            #endregion

        public override void AddValidationRules()
        {
            
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != null, "Name cannot be blank.");

        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }

        public void AssignValuesFromElementToEditor(OwnedElement element)
        {
            InflowOutflowElement elem = (InflowOutflowElement)element;
            Name = elem.Name;
            LastEditDate = elem.LastEditDate;
            Description = elem.Description;
            Curve = elem.InflowOutflowCurve;
        }
        public void AssignValuesFromEditorToCurrentElement()
        {
            CurrentElement.LastEditDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            CurrentElement.Name = Name;
            ((InflowOutflowElement)CurrentElement).Description = Description;
            ((InflowOutflowElement)CurrentElement).InflowOutflowCurve = Curve;
        }

        public UncertainCurveDataCollection GetTheElementsCurve()
        {
            return ((InflowOutflowElement)CurrentElement).InflowOutflowCurve;
        }

        public UncertainCurveDataCollection GetTheEditorsCurve()
        {
            return Curve;
        }
        public void UpdateNameWithNewValue(string name)
        {
            Name = name;
        }
    }
}
