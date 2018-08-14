using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 2:04:38 PM)]
    public class FailureFunctionElement : Utilities.OwnedElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 2:04:38 PM
        #endregion
        #region Fields
        private const string _TableConstant = "Failure Function - ";
        private string _Description;
        private Statistics.UncertainCurveDataCollection _Curve;
        //private List<LeveeFeatureElement> _LateralStructureList;
        private LeveeFeatureElement _SelectedLateralStructure;


        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return _TableConstant;
        }
        public LeveeFeatureElement SelectedLateralStructure
        {
            get { return _SelectedLateralStructure; }
            set { _SelectedLateralStructure = value; NotifyPropertyChanged(); }
        }
        //public List<LeveeFeatureElement> LateralStructureList
        //{
        //    get { return _LateralStructureList; }
        //    set { _LateralStructureList = value; NotifyPropertyChanged(); }
        //}
        public string Description { get { return _Description; } set { _Description = value; NotifyPropertyChanged(); } }

        public Statistics.UncertainCurveDataCollection FailureFunctionCurve
        {
            get { return _Curve; }
            set { _Curve = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public FailureFunctionElement(string userProvidedName, string description, Statistics.UncertainCurveDataCollection failureFunctionCurve, LeveeFeatureElement selectedLatStructure, BaseFdaElement owner) : base(owner)
        {
            Name = userProvidedName;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/FailureFunction.png");

            Description = description;
            if (Description == null) Description = "";
            FailureFunctionCurve = failureFunctionCurve;
            if (selectedLatStructure == null)
            {
                SelectedLateralStructure = new LeveeFeatureElement("", "", 0, this);
            }
            else
            {
                SelectedLateralStructure = selectedLatStructure;
            }

            Utilities.NamedAction editFailureFunctionCurve = new Utilities.NamedAction();
            editFailureFunctionCurve.Header = "Edit Failure Function Curve";
            editFailureFunctionCurve.Action = EditFailureFunctionCurve;

            Utilities.NamedAction removeFailureFunctionCurve = new Utilities.NamedAction();
            removeFailureFunctionCurve.Header = "Remove";
            removeFailureFunctionCurve.Action = Remove;

            Utilities.NamedAction renameElement = new Utilities.NamedAction();
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(editFailureFunctionCurve);
            localActions.Add(removeFailureFunctionCurve);
            localActions.Add(renameElement);


            Actions = localActions;

        }
        #endregion
        #region Voids
        public void EditFailureFunctionCurve(object arg1, EventArgs arg2)
        {
            //get the current list of levees
            List<LeveeFeatureElement> leveeList = GetElementsOfType<LeveeFeatureElement>();
            Statistics.UncertainCurveDataCollection tmp = _Curve.Clone();//necessary because binding..
            FailureFunctionEditorVM vm = new FailureFunctionEditorVM(Name, Description, FailureFunctionCurve, SelectedLateralStructure, leveeList);
            Navigate(vm, true, true);
            if (!vm.WasCancled)
            {
                if (!vm.HasError)
                {
                    //bool hasChanges = false;
                    //if (tmp != FailureFunctionCurve)
                    //{
                    //    //has changes in the curve
                    //    hasChanges = true;
                    //}
                    ////name changes etc.
                    //if (hasChanges)
                    //{
                        AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(vm.Name, Utilities.Transactions.TransactionEnum.EditExisting, ""));
                        Name = vm.Name;//should i disable this way of renaming? if not i need to check for name conflicts.
                        CustomTreeViewHeader = new Utilities.CustomHeaderVM(vm.Name, "pack://application:,,,/Fda;component/Resources/FailureFunction.png");
                        Description = vm.Description;//is binding two way? is this necessary?
                        FailureFunctionCurve = vm.Curve;//is binding two way? is this necessary?
                        SelectedLateralStructure = vm.SelectedLateralStructure;
                        Save();
                    //}

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
                return GetTableConstant() + Name;
            }
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            _Curve.toSqliteTable(TableName);
        }

        public override object[] RowData()
        {
            return new object[] { Name, Description, _SelectedLateralStructure.Name, FailureFunctionCurve.Distribution };
        }

        public override bool SavesToRow()
        {
            return true;
        }
        public override bool SavesToTable()
        {
            return true;
        }
    }
}
