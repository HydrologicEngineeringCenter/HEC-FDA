using FdaViewModel.Utilities;
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
        public AggregatedStageDamageElement( string name , string lastEditDate, string description, Statistics.UncertainCurveDataCollection curve, CreationMethodEnum method) : base()
        {
            LastEditDate = lastEditDate;
            Name = name;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/StageDamage.png");

            Description = description;
            //DamageCategory = damagecategory;
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
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetStageDamageManager()
                ,this, (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                 .WithSiblingRules(this)
               .WithParentGuid(this.GUID)
               .WithCanOpenMultipleTimes(false);

            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(this, actionManager);
            vm.AddSiblingRules( this);

            Navigate(vm, false, true, "Create Stage Damage");
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
    }
}
