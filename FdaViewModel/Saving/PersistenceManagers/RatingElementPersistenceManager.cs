using FdaViewModel.StageTransforms;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class RatingElementPersistenceManager :SavingBase, IPersistableWithUndoRedo
    {
        private const string TableName = "Rating Curves";
        internal override string ChangeTableConstant { get { return "Rating Curve - "; } }    
        private static readonly string[] TableColumnNames =  { "Rating Curve Name", "Last Edit Date", "Description", "Curve Distribution Type", "Curve Type", "Curve" };   
        private static readonly Type[] TableColumnTypes = { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };


        #region constructor
        public RatingElementPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #endregion

        #region utilities
        private object[] GetRowDataFromElement(RatingCurveElement element)
        {
            if(element.Description == null)
            {
                element.Description = "";
            }
            return new object[] { element.Name, element.LastEditDate, element.Description, element.Curve.Distribution, element.Curve.GetType(), ExtentionMethods.CreateXMLCurveString(element.Curve.Distribution, element.Curve.XValues, element.Curve.YValues) };

        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            Statistics.UncertainCurveIncreasing emptyCurve = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            //Statistics.UncertainCurveIncreasing ratingCurve = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]),);

            RatingCurveElement rc = new RatingCurveElement((string)rowData[0], (string)rowData[1], (string)rowData[2], emptyCurve);
            //loads the curve with the values from it's table
            //rc.Curve.fromSqliteTable(ChangeTableConstant+ (string)rowData[1]);
            rc.Curve = ExtentionMethods.GetCurveFromXMLString((string)rowData[5], (Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            return rc;
        }

        #endregion

        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(RatingCurveElement))
            {
                 string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;


                SaveNewElementToParentTable(GetRowDataFromElement((RatingCurveElement)element), TableName, TableColumnNames, TableColumnTypes);
                SaveElementToChangeTable(element.Name, GetRowDataFromElement((RatingCurveElement)element), ChangeTableConstant, TableColumnNames, TableColumnTypes);
                //SaveCurveTable(element.Curve, ChangeTableConstant, editDate);


                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCacheForSaving.AddRatingElement((RatingCurveElement)element);
            }
        }

        public void Remove(ChildElement element)
        {
            //RequestNavigation += Navigate;

            RemoveFromParentTable(element, TableName);
            DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            StudyCacheForSaving.RemoveRatingElement((RatingCurveElement)element);

        }

        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave,int changeTableIndex )
        {
            string editDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = editDate;

            if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((RatingCurveElement)elementToSave), oldElement.Name, TableName) || AreCurvesDifferent(oldElement.Curve,elementToSave.Curve))
            {
                UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((RatingCurveElement)elementToSave), oldElement.Name, TableName, true, ChangeTableConstant);
                //SaveCurveTable(elementToSave.Curve, ChangeTableConstant, editDate);
                // update the existing element. This will actually remove the old element and do an insert at that location with the new element.
                StudyCacheForSaving.UpdateRatingCurve((RatingCurveElement)oldElement, (RatingCurveElement)elementToSave);
            }

        }

        public List<ChildElement> Load()
        {
            return CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
        }
      


    }
}
