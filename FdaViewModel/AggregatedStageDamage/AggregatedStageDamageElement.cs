using FdaViewModel.Utilities;
using Model;
using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageElement : Utilities.ChildElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableConstant = "Aggregated Stage Damage Function - ";

        //private string _Description = "";
        //private Statistics.UncertainCurveDataCollection _Curve;
        //private Inventory.DamageCategory.DamageCategoryRowItem _DamageCategory;
        private readonly CreationMethodEnum _Method;
        #endregion
        #region Properties
        public bool CanEdit { get; }
        public CreationMethodEnum Method
        {
            get { return _Method; }
        }
        //public override string GetTableConstant()
        //{
        //    return _TableConstant;
        //}
        //public string Description 
        //{
        //    get { return _Description; }
        //    set { _Description = value; NotifyPropertyChanged(); }
        //}
        //public Statistics.UncertainCurveDataCollection Curve
        //{
        //    get { return _Curve; }
        //    set { _Curve = value; NotifyPropertyChanged(); }
        //}
  
        #endregion
        #region Constructors
            /// <summary>
            /// Stage damage element
            /// </summary>
            /// <param name="name">Name of element</param>
            /// <param name="lastEditDate">Last edit date</param>
            /// <param name="description">Element description</param>
            /// <param name="curve">The curve that represents the stage vs damage for the element</param>
            /// <param name="method">Creation method</param>
        public AggregatedStageDamageElement( string name , string lastEditDate, string description, IFdaFunction curve, CreationMethodEnum method) : base()
        {
            LastEditDate = lastEditDate;
            Name = name;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/StageDamage.png");

            Description = description;
            if(Description == null)
            {
                Description = "";
            }
            Curve = curve;
            _Method = method;
            //add named actions like edit.
            Utilities.NamedAction editDamageCurve = new Utilities.NamedAction();
            editDamageCurve.Header = "Edit Aggregated Stage Damage Relationship";
            editDamageCurve.Action = EditDamageCurve;

            Utilities.NamedAction removeDamageCurve = new Utilities.NamedAction();
            removeDamageCurve.Header = "Remove";
            removeDamageCurve.Action = RemoveElement;

            Utilities.NamedAction renameDamageCurve = new Utilities.NamedAction(this);
            renameDamageCurve.Header = "Rename";
            renameDamageCurve.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(editDamageCurve);
            localActions.Add(removeDamageCurve);
            localActions.Add(renameDamageCurve);


            Actions = localActions;
        }
        #endregion
        #region Voids
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            AggregatedStageDamageElement elem = (AggregatedStageDamageElement)elementToClone;
            return new AggregatedStageDamageElement(elem.Name, elem.LastEditDate, elem.Description, elem.Curve, elem.Method);
        }
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetStageDamageManager().Remove(this);
        }
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != null, "Name cannot be blank.");
        }
        public void EditDamageCurve(object arg1, EventArgs arg2)
        {
            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetStageDamageManager(), this, 
                (editorVM) => CreateElementFromEditor(editorVM), 
                (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                 .WithSiblingRules(this);

            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(this, "Stage - Damage", "Stage", "Damage", actionManager);
            vm.AddSiblingRules( this);

            string title = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(title, vm, "EditStageDamageElement" + Name);
            Navigate(tab, false, true);
            //List<Inventory.DamageCategory.DamageCategoryOwnedElement> damcateleements = GetElementsOfType<Inventory.DamageCategory.DamageCategoryOwnedElement>();
            //Inventory.DamageCategory.DamageCategoryOwnedElement damcatelement = damcateleements.FirstOrDefault();
            //AggregatedStageDamageEditorVM vm = new AggregatedStageDamageEditorVM(this, (foo) => ((Utilities.OwnerElement)_Owner).SaveExistingElement(foo), (bar) => ((Utilities.OwnerElement)_Owner).AddOwnerRules(bar));
            //Navigate(vm, true, true);
            //if (!vm.WasCancled)
            //{
            //    if (!vm.HasError)
            //    {
            //        vm.SaveWhileEditing();
            //    }
            //}
        }
        //public override void Save()
        //{
        //    Curve.toSqliteTable(TableName);
        //}

        //public override object[] RowData()
        //{
        //    return new object[] { Name,LastEditDate, Description, Curve.Distribution, _Method };
        //}

        //public override bool SavesToRow()
        //{
        //    return true;
        //}
        //public override bool SavesToTable()
        //{
        //    return true;
        //}
        //#endregion
        //#region Functions
        //public override string TableName
        //{
        //    get   {  return GetTableConstant() + LastEditDate; }
        //}
        public ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;

            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new AggregatedStageDamageElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve, CreationMethodEnum.UserDefined);
        }
        #endregion


        public override bool Equals(object obj)
        {
            bool retval = true;
            if (obj.GetType() == typeof(AggregatedStageDamageElement))
            {
                AggregatedStageDamageElement elem = (AggregatedStageDamageElement)obj;
                if (!Name.Equals(elem.Name))
                {
                    retval = false;
                }
                if (Description == null && elem.Description != null)
                {
                    retval = false;
                }
                else if (Description != null && !Description.Equals(elem.Description))
                {
                    retval = false;
                }
                if (!LastEditDate.Equals(elem.LastEditDate))
                {
                    retval = false;
                }
                if (Method != elem.Method)
                {
                    retval = false;
                }
                if (!areCurvesEqual(elem.Curve))
                {
                    retval = false;
                }
            }
            else
            {
                retval = false;
            }
            return retval;
        }

        private bool areCurvesEqual(IFdaFunction curve2)
        {
            //todo: Refactor: I just commented this out. I doubt i still need it.
            bool retval = true;
            //if(Curve.GetType() != curve2.GetType())
            //{
            //    return false;
            //}
            //if (Curve.Distribution != curve2.Distribution)
            //{
            //    return false;
            //}
            //if (Curve.XValues.Count != curve2.XValues.Count)
            //{
            //    return false;
            //}
            //if (Curve.YValues.Count != curve2.YValues.Count)
            //{
            //    return false;
            //}
            //double epsilon = .0001;
            //for(int i = 0;i<Curve.XValues.Count;i++)
            //{
            //    if(Math.Abs( Curve.get_X(i)) - Math.Abs(curve2.get_X(i)) > epsilon)
            //    {
            //        return false;
            //    }
            //    ContinuousDistribution y = Curve.get_Y(i);
            //    ContinuousDistribution y2 = curve2.get_Y(i);
            //    if (Math.Abs(y.GetCentralTendency) - Math.Abs(y2.GetCentralTendency)> epsilon)
            //    {
            //        return false;
            //    }
            //    if (Math.Abs(y.GetSampleSize) - Math.Abs( y2.GetSampleSize) > epsilon)
            //    {
            //        return false;
            //    }
            //}

            return retval;
        }

    }
}
