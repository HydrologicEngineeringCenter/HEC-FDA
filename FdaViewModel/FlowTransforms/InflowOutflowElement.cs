using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.FlowTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 10:33:22 AM)]
    public class InflowOutflowElement : Utilities.OwnedElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 10:33:22 AM
        #endregion
        #region Fields
        private const string _TableConstant = "Inflow Outflow - ";

        private string _Description;
        private Statistics.UncertainCurveDataCollection _Curve;
        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return _TableConstant;
        }
        public string Description { get { return _Description; } set { _Description = value; NotifyPropertyChanged(); } }

        public Statistics.UncertainCurveDataCollection InflowOutflowCurve
        {
            get { return _Curve; }
            set { _Curve = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public InflowOutflowElement(string userProvidedName, string description, Statistics.UncertainCurveDataCollection inflowOutflowCurve, BaseFdaElement owner):base(owner)
        {
            Name = userProvidedName;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/InflowOutflowCircle.png");
            
            Description = description;
            if (Description == null) Description = "";
            InflowOutflowCurve = inflowOutflowCurve;

            Utilities.NamedAction editInflowOutflowCurve = new Utilities.NamedAction();
            editInflowOutflowCurve.Header = "Edit Inflow-Outflow Curve";
            editInflowOutflowCurve.Action = EditInflowOutflowCurve;

            Utilities.NamedAction removeInflowOutflowCurve = new Utilities.NamedAction();
            removeInflowOutflowCurve.Header = "Remove";
            removeInflowOutflowCurve.Action = Remove;

            Utilities.NamedAction renameInflowOutflowCurve = new Utilities.NamedAction();
            renameInflowOutflowCurve.Header = "Rename";
            renameInflowOutflowCurve.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(editInflowOutflowCurve);
            localActions.Add(removeInflowOutflowCurve);
            localActions.Add(renameInflowOutflowCurve);



            Actions = localActions;
        }
        #endregion
        #region Voids
        public void EditInflowOutflowCurve(object arg1, EventArgs arg2)
        {
            InflowOutflowEditorVM vm = new InflowOutflowEditorVM(Name, Description, InflowOutflowCurve);
            Navigate(vm, true, true);
            if (!vm.WasCancled)
            {
                if (!vm.HasFatalError)
                {
                    Name = vm.Name;//should i disable this way of renaming? if not i need to check for name conflicts.
                    Description = vm.Description;//is binding two way? is this necessary?
                    InflowOutflowCurve = vm.Curve;
                 
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
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != null, "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != "test", "Name cannot be test.", false);
        }

        public override void Save()
        {
            _Curve.toSqliteTable(TableName);
        }
        public override object[] RowData()
        {
            return new object[] { Name, Description, InflowOutflowCurve.Distribution };
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
