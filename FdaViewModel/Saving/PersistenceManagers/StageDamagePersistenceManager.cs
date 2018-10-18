using FdaViewModel.AggregatedStageDamage;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class StageDamagePersistenceManager : SavingBase, IPersistable
    {
        private const string _TableConstant = "Aggregated Stage Damage Function - ";
        public  string TableName
        {
            get {  return "Aggregated Stage Damage Relationships"; }
        }

        public  string[] TableColumnNames()
        {
            return new string[] { "Name", "Last Edit Date", "Description", "Curve Uncertainty Type", "Creation Method" };
        }
        public  Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };
        }

        public StageDamagePersistenceManager(Study.FDACache studyCache)
        {
            StudyCache = studyCache;
        }


        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(AggregatedStageDamageElement))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;
                SaveNewElementToParentTable(element, TableName, TableColumnNames(), TableColumnTypes());
                //save the individual table
                SaveFlowFreqCurveTable((AggregatedStageDamageElement)element, editDate);
                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCache.AddStageDamageElement((AggregatedStageDamageElement)element);
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


        private void SaveFlowFreqCurveTable(AggregatedStageDamageElement element, string lastEditDate)
        {
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            element.Curve.toSqliteTable(_TableConstant + lastEditDate);
        }




        public  ChildElement CreateElementFromRowData(object[] rowData)
        {
            Statistics.UncertainCurveDataCollection emptyCurve = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            AggregatedStageDamageElement asd = new AggregatedStageDamageElement((string)rowData[0], (string)rowData[1], (string)rowData[2], emptyCurve, (CreationMethodEnum)Enum.Parse(typeof(CreationMethodEnum), (string)rowData[4]));
            asd.Curve.fromSqliteTable(asd.TableName);
            return asd;
        }

    }
}
