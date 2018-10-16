using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Editors;
using FdaViewModel.Utilities;

namespace FdaViewModel.StageTransforms
{
    public class RatingCurveOwnerElement : Utilities.OwnerElement
    {
        #region Notes
        #endregion
        #region Fields

        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return TableName;
        }
        #endregion
        #region Constructors
        public RatingCurveOwnerElement(Utilities.OwnerElement owner) : base(owner)
        {
            Name = "Rating Curves";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addRatingCurve = new Utilities.NamedAction();
            addRatingCurve.Header = "Create New Rating Curve";
            addRatingCurve.Action = AddNewRatingCurve;

            Utilities.NamedAction ImportRatingCurve = new Utilities.NamedAction();
            ImportRatingCurve.Header = "Import Rating Curve From ASCII";
            ImportRatingCurve.Action = ImportRatingCurvefromAscii;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addRatingCurve);
            //localActions.Add(ImportRatingCurve);

            Actions = localActions;
        }


        #endregion
        #region Voids
        private void ImportRatingCurvefromAscii(object arg1, EventArgs arg2)
        {
            throw new NotImplementedException();
        }

      

        public void AddNewRatingCurve(object arg1, EventArgs arg2)
        {

            //create the default curve: xs[]= ys[]=
            double[] xValues = new double[] { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(95), new Statistics.None(96), new Statistics.None(97), new Statistics.None(99), new Statistics.None(104), new Statistics.None(109), new Statistics.None(110), new Statistics.None(114), new Statistics.None(116), new Statistics.None(119), new Statistics.None(120), new Statistics.None(121) };
            Statistics.UncertainCurveIncreasing defaultCurve = new Statistics.UncertainCurveIncreasing(xValues, yValues, true, true, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper( (editorVM, elem) => SaveNewElement(editorVM,elem));
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithOwnerValidationRules((editorVM, oldName) => AddOwnerRules(editorVM, oldName))
                .WithSaveUndoRedo(saveHelper, (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToEditor(editor, element),
                (editor, element) => AssignValuesFromEditorToElement(editor, element));

            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(defaultCurve, actionManager);

            //editorFactory. CreateEditor(defaultCurve)
            //  .withUndoRedo(saveAction)
            //  .withTransactions


            //RatingCurveEditorVM vm = new RatingCurveEditorVM((foo) => SaveNewElement(foo), (bar) => AddOwnerRules(bar));
            Navigate(vm, false, true, "Create Rating Curve");
            if (!vm.WasCanceled)
            {
                if (!vm.HasError)
                {
                   // string creationDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

                   // RatingCurveElement ele = new RatingCurveElement(vm.Name, creationDate, vm.Description, vm.Curve, this);

                    vm.SaveWhileEditing();

                   // AddElement(ele);
                    //AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(ele.Name, Utilities.Transactions.TransactionEnum.CreateNew, "", nameof(RatingCurveElement)));
                }
            }
        }
        public override void AddBaseElements()
        {
            //throw new NotImplementedException();
        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
        #endregion
        #region Functions
       
        public override string TableName
        {
            get  { return "Rating Curves";}
        }
        public override string[] TableColumnNames()
        {
            return new string[] { "Rating Curve Name", "Last Edit Date", "Description", "Curve Distribution Type", "Curve Type" };
        }
        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(string), typeof(string),typeof(string) };
        }

        public override void AssignValuesFromEditorToElement(BaseEditorVM editorVM, OwnedElement element)
        {
            CurveEditorVM vm = (CurveEditorVM)editorVM;
            element.Name = vm.Name;
            element.Description = vm.Description;
            element.Curve = vm.Curve;
            element.UpdateTreeViewHeader(vm.Name);
        }

        public override void AssignValuesFromElementToEditor(BaseEditorVM editorVM, OwnedElement element)
        {
            CurveEditorVM vm = (CurveEditorVM)editorVM;

            vm.Name = element.Name;
            vm.Description = element.Description;
            vm.Curve = element.Curve;
        }

        public override OwnedElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;
            //Editors.CurveEditorVM vm = (Editors.CurveEditorVM)editorVM;
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new RatingCurveElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve, this);
        }


        public override OwnedElement CreateElementFromRowData(object[] rowData)
        {
            Statistics.UncertainCurveIncreasing emptyCurve = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            RatingCurveElement rc = new RatingCurveElement((string)rowData[0], (string)rowData[1], (string)rowData[2], emptyCurve, this);
            //loads the curve with the values from it's table
            rc.Curve.fromSqliteTable(rc.TableName);
            return rc;
        }
        public override void AddElement(object[] rowData)
        {
            
            AddElement(CreateElementFromRowData(rowData),false);

        }
        #endregion
    }
}
