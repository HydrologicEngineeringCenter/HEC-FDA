using FdaViewModel.StageTransforms;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class RatingElementPersistenceManager :SavingBase, IPersistable
    {
        private const string TableName = "Rating Curves";
        private const string ChangeTableName = "RATING_CHANGE_TABLE";
        private const string TABLE_NAME_CONSTANT = "Rating Curve - ";

        public static string[] TableColumnNames()
        {
            return new string[] { "Rating Curve Name", "Last Edit Date", "Description", "Curve Distribution Type", "Curve Type" };
        }
        public static Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };
        }


        public RatingElementPersistenceManager(Study.FDACache studyCache)
        {
            StudyCache = studyCache;
        }


        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(RatingCurveElement))
            {
            string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;
                SaveNewElementToParentTable(element, TableName, TableColumnNames(), TableColumnTypes());
                //save the individual table
                SaveRatingCurveTable((RatingCurveElement)element, editDate);
                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCache.AddRatingElement((RatingCurveElement)element);
            }
        }

        public void SaveExisting(Utilities.ChildElement element, string oldName, Statistics.UncertainCurveDataCollection oldCurve)
        {
            SaveExistingElement(oldName, oldCurve, element, TableName);
        }

        public List<ChildElement> Load()
        {
            return CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
        }

        private void SaveRatingCurveTable(RatingCurveElement element, string lastEditDate)
        {
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            element.Curve.toSqliteTable(TABLE_NAME_CONSTANT + lastEditDate); 
        }

        public ChildElement CreateElementFromRowData(object[] rowData)
        {
            Statistics.UncertainCurveIncreasing emptyCurve = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            RatingCurveElement rc = new RatingCurveElement((string)rowData[0], (string)rowData[1], (string)rowData[2], emptyCurve);
            //loads the curve with the values from it's table
            rc.Curve.fromSqliteTable(rc.TableName);
            return rc;
        }

    }
}
