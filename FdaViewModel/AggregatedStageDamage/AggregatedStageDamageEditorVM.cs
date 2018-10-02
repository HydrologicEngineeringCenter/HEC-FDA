using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities;
using Statistics;

namespace FdaViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageEditorVM:Utilities.Transactions.TransactionAndMessageBase, Utilities.ISaveUndoRedo
    {
        #region Notes
        #endregion
        #region Fields
        private string _Name = "";
        private string _Description = "";
        private bool _ReadOnly = false;
        private readonly string _Title = "Aggregated Stage Damage";
        private Statistics.UncertainCurveDataCollection _Curve;

        #endregion
        #region Properties
        public Action<Utilities.ISaveUndoRedo> SaveAction { get; set; }


   
       
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
        public Statistics.UncertainCurveDataCollection Curve
        {
            get { return _Curve; }
            set { _Curve = value;  NotifyPropertyChanged(); }
        }
        public OwnedElement CurrentElement { get; set; }



        #endregion
        #region Constructors
        public AggregatedStageDamageEditorVM(Action<Utilities.ISaveUndoRedo> saveAction, Action<BaseViewModel> ownerValidationRules) : base()
        {
            double[] xValues = new double[16] { 95, 97, 99, 101, 103, 105, 107, 110, 112, 115, 117, 120, 122, 125, 127, 130 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[16] { new Statistics.None(0), new Statistics.None(0), new Statistics.None(0), new Statistics.None(1), new Statistics.None(3), new Statistics.None(10), new Statistics.None(50), new Statistics.None(1000), new Statistics.None(2000), new Statistics.None(4000), new Statistics.None(8000), new Statistics.None(10000), new Statistics.None(11000), new Statistics.None(11500), new Statistics.None(11750), new Statistics.None(11875) };

            SaveAction = saveAction;
            ownerValidationRules(this);

            Curve = new Statistics.UncertainCurveIncreasing(xValues, yValues, true, true, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        }

        public AggregatedStageDamageEditorVM(AggregatedStageDamageElement elem, Action<Utilities.ISaveUndoRedo> saveAction, Action<BaseViewModel> ownerValidationRules) :base()
        {
            ownerValidationRules(this);

            SaveAction = saveAction;

            CurrentElement = elem;
            CurrentElement.ChangeIndex = 0;

            AssignValuesFromElementToEditor(elem);

          
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
            //_OwnerNode.SaveElementWhileEditing(this);
        }
        public override void AddValidationRules()
        {
            //AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => { if (Name == null) { return false; } else { return !Name.Equals(""); } }, "Name cannot be blank");
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }

        public void AssignValuesFromElementToEditor(OwnedElement element)
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
       

        public UncertainCurveDataCollection GetTheElementsCurve()
        {
            return ((AggregatedStageDamageElement)CurrentElement).Curve;
        }

        public UncertainCurveDataCollection GetTheEditorsCurve()
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
