using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory
{
    class InventoryOwnerElement : Utilities.OwnerElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableName = "Economics";

        public override string TableName
        {
            get
            {
                return _TableName;
            }
        }
        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return TableName;
        }
        #endregion
        #region Constructors
        public InventoryOwnerElement(BaseFdaElement owner) : base(owner)
        {
            Name = _TableName;
            
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);
        }

        #endregion
        #region Voids
        public override void AddValidationRules()
        {

        }
        public override void AddBaseElements()
        {
            //DamageCategory.DamageCategoryOwnedElement d = new DamageCategory.DamageCategoryOwnedElement(this);
            //this.AddElement(d);

            OccupancyTypes.OccupancyTypesOwnerElement o = new OccupancyTypes.OccupancyTypesOwnerElement(this);
            this.AddElement(o);

            StructureInventoryOwnerElement sioe = new StructureInventoryOwnerElement(this);
            this.AddElement(sioe);

            AggregatedStageDamage.AggregatedStageDamageOwnerElement a = new AggregatedStageDamage.AggregatedStageDamageOwnerElement(this);
            this.AddElement(a);

        }
        public override void Save()
        {
            foreach (Utilities.OwnedElement ele in _Elements)
            {
                ele.Save();
            }
        }
        #endregion
        #region Functions
        public override bool SavesToTable()
        {
            return false;
        }
        public override string[] TableColumnNames()
        {
            return new string[] { "Name" };
        }

        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string) };
        }

        public override void AddElement(object[] rowData)
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
