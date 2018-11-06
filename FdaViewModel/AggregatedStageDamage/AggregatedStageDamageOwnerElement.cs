using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageOwnerElement : Utilities.ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        
        #endregion
        #region Constructors
        public AggregatedStageDamageOwnerElement(Utilities.ParentElement owner) : base(owner)
        {
            Name = "Aggregated Stage Damage Relationships";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addDamageCurve = new Utilities.NamedAction();
            addDamageCurve.Header = "Create New Aggregated Stage Damage Relationship";
            addDamageCurve.Action = AddNewDamageCurve;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addDamageCurve);

            Actions = localActions;

            StudyCache.StageDamageAdded += AddStageDamageElement;
            StudyCache.StageDamageRemoved += RemoveStageDamageElement;
            StudyCache.StageDamageUpdated += UpdateStageDamageElement;
            GUID = Guid.NewGuid();

        }
        #endregion
        #region Voids
        private void UpdateStageDamageElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddStageDamageElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveStageDamageElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        public void AddNewDamageCurve(object arg1, EventArgs arg2)
        {
            double[] xValues = new double[16] { 95, 97, 99, 101, 103, 105, 107, 110, 112, 115, 117, 120, 122, 125, 127, 130 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[16] { new Statistics.None(0), new Statistics.None(0), new Statistics.None(0), new Statistics.None(1), new Statistics.None(3), new Statistics.None(10), new Statistics.None(50), new Statistics.None(1000), new Statistics.None(2000), new Statistics.None(4000), new Statistics.None(8000), new Statistics.None(10000), new Statistics.None(11000), new Statistics.None(11500), new Statistics.None(11750), new Statistics.None(11875) };
            Statistics.UncertainCurveIncreasing defaultCurve = new Statistics.UncertainCurveIncreasing(xValues, yValues, true, true, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetStageDamageManager()
                , (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                 .WithSiblingRules(this)
               .WithParentGuid(this.GUID)
               .WithCanOpenMultipleTimes(true);

            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(defaultCurve, actionManager);
            //StudyCache.AddSiblingRules(vm, this);
            //vm.AddSiblingRules(this);

            Navigate(vm, false, true, "Create Stage Damage");

            //AggregatedStageDamageEditorVM vm = new AggregatedStageDamageEditorVM((foo) => SaveNewElement(foo), (bar) => AddOwnerRules(bar));
            //Navigate(vm, true, true);
            //if (!vm.WasCancled)
            //{
            //    if (!vm.HasError)
            //    {
            //        string creationDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

            //        AggregatedStageDamageElement ele = new AggregatedStageDamageElement( vm.Name, creationDate, vm.Description, vm.Curve, CreationMethodEnum.UserDefined,this);
            //        AddElement(ele);
            //    }
            //}                   
        }
        //public override void AddBaseElements()
        //{
            
        //}
        public override void AddValidationRules()
        {
        }
        #endregion
        #region Functions
        //public override string TableName
        //{
        //    get
        //    {
        //        return "Aggregated Stage Damage Relationships";
        //    }
        //}
        
        //public override string[] TableColumnNames()
        //{
        //    return new string[] { "Name","Last Edit Date","Description", "Curve Uncertainty Type", "Creation Method" };
        //}
        //public override Type[] TableColumnTypes()
        //{
        //    return new Type[] { typeof(string), typeof(string), typeof(string), typeof(string),typeof(string) };
        //}

        public  ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;

            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new AggregatedStageDamageElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve, CreationMethodEnum.UserDefined);
        }
        //public override ChildElement CreateElementFromRowData(object[] rowData)
        //{
        //    Statistics.UncertainCurveDataCollection emptyCurve = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
        //    AggregatedStageDamageElement asd = new AggregatedStageDamageElement((string)rowData[0],(string)rowData[1], (string)rowData[2], emptyCurve, (CreationMethodEnum)Enum.Parse(typeof(CreationMethodEnum), (string)rowData[4]),this);
        //    asd.Curve.fromSqliteTable(asd.TableName);
        //    return asd;
        //}

        //public override void AddElementFromRowData(object[] rowData)
        //{ 
        //    AddElement(CreateElementFromRowData(rowData),false);
        //}
        #endregion
    }
}
