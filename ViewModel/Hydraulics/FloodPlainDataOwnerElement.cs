using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Hydraulics
{
    class FloodPlainDataOwnerElement : Utilities.ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableName = "Floodplain Data";
        #endregion
        #region Properties
        //public override string GetTableConstant()
        //{
        //    return TableName;
        //}
        //public override string TableName
        //{
        //    get
        //    {
        //        return _TableName;
        //    }
        //}
        #endregion
        #region Constructors
        public FloodPlainDataOwnerElement( ):base()
        {
            Name = _TableName;
        }

        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            
        }
        public  void AddBaseElements()
        {
            GriddedHydraulicsOwnerElement h = new GriddedHydraulicsOwnerElement();
            this.AddElement(h);

            WaterSurfaceProfileOwnerElement w = new WaterSurfaceProfileOwnerElement();
            this.AddElement(w);
        }
        #endregion
        #region Functions
        //public override string[] TableColumnNames()
        //{
        //    throw new NotImplementedException();
        //}

        //public override Type[] TableColumnTypes()
        //{
        //    throw new NotImplementedException();
        //}
        //public override ChildElement CreateElementFromRowData(object[] rowData)
        //{
        //    return null;
        //}
        //public override void AddElementFromRowData(object[] rowData)
        //{
        //    //throw new NotImplementedException();
        //}

        #endregion
    }
}
