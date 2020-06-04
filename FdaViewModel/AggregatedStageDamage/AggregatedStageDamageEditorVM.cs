using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities;
using Functions;
using Model;
using Model.Condition.ComputePoint.ImpactAreaFunctions;
using Model.Inputs.Functions.ImpactAreaFunctions;

namespace FdaViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageEditorVM:Utilities.Transactions.TransactionAndMessageBase, ISaveUndoRedo
    {
        #region Notes
        #endregion
        #region Fields
        private string _Name = "";
        private string _Description = "";
        private bool _ReadOnly = false;
        private readonly string _Title = "Aggregated Stage Damage";
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
 
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value;  NotifyPropertyChanged(); }
        }
        public string PlotTitle
        {
            get { return _Title; }
        }
        public IFdaFunction Curve
        {
            get { return _Curve; }
            set { _Curve = value;  NotifyPropertyChanged(); }
        }
        public ChildElement CurrentElement { get; set; }



        #endregion
        #region Constructors
        /// <summary>
        /// Editor View Model for Aggregated Stage Damage
        /// </summary>
        /// <param name="saveAction"></param>
        /// <param name="ownerValidationRules"></param>
        public AggregatedStageDamageEditorVM(Action<Utilities.ISaveUndoRedo> saveAction, Action<BaseViewModel> ownerValidationRules) : base()
        {
            List<double> xValues = new List<double>() { 95, 97, 99, 101, 103, 105, 107, 110, 112, 115, 117, 120, 122, 125, 127, 130 };
            List<double> yValues = new List<double>() { 95, 97, 99, 101, 103, 105, 107, 110, 112, 115, 117, 120, 122, 125, 127, 130 };

            SaveAction = saveAction;
            ownerValidationRules(this);

            ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(xValues, yValues);
            Curve = ImpactAreaFunctionFactory.Factory(func, IFdaFunctionEnum.InteriorStageDamage);
        }

        public AggregatedStageDamageEditorVM(AggregatedStageDamageElement elem, Action<Utilities.ISaveUndoRedo> saveAction, Action<BaseViewModel> ownerValidationRules) :base(elem)
        {
            ownerValidationRules(this);

            SaveAction = saveAction;

            CurrentElement = elem;
            //CurrentElement.ChangeIndex = 0;

            AssignValuesFromElementToEditor(elem);

          
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
        public void UpdateTheUndoRedoRowItems()
        {
            //int currentIndex = CurrentElement.ChangeIndex;
            //RedoRows.Clear();
            //for (int i = currentIndex + 1; i < UndoRedoRows.Count; i++)
            //{
            //    RedoRows.Add(UndoRedoRows[i]);
            //}

        }
        public  void Redo()
        {
            //RedoElement(this);
        }

        public  void SaveWhileEditing()
        {
            //SaveAction(this);
            //_OwnerNode.SaveElementWhileEditing(this);
        }
        public override void AddValidationRules()
        {
            //AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => { if (Name == null) { return false; } else { return !Name.Equals(""); } }, "Name cannot be blank");
        }

        

        public void AssignValuesFromElementToEditor(ChildElement element)
        {
            AggregatedStageDamageElement elem = (AggregatedStageDamageElement)element;
            Name = elem.Name;
            LastEditDate = elem.LastEditDate;
            Description = elem.Description;
            Curve = elem.Curve;
        }
        public void AssignValuesFromEditorToCurrentElement()
        {
            CurrentElement.LastEditDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            CurrentElement.Name = Name;
            ((AggregatedStageDamageElement)CurrentElement).Description = Description;
            ((AggregatedStageDamageElement)CurrentElement).Curve = Curve;
        }
       

        public IFdaFunction GetTheElementsCurve()
        {
            return ((AggregatedStageDamageElement)CurrentElement).Curve;
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
