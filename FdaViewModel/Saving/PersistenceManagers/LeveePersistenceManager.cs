using FdaViewModel.GeoTech;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class LeveePersistenceManager : SavingBase, IPersistable
    {
        public  string TableName
        {
            get { return "Levee Features";  }
        }
        public  string[] TableColumnNames()
        {
            return new string[] { "Levee Feature", "Description", "Elevation" };
        }
        public  Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(double) };
        }


        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(LeveeFeatureElement))
            {
                SaveNewElementToParentTable(element, TableName, TableColumnNames(), TableColumnTypes());
                StudyCache.AddLeveeElement((LeveeFeatureElement)element);
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



        public LeveePersistenceManager(Study.FDACache studyCache)
        {
            StudyCache = studyCache;
        }


        public  ChildElement CreateElementFromRowData(object[] rowData)
        {
            return new LeveeFeatureElement((string)rowData[0], (string)rowData[1], (double)rowData[2]);
        }

    }
}
