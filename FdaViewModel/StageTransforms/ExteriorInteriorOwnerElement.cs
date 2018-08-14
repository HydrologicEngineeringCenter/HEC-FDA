using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.StageTransforms
{
    class ExteriorInteriorOwnerElement : Utilities.OwnerElement
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
        public ExteriorInteriorOwnerElement(BaseFdaElement owner) : base(owner)
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
        }
        #endregion
        #region Voids
        private void ImportFromASCII(object arg1, EventArgs arg2)
        {
            throw new NotImplementedException();
        }

        public void AddNewExteriorInteriorCurve(object arg1, EventArgs arg2)
        {
            ExteriorInteriorEditorVM vm = new ExteriorInteriorEditorVM();
            Navigate(vm);
            if (!vm.WasCancled)
            {
                if (!vm.HasError)
                {
                    ExteriorInteriorElement ele = new ExteriorInteriorElement(vm.Name, vm.Description, vm.Curve, this);
                    AddElement(ele);
                    AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(ele.Name, Utilities.Transactions.TransactionEnum.CreateNew, "", nameof(ExteriorInteriorElement)));
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
                return "Interior Exterior Curves";
            }
        }
        public override string[] TableColumnNames()
        {
            return new string[] { "Interior Exterior Curve", "Description", "Curve Distribution Type" };
        }
        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(string) };
        }

        public override void AddElement(object[] rowData)
        {
            Statistics.UncertainCurveDataCollection ucdc = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[2]));
            ExteriorInteriorElement ele = new ExteriorInteriorElement((string)rowData[0], (string)rowData[1],ucdc, this);
            ele.ExteriorInteriorCurve.fromSqliteTable(ele.TableName);
            AddElement(ele,false);
        }
        #endregion
    }
}
