using FdaViewModel.ImpactArea;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class ImpactAreaPersistenceManager : SavingBase, IPersistable
    {

        public string TableName { get { return "Impact Areas"; } }
        public  string[] TableColumnNames()
        {
            return new string[] { "Impact Area Set Name", "Description" };
        }

        public  Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string) };
        }


        public ImpactAreaPersistenceManager(Study.FDACache studyCache)
        {
            StudyCache = studyCache;
        }


        public List<Utilities.ChildElement> Load()
        {
           return CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
        }

        public void SaveExisting(Utilities.ChildElement element, string oldName, Statistics.UncertainCurveDataCollection oldCurve)
        {

        }

        public void SaveNew(Utilities.ChildElement element)
        {
            //SaveNewElement()
        }




        public ImpactAreaElement CreateElementFromRowData(object[] rowData)
        {
            ObservableCollection<ImpactAreaRowItem> dummyCollection = new ObservableCollection<ImpactAreaRowItem>();

            //i need to read from the sqlite table and get the polygon features and pass it to the new element
            //DataBase_Reader.SqLiteReader sqr = new DataBase_Reader.SqLiteReader(Storage.Connection.Instance.ProjectFile);
            //LifeSimGIS.GeoPackageReader gpr = new LifeSimGIS.GeoPackageReader(sqr);
            //LifeSimGIS.PolygonFeatures polyFeatures = (LifeSimGIS.PolygonFeatures)gpr.ConvertToGisFeatures("Impact Areas - " + rowData[0]);

            ImpactAreaElement iae = new ImpactAreaElement((string)rowData[0], (string)rowData[1], dummyCollection);

            int lastRow = Storage.Connection.Instance.GetTable(iae.TableName).NumberOfRows - 1;
            ObservableCollection<object> tempCollection = new ObservableCollection<object>();
            foreach (object[] row in Storage.Connection.Instance.GetTable(iae.TableName).GetRows(0, lastRow))
            {
                //each row here should be a name and an index point
                ImpactAreaRowItem ri = new ImpactAreaRowItem(row[2].ToString(), Convert.ToDouble(row[3]), tempCollection);
                tempCollection.Add(ri);
            }
            ObservableCollection<ImpactAreaRowItem> items = new ObservableCollection<ImpactAreaRowItem>();
            foreach (object row in tempCollection)
            {
                items.Add((ImpactAreaRowItem)row);
            }
            iae.ImpactAreaRows = items;
            return iae;
        }

    }
}
