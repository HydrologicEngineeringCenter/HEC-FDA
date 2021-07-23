using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.FrequencyRelationships
{
    class GraphicalFrequencyOwnerElement : Utilities.ParentElement
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
        public GraphicalFrequencyOwnerElement( ) : base()
        {
            Name = "Graphical Flow Frequency Curves";

            Utilities.NamedAction addRatingCurve = new Utilities.NamedAction();
            addRatingCurve.Header = "Create New Graphical Flow Frequency Curve";
            addRatingCurve.Action = AddNewRatingCurve;

            Utilities.NamedAction ImportRatingCurve = new Utilities.NamedAction();
            ImportRatingCurve.Header = "Import Graphical Flow Frequency Curve From ASCII";
            ImportRatingCurve.Action = ImportRatingCurvefromAscii;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
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
        //        return "Graphical Flow Frequency Curves";
        //    }
        //}

        //public override string[] TableColumnNames()
        //{
        //    return new string[] { "Graphical Flow Frequency Curve" };
        //}
        //public override Type[] TableColumnTypes()
        //{
        //    return new Type[] { typeof(string) };
        //}
        //public override void AddElementFromRowData(object[] rowData)
        //{
        //    //
        //}
        #endregion
    }
}
