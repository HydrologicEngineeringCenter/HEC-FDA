using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Inventory
{
    class InventoryOwnerElement : Utilities.ParentElement
    {
        #region Notes
        #endregion
        #region Fields    
        #endregion
        #region Properties
       
        #endregion
        #region Constructors
        public InventoryOwnerElement( ) : base()
        {
            Name = StringConstants.ECONOMICS;            
            CustomTreeViewHeader = new CustomHeaderVM(Name);
        }

        #endregion
        #region Voids
        public override void AddValidationRules()
        {

        }
        public void AddBaseElements(Study.FDACache cache)
        {
           
            OccupancyTypes.OccupancyTypesOwnerElement o = new OccupancyTypes.OccupancyTypesOwnerElement();
            this.AddElement(o);
            cache.OccTypeParent = o;

            StructureInventoryOwnerElement sioe = new StructureInventoryOwnerElement();
            this.AddElement(sioe);
            cache.StructureInventoryParent = sioe;

            AggregatedStageDamage.AggregatedStageDamageOwnerElement a = new AggregatedStageDamage.AggregatedStageDamageOwnerElement();
            this.AddElement(a);
            cache.StageDamageParent = a;

        }
       
        #endregion

    }
}
