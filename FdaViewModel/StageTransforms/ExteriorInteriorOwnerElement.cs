using FdaViewModel.Utilities;
using Model;
using Model.Inputs.Functions.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.StageTransforms
{
    public class ExteriorInteriorOwnerElement : Utilities.ParentElement
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
        public ExteriorInteriorOwnerElement( ) : base()
        {
            Name = "Exterior Interior Relationships";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addExteriorInterior = new Utilities.NamedAction();
            addExteriorInterior.Header = "Create New Exterior Interior Relationship";
            addExteriorInterior.Action = AddNewExteriorInteriorCurve;

            //Utilities.NamedAction ImportFromAscii = new Utilities.NamedAction();
            //ImportFromAscii.Header = "Import Exterior Interior Relationship From ASCII";
            //ImportFromAscii.Action = ImportFromASCII;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addExteriorInterior);
            //localActions.Add(ImportFromAscii);

            Actions = localActions;

            StudyCache.ExteriorInteriorAdded += AddExteriorInteriorElement;
            StudyCache.ExteriorInteriorRemoved += RemoveExteriorInteriorElement;
            StudyCache.ExteriorInteriorUpdated += UpdateExteriorInteriorElement;
        }
        #endregion
        #region Voids
        private void UpdateExteriorInteriorElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddExteriorInteriorElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveExteriorInteriorElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void ImportFromASCII(object arg1, EventArgs arg2)
        {
            throw new NotImplementedException();
        }

        public void AddNewExteriorInteriorCurve(object arg1, EventArgs arg2)
        {
            List<double> xValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            List<double> yValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            Functions.ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(xValues, yValues);
            IFdaFunction defaultCurve = ImpactAreaFunctionFactory.Factory(func, ImpactAreaFunctionEnum.Rating);

            //double[] xValues = new double[] { 90, 100, 105, 110, 112, 115, 116, 117, 118, 130 };
            //Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(95), new Statistics.None(96), new Statistics.None(100), new Statistics.None(105), new Statistics.None(106), new Statistics.None(107), new Statistics.None(113), new Statistics.None(119), new Statistics.None(120), new Statistics.None(130) };
            //Statistics.UncertainCurveIncreasing defaultCurve = new Statistics.UncertainCurveIncreasing(xValues, yValues, true, true, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetExteriorInteriorManager()
                , (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                .WithSiblingRules(this);
               //.WithParentGuid(this.GUID)
               //.WithCanOpenMultipleTimes(true);

            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(defaultCurve, actionManager);
            //StudyCache.AddSiblingRules(vm, this);
            //vm.AddSiblingRules(this);
            string header = "Create Exterior Interior";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateExteriorInterior");
            Navigate(tab, false, true);

            //ExteriorInteriorEditorVM vm = new ExteriorInteriorEditorVM((foo) => SaveNewElement(foo), (bar) => AddOwnerRules(bar));
            //Navigate(vm);
            //if (!vm.WasCancled)
            //{
            //    if (!vm.HasError)
            //    {
            //        string creationDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

            //        ExteriorInteriorElement ele = new ExteriorInteriorElement(vm.Name,creationDate, vm.Description, vm.Curve, this);
            //        AddElement(ele);
            //        AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(ele.Name, Utilities.Transactions.TransactionEnum.CreateNew, "", nameof(ExteriorInteriorElement)));
            //    }
            //}
        }
       
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
        #endregion
        #region Functions
        //public override string TableName
        //{
        //    get
        //    {
        //        return "Interior Exterior Curves";
        //    }
        //}
        //public override string[] TableColumnNames()
        //{
        //    return new string[] { "Name","Last Edit Date", "Description", "Curve Distribution Type" };
        //}
        //public override Type[] TableColumnTypes()
        //{
        //    return new Type[] { typeof(string),typeof(string), typeof(string), typeof(string) };
        //}
        public  ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;

            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new ExteriorInteriorElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve);
            //return null;
        }
        //public override ChildElement CreateElementFromRowData(object[] rowData)
        //{
        //    Statistics.UncertainCurveDataCollection ucdc = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
        //    ExteriorInteriorElement ele = new ExteriorInteriorElement((string)rowData[0],(string)rowData[1], (string)rowData[2], ucdc, this);
        //    ele.ExteriorInteriorCurve.fromSqliteTable(ele.TableName);
        //    return ele;
        //}
        //public override void AddElementFromRowData(object[] rowData)
        //{
            
        //    AddElement(CreateElementFromRowData(rowData),false);
        //}
        #endregion
    }
}
