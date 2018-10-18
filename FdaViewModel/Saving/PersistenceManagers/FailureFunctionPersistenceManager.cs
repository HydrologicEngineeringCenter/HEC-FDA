using FdaViewModel.GeoTech;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class FailureFunctionPersistenceManager : SavingBase, IPersistable
    {
        private const string _TableConstant = "Failure Function - ";

        public  string TableName
        {
            get { return "Failure Functions"; }
        }

        public  string[] TableColumnNames()
        {
            return new string[] { "Name", "Last Edit Date", "Description", "Associated Levee Feature", "Curve Distribution Type" };
        }

        public  Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };
        }

        public FailureFunctionPersistenceManager(Study.FDACache studyCache)
        {
            StudyCache = studyCache;
        }



        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(FailureFunctionElement))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;
                SaveNewElementToParentTable(element, TableName, TableColumnNames(), TableColumnTypes());
                //save the individual table
                SaveFailureFunctionCurveTable((FailureFunctionElement)element, editDate);
                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCache.AddFailureFunctionElement((FailureFunctionElement)element);
            }
        }

        public void SaveExisting(Utilities.ChildElement element, string oldName, Statistics.UncertainCurveDataCollection oldCurve)
        {
            SaveExistingElement(oldName, oldCurve, element, TableName);
        }

        public List<ChildElement> Load()
        {
            return CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
        }


        private void SaveFailureFunctionCurveTable(FailureFunctionElement element, string lastEditDate)
        {
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            element.Curve.toSqliteTable(_TableConstant + lastEditDate);
        }



        public  ChildElement CreateElementFromRowData(object[] rowData)
        {
            List<LeveeFeatureElement> ele = StudyCache.LeveeElements;// GetElementsOfType<LeveeFeatureElement>();
            LeveeFeatureElement lfe = null;
            foreach (LeveeFeatureElement element in ele)
            {
                if (element.Name == (string)rowData[3])
                {
                    lfe = element;
                }
            }
            Statistics.UncertainCurveDataCollection ucdc = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[4]));
            FailureFunctionElement failure = new FailureFunctionElement((string)rowData[0], (string)rowData[1], (string)rowData[2], ucdc, lfe);
            failure.Curve.fromSqliteTable(failure.TableName);
            return failure;
        }


    }
}
