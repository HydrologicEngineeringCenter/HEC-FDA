using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyOwnerElement : Utilities.OwnerElement
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
        public AnalyticalFrequencyOwnerElement(Utilities.OwnerElement owner) : base(owner)
        {
            Name = "Analyitical Flow Frequency Curves";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);
            Utilities.NamedAction addRatingCurve = new Utilities.NamedAction();
            addRatingCurve.Header = "Create New Analyitical Flow Frequency Curve";
            addRatingCurve.Action = AddNewFlowFrequencyCurve;

            Utilities.NamedAction ImportRatingCurve = new Utilities.NamedAction();
            ImportRatingCurve.Header = "Import Analyitical Flow Frequency Curve From ASCII";
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

        public void AddNewFlowFrequencyCurve(object arg1, EventArgs arg2)
        {
            AnalyticalFrequencyEditorVM vm = new AnalyticalFrequencyEditorVM();
            Navigate(vm);
            if (!vm.WasCancled)
            {
                if (!vm.HasError)
                {
                    string creationDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

                    Statistics.LogPearsonIII lpiii = new Statistics.LogPearsonIII(vm.Mean, vm.StandardDeviation, vm.Skew, vm.SampleSize);//are the default probabilities editable in the model?
                    AnalyticalFrequencyElement afe = new AnalyticalFrequencyElement(vm.Name, creationDate, vm.Description, lpiii, this);
                    AddElement(afe);
                    AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(afe.Name, Utilities.Transactions.TransactionEnum.CreateNew, "Initial Name: " + afe.Name + " Description: " + afe.Description + " Mean: " + afe.Distribution.GetMean + " Standard Deviation: " + afe.Distribution.GetStDev + " Skew: " + afe.Distribution.GetG + " EYOR: " + afe.Distribution.GetSampleSize, nameof(AnalyticalFrequencyElement)));
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
            get
            {
                return "Analyitical Frequency Curves";
            }
        }
        public override string[] TableColumnNames()
        {
            return new string[] { "Name","Last Edit Date", "Description", "Mean (of Log)", "Standard Deviation (of Log)", "Skew (of Log)", "Equivalent Years of Record" };
        }
        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string),typeof(string), typeof(string), typeof(double), typeof(double), typeof(double), typeof(int) };
        }
        public override OwnedElement CreateElementFromEditor(ISaveUndoRedo editorVM)
        {
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new AnalyticalFrequencyElement(editorVM.Name, editDate, ((AnalyticalFrequencyEditorVM)editorVM).Description, ((AnalyticalFrequencyEditorVM)editorVM).Distribution, this);
        }
        public override OwnedElement CreateElementFromRowData(object[] rowData)
        {
            double mean = (double)rowData[3];
            double stdev = (double)rowData[4];
            double skew = (double)rowData[5];
            Int64 n = (Int64)rowData[6];
            return new AnalyticalFrequencyElement((string)rowData[0], (string)rowData[1], (string)rowData[2], new Statistics.LogPearsonIII(mean, stdev, skew, (int)n), this);

        }

        public override void AddElement(object[] rowData)
        {
            AddElement(CreateElementFromRowData(rowData),false);
        }
        #endregion
    }
}
