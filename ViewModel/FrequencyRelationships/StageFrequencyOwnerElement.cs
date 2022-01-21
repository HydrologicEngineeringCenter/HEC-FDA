using ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.FrequencyRelationships
{
    class StageFrequencyOwnerElement : Utilities.ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        //public override string GetTableConstant()
        //{
        //    return TableName;
        //}
        #endregion
        #region Constructors
        public StageFrequencyOwnerElement( ) : base()
        {
            Name = "Stage Frequency Curves";

            NamedAction addRatingCurve = new NamedAction();
            addRatingCurve.Header = "Create New Stage Frequency Curve";
            addRatingCurve.Action = AddNewRatingCurve;

            NamedAction ImportRatingCurve = new NamedAction();
            ImportRatingCurve.Header = StringConstants.ImportFromOldFda("Stage Frequency Curve");
            ImportRatingCurve.Action = ImportRatingCurvefromAscii;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addRatingCurve);
            localActions.Add(ImportRatingCurve);

            Actions = localActions;
        }


        #endregion
        #region Voids
        private void ImportRatingCurvefromAscii(object arg1, EventArgs arg2)
        {
            throw new NotImplementedException();
        }

        private void AddNewRatingCurve(object arg1, EventArgs arg2)
        {
            throw new NotImplementedException();
        }
        
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
        #endregion
        #region Functions
        //public override string TableName
        //{
        //    get
        //    {
        //        return "Stage Frequency Curves";
        //    }
        //}
        //public override string[] TableColumnNames()
        //{
        //    return new string[] { "Stage Frequency Curve" };
        //}
        //public override Type[] TableColumnTypes()
        //{
        //    return new Type[] { typeof(string) };
        //}

        //public override ChildElement CreateElementFromRowData(object[] rowData)
        //{
        //    return null;
        //}

        //public override void AddElementFromRowData(object[] rowData)
        //{
        //    //AddElement(new )
        //}
        #endregion
    }
}
