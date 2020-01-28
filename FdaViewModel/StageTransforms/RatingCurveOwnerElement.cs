using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Editors;
using FdaViewModel.Utilities;
using Model;
using Model.Inputs.Functions.ImpactAreaFunctions;

namespace FdaViewModel.StageTransforms
{
    public class RatingCurveOwnerElement : Utilities.ParentElement
    {
        #region Notes
        #endregion
        #region Fields

        #endregion
        #region Properties
        
        #endregion
        #region Constructors
        public RatingCurveOwnerElement( ) : base()
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

            StudyCache.RatingAdded += AddRatingCurveElement;
            StudyCache.RatingRemoved += RemoveRatingCurveElement;
            StudyCache.RatingUpdated += UpdateRatingCurveElement;
        }

        #endregion
        #region Voids
        private void UpdateRatingCurveElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddRatingCurveElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveRatingCurveElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);        
        }
        private void ImportRatingCurvefromAscii(object arg1, EventArgs arg2)
        {
            throw new NotImplementedException();
        }

      

        public void AddNewRatingCurve(object arg1, EventArgs arg2)
        {
            List<double> xValues = new List<double>() { 1000, 10000, 15000 };//, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            List<double> yValues = new List<double>() { 1000, 10000, 15000 };//, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            Functions.ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(xValues, yValues);
            IFdaFunction defaultCurve = ImpactAreaFunctionFactory.Factory(func, ImpactAreaFunctionEnum.Rating);
            //create the default curve: 
            //double[] xValues = new double[] { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            //Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(95), new Statistics.None(96), new Statistics.None(97), new Statistics.None(99), new Statistics.None(104), new Statistics.None(109), new Statistics.None(110), new Statistics.None(114), new Statistics.None(116), new Statistics.None(119), new Statistics.None(120), new Statistics.None(121) };
            //Statistics.UncertainCurveIncreasing defaultCurve = new Statistics.UncertainCurveIncreasing(xValues, yValues, true, true, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper( Saving.PersistenceFactory.GetRatingManager()
                , (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                .WithSiblingRules(this);
              // .WithParentGuid(this.GUID)
               //.WithCanOpenMultipleTimes(true);

            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(defaultCurve, "Outflow - Exterior Stage", "Outflow", "Exterior Stage", actionManager);
            //StudyCache.AddSiblingRules(vm,this);
            //vm.AddSiblingRules(this);
            string header = "Create Rating Curve " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateRatingCurve");
            Navigate(tab, false, true);
            
        }
        //public override void AddBaseElements()
        //{
        //    //throw new NotImplementedException();
        //}
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
        #endregion
        #region Functions
       
       
        //public override string[] TableColumnNames()
        //{
        //    return new string[] { "Rating Curve Name", "Last Edit Date", "Description", "Curve Distribution Type", "Curve Type" };
        //}
        //public override Type[] TableColumnTypes()
        //{
        //    return new Type[] { typeof(string), typeof(string), typeof(string), typeof(string),typeof(string) };
        //}

       

        public  ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;
            //Editors.CurveEditorVM vm = (Editors.CurveEditorVM)editorVM;
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new RatingCurveElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve);
        }


        //public override ChildElement CreateElementFromRowData(object[] rowData)
        //{
        //    Statistics.UncertainCurveIncreasing emptyCurve = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
        //    RatingCurveElement rc = new RatingCurveElement((string)rowData[0], (string)rowData[1], (string)rowData[2], emptyCurve, this);
        //    //loads the curve with the values from it's table
        //    rc.Curve.fromSqliteTable(rc.TableName);
        //    return rc;
        //}
        //public override void AddElementFromRowData(object[] rowData)
        //{
            
        //    AddElement(CreateElementFromRowData(rowData),false);

        //}
        #endregion
    }
}
