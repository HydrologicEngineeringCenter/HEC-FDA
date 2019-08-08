using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.FlowTransforms
{
    public class InflowOutflowOwnerElement : Utilities.ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        //public override string GetTableConstant()
        //{
        //    return TableName;
        //}
        #endregion
        #region Constructors
        public InflowOutflowOwnerElement( ) : base()
        {
            Name = "Inflow Outflow Relationships";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addInflowOutflow = new Utilities.NamedAction();
            addInflowOutflow.Header = "Create New Inflow Outflow Relationship";
            addInflowOutflow.Action = AddInflowOutflow;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addInflowOutflow);

            Actions = localActions;

            StudyCache.InflowOutflowAdded += AddInflowOutflowElement;
            StudyCache.InflowOutflowRemoved += RemoveInflowOutflowElement;
            StudyCache.InflowOutflowUpdated += UpdateInflowOutflowElement;
            GUID = Guid.NewGuid();

        }


        #endregion
        #region Voids
        private void UpdateInflowOutflowElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddInflowOutflowElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveInflowOutflowElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void ImportFromASCII(object arg1, EventArgs arg2)
        {
            throw new NotImplementedException();
        }

        public void AddInflowOutflow(object arg1, EventArgs arg2)
        {

            double[] xValues = new double[] { 0, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(2000), new Statistics.None(3000), new Statistics.None(4000), new Statistics.None(5000), new Statistics.None(6000), new Statistics.None(7000), new Statistics.None(8000), new Statistics.None(9000), new Statistics.None(10000), new Statistics.None(11000) };
            Statistics.UncertainCurveIncreasing defaultCurve = new Statistics.UncertainCurveIncreasing(xValues, yValues, true, true, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetInflowOutflowManager()
                , (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                //.WithOwnerValidationRules((editorVM, oldName) => AddOwnerRules(editorVM, oldName))
                .WithSaveUndoRedo(saveHelper)
                .WithSiblingRules(this);
               //.WithParentGuid(this.GUID)
               //.WithCanOpenMultipleTimes(true);

            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(defaultCurve, actionManager);
            //vm.ParentGUID = this.GUID;
            //StudyCache.AddSiblingRules(vm, this);
            //vm.AddSiblingRules(this);
            string title = "Create Inflow Outflow";
            DynamicTabVM tab = new DynamicTabVM(title, vm, "NewInflowOutflow" + Name);
            Navigate( tab, false, false);
            //if (!vm.WasCancled)
            //{
            //    if (!vm.HasFatalError)
            //    {

            //        vm.SaveWhileEditing();

            //    }
            //}

        }
      
        public override void AddValidationRules()
        {
            //AddRule(nameof(Name), () => Name != "test", "Name cannot be test.");

            //throw new NotImplementedException();
        }
        #endregion
        #region Functions
    
        public  ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;

            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new InflowOutflowElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve);
           // return null;
        }
        //public override ChildElement CreateElementFromRowData(object[] rowData)
        //{
        //    Statistics.UncertainCurveDataCollection ucdc = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
        //    InflowOutflowElement inout = new InflowOutflowElement((string)rowData[0], (string)rowData[1], (string)rowData[2], ucdc, this);
        //    inout.InflowOutflowCurve.fromSqliteTable(inout.TableName);
        //    return inout;
        //}
        //public override void AddElementFromRowData(object[] rowData)
        //{
            
        //    AddElement(CreateElementFromRowData(rowData),false);
        //}
        #endregion
    }
}
