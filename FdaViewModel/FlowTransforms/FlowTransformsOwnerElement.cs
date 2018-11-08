using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.FlowTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 9:23:47 AM)]
    class FlowTransformsOwnerElement : Utilities.ParentElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 9:23:47 AM
        #endregion
        #region Fields
        #endregion
        #region Properties
       
        #endregion
        #region Constructors
        public FlowTransformsOwnerElement(BaseFdaElement owner) : base(owner)
        {
            Name = "Flow Transforms";
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
        //        return "Flow Transforms";
        //    }
        //}

        public  void AddBaseElements(Study.FDACache cache)
        {
            InflowOutflowOwnerElement io = new InflowOutflowOwnerElement(this);
            AddElement(io);
            cache.InflowOutflowParent = io;
        }
        //public override void Save()
        //{
        //    foreach (Utilities.ChildElement ele in _Elements)
        //    {
        //        ele.Save();
        //    }
        //}
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }


        //public override string[] TableColumnNames()
        //{
        //    throw new NotImplementedException();
        //}

        //public override Type[] TableColumnTypes()
        //{
        //    throw new NotImplementedException();
        //}
        //public override bool SavesToTable()
        //{
        //    return false;
        //}

        //public override void AddElementFromRowData(object[] rowData)
        //{
        //    //throw new NotImplementedException();
        //}
    }
}
