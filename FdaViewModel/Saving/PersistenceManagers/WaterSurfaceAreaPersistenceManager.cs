using FdaViewModel.Utilities;
using FdaViewModel.WaterSurfaceElevation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class WaterSurfaceAreaPersistenceManager : SavingBase, IPersistable
    {
        private const string TableName = "Water Surface Elevations";

        public  string[] TableColumnNames()
        {
            return new string[] { "Name", "Description", "IsDepthGrids" };
        }

        public  Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(bool) };
        }

        public WaterSurfaceAreaPersistenceManager(Study.FDACache studyCache)
        {
            StudyCache = studyCache;
        }


        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(WaterSurfaceElevationElement))
            {
                SaveNewElementToParentTable(element, TableName, TableColumnNames(), TableColumnTypes());
                StudyCache.AddWaterSurfaceElevationElement((WaterSurfaceElevationElement)element);
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


        

        public ChildElement CreateElementFromRowData(object[] rowData)
        {

            List<PathAndProbability> ppList = new List<PathAndProbability>();


            WaterSurfaceElevationElement wse = new WaterSurfaceElevationElement((string)rowData[0], (string)rowData[1], ppList, (bool)rowData[2]);

            int lastRow = Storage.Connection.Instance.GetTable(wse.TableName).NumberOfRows - 1;
            foreach (object[] row in Storage.Connection.Instance.GetTable(wse.TableName).GetRows(0, lastRow))
            {
                wse.RelativePathAndProbability.Add(new PathAndProbability(row[0].ToString(), Convert.ToDouble(row[1])));
            }
            return wse;
            //AddElement(wse, false);
        }
    }
}
