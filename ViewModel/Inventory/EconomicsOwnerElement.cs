using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory
{
    class InventoryOwnerElement : Utilities.ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableName = "Economics";

     
        #endregion
        #region Properties
       
        #endregion
        #region Constructors
        public InventoryOwnerElement( ) : base()
        {
            Name = _TableName;
            
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);
        }

        #endregion
        #region Voids
        public override void AddValidationRules()
        {

        }
        public void AddBaseElements(Study.FDACache cache)
        {
            //DamageCategory.DamageCategoryOwnedElement d = new DamageCategory.DamageCategoryOwnedElement(this);
            //this.AddElement(d);

            OccupancyTypes.OccupancyTypesOwnerElement o = new OccupancyTypes.OccupancyTypesOwnerElement();
            this.AddElement(o);
            cache.OccTypeParent = o;

            StructureInventoryOwnerElement sioe = new StructureInventoryOwnerElement();
            this.AddElement(sioe);

            AggregatedStageDamage.AggregatedStageDamageOwnerElement a = new AggregatedStageDamage.AggregatedStageDamageOwnerElement();
            this.AddElement(a);
            cache.StageDamageParent = a;

        }
       
        #endregion
        #region Functions
        //public override bool SavesToTable()
        //{
        //    return false;
        //}
        //public override string[] TableColumnNames()
        //{
        //    return new string[] { "Name" };
        //}

        //public override Type[] TableColumnTypes()
        //{
        //    return new Type[] { typeof(string) };
        //}

        //public override void AddElementFromRowData(object[] rowData)
        //{
        //    //throw new NotImplementedException();
        //}
        #endregion
    }
}
