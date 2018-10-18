using FdaViewModel.Watershed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class TerrainElementPersistenceManager : SavingBase, IPersistable
    {
        public  string TableName
        {
            get { return "Terrains"; }
        }
        public  string[] TableColumnNames()
        {
            return new string[] { "Terrain Name", "Path Name" };
        }

        public  Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string) };
        }


        public TerrainElementPersistenceManager(Study.FDACache studyCache)
        {
            StudyCache = studyCache;
        }


        public List<Utilities.ChildElement> Load()
        {
            return CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
        }

        public void SaveNew(Utilities.ChildElement element)
        {

        }

        public void SaveExisting(Utilities.ChildElement element, string oldName, Statistics.UncertainCurveDataCollection oldCurve)
        {

        }


        public TerrainElement CreateElementFromRowData(object[] rowData)
        {
            return new TerrainElement((string)rowData[0], (string)rowData[1]);
        }

    }
}
