using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using FdaViewModel.Utilities;

namespace FdaViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 2:00:01 PM)]
    public class FailureFunctionOwnerElement : Utilities.OwnerElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 2:00:01 PM
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
        public FailureFunctionOwnerElement(BaseFdaElement owner):base(owner)
        {
            Name = "Failure Functions";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addFailureFunction = new Utilities.NamedAction();
            addFailureFunction.Header = "Create New Failure Function Curve";
            addFailureFunction.Action = AddNewFailureFunction;

            //Utilities.NamedAction ImportRatingCurve = new Utilities.NamedAction();
            //ImportRatingCurve.Header = "Import Rating Curve From ASCII";
            //ImportRatingCurve.Action = ImportRatingCurvefromAscii;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addFailureFunction);
            //localActions.Add(ImportRatingCurve);

            Actions = localActions;

        }
        #endregion
        #region Voids
        public void AddNewFailureFunction(object arg1, EventArgs arg2)
        {
            List<LeveeFeatureElement> leveeList = GetElementsOfType<LeveeFeatureElement>();
            FailureFunctionEditorVM vm = new FailureFunctionEditorVM((foo) => SaveNewElement(foo), (bar) => AddOwnerRules(bar), leveeList);
            Navigate(vm);
            if (!vm.WasCancled)
            {
                if (!vm.HasError)
                {
                    string creationDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

                    FailureFunctionElement ele = new FailureFunctionElement(vm.Name, creationDate,  vm.Description, vm.Curve,vm.SelectedLateralStructure , this);
                    AddElement(ele);
                    AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(ele.Name, Utilities.Transactions.TransactionEnum.CreateNew, "", nameof(FailureFunctionElement)));
                }
            }
        }
        #endregion
        #region Functions
        #endregion
        public override string TableName
        {
            get
            {
                return "Failure Functions";
            }
        }

        public override void AddBaseElements()
        {
            
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override string[] TableColumnNames()
        {
            return new string[] { "Name", "Last Edit Date", "Description", "Associated Levee Feature", "Curve Distribution Type" };
        }

        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string),typeof(string), typeof(string), typeof(string), typeof(string) };
        }
        public override OwnedElement CreateElementFromEditor(ISaveUndoRedo editorVM)
        {
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new FailureFunctionElement(editorVM.Name, editDate, ((FailureFunctionEditorVM)editorVM).Description, ((FailureFunctionEditorVM)editorVM).Curve, ((FailureFunctionEditorVM)editorVM).SelectedLateralStructure, this);
        }
        public override OwnedElement CreateElementFromRowData(object[] rowData)
        {
            List<LeveeFeatureElement> ele = GetElementsOfType<LeveeFeatureElement>();
            LeveeFeatureElement lfe = null;
            foreach (LeveeFeatureElement element in ele)
            {
                if (element.Name == (string)rowData[3])
                {
                    lfe = element;
                }
            }
            Statistics.UncertainCurveDataCollection ucdc = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[4]));
            FailureFunctionElement failure = new FailureFunctionElement((string)rowData[0],(string)rowData[1], (string)rowData[2], ucdc, lfe, this);
            failure.FailureFunctionCurve.fromSqliteTable(failure.TableName);
            return failure;
        }
        public override void AddElement(object[] rowData)
        {
            AddElement(CreateElementFromRowData(rowData), false);
        }
    }
}
