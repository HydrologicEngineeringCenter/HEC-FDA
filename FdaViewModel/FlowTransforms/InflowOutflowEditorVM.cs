using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FdaViewModel.Utilities;
using Functions;
using Model;

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
        private Model.IFdaFunction _Curve;
        private string _PlotTitle = "Inflow-Outflow Curve";
        private List<Utilities.Transactions.TransactionRowItem> _Transactions;
        private ObservableCollection<Utilities.UndoRedoRowItem> _UndoRedoRows = new ObservableCollection<UndoRedoRowItem>();
        private ObservableCollection<Utilities.UndoRedoRowItem> _UndoRows;
        private ObservableCollection<Utilities.UndoRedoRowItem> _RedoRows;

        #endregion
        #region Properties

        public ObservableCollection<Utilities.UndoRedoRowItem> UndoRedoRows
        {
            get { return _UndoRedoRows; }
            set { _UndoRedoRows = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<Utilities.UndoRedoRowItem> UndoRows
        {
            get { return _UndoRows; }
            set { _UndoRows = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<Utilities.UndoRedoRowItem> RedoRows
        {
            get { return _RedoRows; }
            set { _RedoRows = value; NotifyPropertyChanged(); }
        }
        //public int SelectedIndexInUndoList
        //{
        //    get { return 0; }
        //    set { if (value == -1) { return; } CurrentElement.ChangeIndex += value; Undo(); }
        //}
        public int SelectedIndexInRedoList { get; set; }

        public ChildElement CurrentElement { get; set; }
        public IFdaFunction Curve
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
        public Action<Utilities.ISaveUndoRedo> SaveAction { get; set; }
 
        #endregion
        #region Constructors

        public InflowOutflowEditorVM(Action<Utilities.ISaveUndoRedo> saveAction, Action<BaseViewModel> ownerValidationRules) : base()//InflowOutflowOwnerElement owner):base() //Action saveAction) : base()
        {

            ownerValidationRules(this);

            //double[] xs = new double[] { 0, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000 };
            ////double[] ys = new double[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            //Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(2000), new Statistics.None(3000), new Statistics.None(4000), new Statistics.None(5000), new Statistics.None(6000), new Statistics.None(7000), new Statistics.None(8000), new Statistics.None(9000), new Statistics.None(10000), new Statistics.None(11000) };

            List<double> xValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            List<double> yValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            Functions.ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(xValues, yValues);
            IFdaFunction defaultFunction = IFdaFunctionFactory.Factory( IParameterEnum.InflowOutflow, (IFunction)func);

            SaveAction = saveAction;
            Curve = defaultFunction;
        
        }
        public InflowOutflowEditorVM(InflowOutflowElement elem, Action<Utilities.ISaveUndoRedo> saveAction, Action<BaseViewModel,string> ownerValidationRules) : base(elem)
        {
            ownerValidationRules(this,elem.Name);

            CurrentElement = elem;
            //CurrentElement.ChangeIndex = 0;

            AssignValuesToThisFromElement(elem);

            SaveAction = saveAction;

            //DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(CurrentElement.ChangeTableName());

            //UpdateUndoRedoVisibility(changeTableView, CurrentElement.ChangeIndex);
            //UndoRedoRows = CreateUndoRedoRows(changeTableView,0,1);
            UpdateTheUndoRedoRowItems();

        }

        #endregion
        #region Voids
        public override void Save()
        {
            throw new NotImplementedException();
        }
        public void UpdateTheUndoRedoRowItems()
        {
            //UndoRows = new ObservableCollection<UndoRedoRowItem>();
            //RedoRows = new ObservableCollection<UndoRedoRowItem>();
            //int currentIndex = CurrentElement.ChangeIndex;
            
            //for(int i = currentIndex+1;i<UndoRedoRows.Count;i++)
            //{
            //    UndoRows.Add(UndoRedoRows[i]);
            //}

        }
        public  void Undo()
        {
           // UndoElement(this);
        }

        public  void Redo()
        {
            //RedoElement(this);
        }

        public  void SaveWhileEditing()
        {
            
            UndoRedoRows.Insert(0, new UndoRedoRowItem(Name, DateTime.Now.ToString("G")));
            UpdateTheUndoRedoRowItems();
            SaveAction(this);

        }

        private void AssignValuesToThisFromElement(InflowOutflowElement elem)
        {
            Name = elem.Name;
            LastEditDate = elem.LastEditDate;
            Description = elem.Description;
            Curve = elem.Curve;
        }

        #endregion
            #region Functions
            #endregion

        public override void AddValidationRules()
        {
            
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != null, "Name cannot be blank.");

        }

      

        public void AssignValuesFromElementToEditor(ChildElement element)
        {
            InflowOutflowElement elem = (InflowOutflowElement)element;
            Name = elem.Name;
            LastEditDate = elem.LastEditDate;
            Description = elem.Description;
            Curve = elem.Curve;
        }
        public void AssignValuesFromEditorToCurrentElement()
        {
            CurrentElement.LastEditDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            CurrentElement.Name = Name;
            ((InflowOutflowElement)CurrentElement).Description = Description;
            ((InflowOutflowElement)CurrentElement).Curve = Curve;
        }

        public IFdaFunction GetTheElementsCurve()
        {
            return ((InflowOutflowElement)CurrentElement).Curve;
        }

        public IFdaFunction GetTheEditorsCurve()
        {
            return Curve;
        }
        public void UpdateNameWithNewValue(string name)
        {
            Name = name;
        }

        public void LoadInitialStateForUndoRedo()
        {
            throw new NotImplementedException();
        }
    }
}
