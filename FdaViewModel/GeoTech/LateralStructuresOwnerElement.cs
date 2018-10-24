using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 9:36:00 AM)]
    class LateralStructuresOwnerElement : Utilities.ParentElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 9:36:00 AM
        #endregion
        #region Fields
        #endregion
        #region Properties
       
        #endregion
        #region Constructors
        public LateralStructuresOwnerElement(BaseFdaElement owner) : base(owner)
        {
            Name = "Lateral Structures";
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
        //public override string TableName
        //{
        //    get
        //    {
        //        return "Lateral Structures";
        //    }
        //}

        public  void AddBaseElements()
        {
            LeveeFeatureOwnerElement lf = new LeveeFeatureOwnerElement(this);
            AddElement(lf);

            FailureFunctionOwnerElement ff = new FailureFunctionOwnerElement(this);
            AddElement(ff);
        }

        //public override void AddElementFromRowData(object[] rowData)
        //{
        //    throw new NotImplementedException();
        //}

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        //public override string[] TableColumnNames()
        //{
        //    return new string[] { "Name" };
        //}

        //public override Type[] TableColumnTypes()
        //{
        //    return new Type[] { typeof(string) };
        //}
        //public override bool SavesToTable()
        //{
        //    return false;
        //}
    }
}
