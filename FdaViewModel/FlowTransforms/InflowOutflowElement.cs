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
        private InflowOutflowOwnerElement _OwnerNode;
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
        public InflowOutflowElement(string userProvidedName, string lastEditDate, string description, Statistics.UncertainCurveDataCollection inflowOutflowCurve, Utilities.OwnerElement owner):base(owner)
        {
            _OwnerNode = (InflowOutflowOwnerElement)owner;
            LastEditDate = lastEditDate;
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
            AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(Name, Utilities.Transactions.TransactionEnum.EditExisting, 
                "Openning " + Name + " for editing.",nameof(InflowOutflowElement)));

            FdaModel.Utilities.Messager.ErrorMessage err = new FdaModel.Utilities.Messager.ErrorMessage("Test message when opening", FdaModel.Utilities.Messager.ErrorMessageEnum.Report, nameof(InflowOutflowElement));
            FdaModel.Utilities.Messager.Logger.Instance.ReportMessage(err);

            InflowOutflowEditorVM vm = new InflowOutflowEditorVM(this);
            Navigate(vm, true, true);
            if (!vm.WasCancled)
            {
                if (!vm.HasFatalError)
                {

                    LastEditDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

                    string originalName = Name;
                    Statistics.UncertainCurveDataCollection oldCurve = InflowOutflowCurve;

                    Name = vm.Name;
                    Description = vm.Description;
                    InflowOutflowCurve = vm.Curve;
                    ChangeIndex = vm.ChangeIndex;

                    ((InflowOutflowOwnerElement)_Owner).UpdateTableRowIfModified(originalName, this);
                    UpdateTableIfModified(originalName,oldCurve, InflowOutflowCurve);    
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
                return GetTableConstant() + LastEditDate;
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
            return new object[] { Name, LastEditDate, Description, InflowOutflowCurve.Distribution };
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
