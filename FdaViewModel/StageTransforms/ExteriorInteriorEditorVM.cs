using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities;
using Model;
using Model.Inputs.Functions.ImpactAreaFunctions;
using Statistics;

namespace FdaViewModel.StageTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 10:57:35 AM)]
    public class ExteriorInteriorEditorVM:Utilities.Transactions.TransactionAndMessageBase
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 10:57:35 AM
        #endregion
        #region Fields
        private string _Name = "";
        private string _Description = "";
        private readonly string _Title = "Exterior-Interior Curve";

        private IFdaFunction _Curve;


        #endregion
        #region Properties
        public Action<Utilities.ISaveUndoRedo> SaveAction { get; set; }

        //public int SelectedIndexInUndoList
        //{
        //    set { CurrentElement.ChangeIndex += value + 1; Undo(); }
        //}
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
        public ExteriorInteriorEditorVM(Action<Utilities.ISaveUndoRedo> saveAction, Action<BaseViewModel> ownerValidationRules) :base()
        {
            ownerValidationRules(this);

            //double[] xs = new double[] { 90, 100, 105, 110, 112, 115, 116, 117, 118, 130 };
            //Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(95), new Statistics.None(96), new Statistics.None(100), new Statistics.None(105), new Statistics.None(106), new Statistics.None(107), new Statistics.None(113), new Statistics.None(119), new Statistics.None(120), new Statistics.None(130) };
            //Curve = new Statistics.UncertainCurveIncreasing(xs, yValues, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            List<double> xValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            List<double> yValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            Functions.ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(xValues, yValues);
            Curve = ImpactAreaFunctionFactory.Factory(func, ImpactAreaFunctionEnum.Rating);

            SaveAction = saveAction;

         }

        public ExteriorInteriorEditorVM(ExteriorInteriorElement elem, Action<Utilities.ISaveUndoRedo> saveAction, Action<BaseViewModel> ownerValidationRules) :base(elem)// string name, string description, Statistics.UncertainCurveDataCollection curve):base()
        {
            ownerValidationRules(this);

            CurrentElement = elem;
            //CurrentElement.ChangeIndex = 0;

            AssignValuesFromElementToEditor(elem);

            SaveAction = saveAction;

            //DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(CurrentElement.ChangeTableName());
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
            //UndoElement(this);
        }

        public  void Redo()
        {
            //RedoElement(this);
        }
        public  void SaveWhileEditing()
        {
           // SaveAction(this);
            //_OwnerNode.SaveElementWhileEditing(this);
        }
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
        }

      

        public void AssignValuesFromElementToEditor(ChildElement element)
        {
            ExteriorInteriorElement elem = (ExteriorInteriorElement)element;
            Name = elem.Name;
            LastEditDate = elem.LastEditDate;
            Description = elem.Description;
            Curve = elem.Curve;
        }

        public IFdaFunction GetTheElementsCurve()
        {
            return ((ExteriorInteriorElement)CurrentElement).Curve;
        }

        public IFdaFunction GetTheEditorsCurve()
        {
            return Curve;
        }

        public void AssignValuesFromEditorToCurrentElement()
        {
            CurrentElement.LastEditDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            CurrentElement.Name = Name;
            ((ExteriorInteriorElement)CurrentElement).Description = Description;
            ((ExteriorInteriorElement)CurrentElement).Curve = Curve;
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
        public void UpdateNameWithNewValue(string name)
        {
            Name = name;
        }
        #endregion
        #region Functions
        #endregion
    }
}
