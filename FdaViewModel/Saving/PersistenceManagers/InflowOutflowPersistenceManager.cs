using FdaViewModel.FlowTransforms;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class InflowOutflowPersistenceManager : SavingBase, IPersistable
    {
        private const string _TableConstant = "Inflow Outflow - ";
        public  string TableName
        {
            get { return "Inflow Outflow Relationships";   }
        }

        public string[] TableColumnNames()
        {
            return new string[] { "Name", "Last Edit Date", "Description", "Curve Distribution Type" };
        }
        public Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(string), typeof(string) };
        }

        public InflowOutflowPersistenceManager(Study.FDACache studyCache)
        {
            StudyCache = studyCache;
        }



        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(InflowOutflowElement))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;
                SaveNewElementToParentTable(element, TableName, TableColumnNames(), TableColumnTypes());
                //save the individual table
                SaveFlowFreqCurveTable((InflowOutflowElement)element, editDate);
                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCache.AddInflowOutflowElement((InflowOutflowElement)element);
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


        private void SaveFlowFreqCurveTable(InflowOutflowElement element, string lastEditDate)
        {
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            element.Curve.toSqliteTable(_TableConstant + lastEditDate);
        }




        public ChildElement CreateElementFromRowData(object[] rowData)
        {
            Statistics.UncertainCurveDataCollection ucdc = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            InflowOutflowElement inout = new InflowOutflowElement((string)rowData[0], (string)rowData[1], (string)rowData[2], ucdc);
            inout.InflowOutflowCurve.fromSqliteTable(inout.TableName);
            return inout;
        }

    }
}
