using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FdaViewModel.Utilities;
using Model;
using Model.Inputs.Functions.ImpactAreaFunctions;

namespace FdaViewModel.StageTransforms
{
    public class RatingCurveEditorVM : Utilities.Transactions.TransactionAndMessageBase
    {
        #region Notes
        #endregion
        #region Fields
        private string _Name = "";
        private string _Description = "";
        private bool _ReadOnly = false;
        private readonly string _Title = "Rating Curve";
        private IFdaFunction _Curve;

        //private ICommand _UndoCommand;
        //private ICommand _RedoCommand;
        //private ICommand _SaveWhileEditing;

        //private RatingCurveElement _savedElement;

        #endregion
        #region Properties
        public Action<Utilities.ISaveUndoRedo> SaveAction { get; set; }
        //public int SelectedIndexInUndoList
        //{
        //    set { CurrentElement.ChangeIndex += value + 1; Undo(); }
        //}

        //public string Name
        //{
        //    get { return _Name; }
        //    set { _Name = value; NotifyPropertyChanged(); }
        //}
        //public string Description
        //{
        //    get { return _Description; }
        //    set { _Description = value; NotifyPropertyChanged(); }
        //}
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value; NotifyPropertyChanged(); }
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
        //public ChildElement CurrentElement { get; set; }

        //public RatingCurveOwnerElement OwnerNode { get; set; }


        #endregion
        #region Constructors
        /// <summary>
        /// Used when creating a new Rating
        /// </summary>
        public RatingCurveEditorVM(Action<Utilities.ISaveUndoRedo> savingAction, Action<BaseViewModel> ownerValidationRules) : base()
        {
            ownerValidationRules(this);

            SaveAction = savingAction;
            //double[] xValues = new double[] { 100, 200, 500, 1000, 2000 };
            //Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(1), new Statistics.None(2), new Statistics.None(5), new Statistics.None(10), new Statistics.None(20) };
            List<double> xValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            List<double> yValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            Functions.ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(xValues, yValues);
            ImpactAreaFunctionFactory.Factory(func, ImpactAreaFunctionEnum.Rating);
            //Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(95), new Statistics.None(96), new Statistics.None(97), new Statistics.None(99), new Statistics.None(104), new Statistics.None(109), new Statistics.None(110), new Statistics.None(114), new Statistics.None(116), new Statistics.None(119), new Statistics.None(120), new Statistics.None(121) };
            //Curve = new Statistics.UncertainCurveIncreasing(xValues, yValues,true,true,Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

        }
        /// <summary>
        /// Used when editing an existing curve
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="curve"></param>
        public RatingCurveEditorVM(RatingCurveElement elem, Action<Utilities.ISaveUndoRedo> savingAction, Action<BaseViewModel> ownerValidationRules) : base(elem)
        {
            ownerValidationRules(this);

            SaveAction = savingAction;

            CurrentElement = elem;
           // CurrentElement.ChangeIndex = 0;// elem.ChangeIndex; 
            AssignValuesFromElementToEditor(elem);

            //DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(CurrentElement.ChangeTableName());
            //UpdateUndoRedoVisibility(changeTableView, CurrentElement.ChangeIndex);

        }
        #endregion
        #region Voids
        public override void Save()
        {
            //throw new NotImplementedException();
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
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
        }
      

        public void AssignValuesFromElementToEditor(ChildElement element)
        {
            RatingCurveElement elem = (RatingCurveElement)element;
            Name = elem.Name;
            LastEditDate = elem.LastEditDate;
            Description = elem.Description;
            Curve = elem.Curve;
        }

        public void AssignValuesFromEditorToCurrentElement()
        {
            CurrentElement.LastEditDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            CurrentElement.Name = Name;
           ((RatingCurveElement) CurrentElement).Description = Description;
            ((RatingCurveElement)CurrentElement).Curve = Curve;
        }

        //public OwnedElement CreateNewElement()
        //{
        //    string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
        //    return new RatingCurveElement(Name, editDate, Description, Curve, OwnerNode);
          
        //}
      
        //public Statistics.UncertainCurveDataCollection GetTheElementsCurve()
        //{
        //    return ((RatingCurveElement)CurrentElement).Curve;
        //}
        //public Statistics.UncertainCurveDataCollection GetTheEditorsCurve()
        //{
        //    return Curve;
        //}

        //public void UpdateNameWithNewValue(string name)
        //{
        //    Name = name;
        //}
        #endregion
    }
}
