using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.FrequencyRelationships
{
    class FrequencyRelationshipsOwnerElement : Utilities.ParentElement
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
        public FrequencyRelationshipsOwnerElement( ) : base()
        {
            Name = "Frequency Relationships";
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);
            //Utilities.NamedAction add = new Utilities.NamedAction();
            //add.Header = "Create New Levee Feature";
            //add.Action = AddNewLeveeFeature;

            //List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            //localActions.Add(add);

            //Actions = localActions;
        }
        #endregion
        #region Voids
        public  void AddBaseElements(Study.FDACache cache)
        {
            AnalyticalFrequencyOwnerElement r = new AnalyticalFrequencyOwnerElement();
            AddElement(r);
            cache.FlowFrequencyParent = r;

            GraphicalFrequencyOwnerElement i = new GraphicalFrequencyOwnerElement();
            //AddElement(i);

            StageFrequencyOwnerElement s = new StageFrequencyOwnerElement();
            //AddElement(s);
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
        //        return "";
        //    }
        //}
        //public override void Save()
        //{
        //    foreach (Utilities.ChildElement ele in _Elements)
        //    {
        //        ele.Save();
        //    }
        //}

        //public override string[] TableColumnNames()
        //{
        //    throw new NotImplementedException();
        //}
        //public override Type[] TableColumnTypes()
        //{
        //    throw new NotImplementedException();
        //}
        //public override void AddElementFromRowData(object[] rowData)
        //{
        //    //AddElement(new InflowOutflowElement((string)rowData[0], (string)rowData[1], new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None), this));
        //}
        //public override bool SavesToTable()
        //{
        //    return false;
        //}
        #endregion
    }
}
