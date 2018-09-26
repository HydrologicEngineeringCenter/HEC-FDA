using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.GeoTech
{
    class LeveeFeatureOwnerElement : Utilities.OwnerElement
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
        public LeveeFeatureOwnerElement(Utilities.OwnerElement owner) : base(owner)
        {
            Name = "Levee Features";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);


            Utilities.NamedAction add = new Utilities.NamedAction();
            add.Header = "Create New Levee Feature";
            add.Action = AddNewLeveeFeature;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(add);

            Actions = localActions;
        }
        #endregion
        #region Voids
        public void AddNewLeveeFeature(object arg1, EventArgs arg2)
        {
            LeveeFeatureEditorVM vm = new LeveeFeatureEditorVM();
            Navigate(vm);
            if (!vm.WasCancled)
            {
                if (!vm.HasError)
                {
                    LeveeFeatureElement ele = new LeveeFeatureElement(vm.Name, vm.Description, vm.Elevation, this);
                    AddElement(ele);
                    AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(ele.Name, Utilities.Transactions.TransactionEnum.CreateNew, "", nameof(LeveeFeatureElement)));
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
                return "Levee Features";
            }
        }
        public override string[] TableColumnNames()
        {
            return new string[] { "Levee Feature", "Description", "Elevation" };
        }
        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(double) };
        }
        public override OwnedElement CreateElementFromRowData(object[] rowData)
        {
            return new LeveeFeatureElement((string)rowData[0], (string)rowData[1], (double)rowData[2], this);
        }

        public override void AddElement(object[] rowData)
        {
            AddElement(CreateElementFromRowData(rowData),false);
        }
        #endregion
    }
}
