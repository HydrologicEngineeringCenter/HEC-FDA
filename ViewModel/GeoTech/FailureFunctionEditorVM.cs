using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Utilities;
using Functions;
using Model;


namespace ViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 2:11:22 PM)]
    public class FailureFunctionEditorVM : Utilities.Transactions.TransactionAndMessageBase
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 2:11:22 PM
        #endregion
        #region Fields
        private string _Name = "";
        private string _Description = "";
        private readonly string _Title = "Failure Function Curve";
        private IFdaFunction _Curve;
        private List<LeveeFeatureElement> _LateralStructureList;
        private LeveeFeatureElement _SelectedLateralStructure;

        #endregion
        #region Properties
        public Action<Utilities.ISaveUndoRedo> SaveAction { get; set; }


        //public int SelectedIndexInUndoList
        //{
        //    set { CurrentElement.ChangeIndex += value + 1; Undo(); }
        //}

        public LeveeFeatureElement SelectedLateralStructure
        {
            get { return _SelectedLateralStructure; }
            set { _SelectedLateralStructure = value; NotifyPropertyChanged();  }
        }
        public List<LeveeFeatureElement> LateralStructureList
        {
            get { return _LateralStructureList; }
            set { _LateralStructureList = value; NotifyPropertyChanged(); }
        }
      
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
        public string PlotTitle
        {
            get { return _Title; }
        }
        public IFdaFunction Curve
        {
            get { return _Curve; }
            set { _Curve = value; NotifyPropertyChanged(); }
        }
        public ChildElement CurrentElement { get; set; }



        #endregion
        #region Constructors
        public FailureFunctionEditorVM(Action<Utilities.ISaveUndoRedo> saveAction, Action<BaseViewModel> ownerValidationRules, List<LeveeFeatureElement> leveeList):base()
        {
            //double[] xValues = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 15 };
            //Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(0), new Statistics.None(.1), new Statistics.None(.2), new Statistics.None(.3), new Statistics.None(.4), new Statistics.None(.5), new Statistics.None(.6), new Statistics.None(.7), new Statistics.None(.8), new Statistics.None(.9), new Statistics.None(.95), new Statistics.None(.99) };
            //Curve = new Statistics.UncertainCurveIncreasing(xValues, yValues, true, true, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

            List<double> xValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            List<double> yValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            Functions.ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            Curve = IFdaFunctionFactory.Factory( IParameterEnum.LateralStructureFailure, (IFunction)func);

            LateralStructureList = leveeList;
            SaveAction = saveAction;
            ownerValidationRules(this);



        }

        public FailureFunctionEditorVM(FailureFunctionElement elem, Action<Utilities.ISaveUndoRedo> saveAction, Action<BaseViewModel> ownerValidationRules, List<LeveeFeatureElement> latStructList) :base(elem)// string name, string description, Statistics.UncertainCurveDataCollection curve,LeveeFeatureElement selectedLevee, List<LeveeFeatureElement> latStructList):base()
        {
            CurrentElement = elem;
            //CurrentElement.ChangeIndex = 0;
            SaveAction = saveAction;
            LateralStructureList = latStructList;
            ownerValidationRules(this);

            AssignValuesFromElementToEditor(elem);

           // DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(CurrentElement.ChangeTableName());
            //UpdateUndoRedoVisibility(changeTableView, CurrentElement.ChangeIndex);
        }
        #endregion
        #region Voids
        public override void Save()
        {
            throw new NotImplementedException();
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
            //SaveAction(this);
           // _OwnerNode.SaveElementWhileEditing(this);
        }
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
        }

        
        public void UpdateTheUndoRedoRowItems()
        {
            //int currentIndex = CurrentElement.ChangeIndex;
            //RedoRows.Clear();
            //for (int i = currentIndex + 1; i < UndoRedoRows.Count; i++)
            //{
            //    RedoRows.Add(UndoRedoRows[i]);
            //}

        }
        public void AssignValuesFromElementToEditor(ChildElement element)
        {
            FailureFunctionElement elem = (FailureFunctionElement)element;
            Name = elem.Name;
            SelectedLateralStructure = elem.SelectedLateralStructure;
            LastEditDate = elem.LastEditDate;
            Description = elem.Description;
            Curve = elem.Curve;
        }
        public void AssignValuesFromEditorToCurrentElement()
        {
            CurrentElement.LastEditDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            CurrentElement.Name = Name;
            ((FailureFunctionElement)CurrentElement).SelectedLateralStructure = SelectedLateralStructure;
            ((FailureFunctionElement)CurrentElement).Description = Description;
            ((FailureFunctionElement)CurrentElement).Curve = Curve;
        }

        public IFdaFunction GetTheElementsCurve()
        {
            return ((FailureFunctionElement)CurrentElement).Curve;
        }

        public IFdaFunction GetTheEditorsCurve()
        {
            return Curve;
        }
        public void UpdateNameWithNewValue(string name)
        {
            Name = name;
        }

        #endregion
        #region Functions
        #endregion

    }
}
