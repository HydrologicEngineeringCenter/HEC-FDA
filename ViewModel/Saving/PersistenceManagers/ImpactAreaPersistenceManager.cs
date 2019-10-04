using FdaViewModel.ImpactArea;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class ImpactAreaPersistenceManager : SavingBase, IElementManager
    {
        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "Impact_Area";
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("ImpactAreaPersistenceManager");

        private const string TABLE_NAME = "Impact Areas";
        internal override string ChangeTableConstant { get { return "?????"; } }

        public override string TableName
        {
            get { return TABLE_NAME; }
        }

        public override string[] TableColumnNames => throw new NotImplementedException();

        private static readonly string[] TableColNames = { "Impact Area Set Name", "Description" };
        private static readonly Type[] TableColTypes = { typeof(string), typeof(string) };
        private static string IndexPointTableNameConstant = "IndexPointTable -";
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }



        public ImpactAreaPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(ImpactAreaElement element)
        {
                return new object[] { element.Name, element.Description };
        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            ObservableCollection<ImpactAreaRowItem> dummyCollection = new ObservableCollection<ImpactAreaRowItem>();

            //i need to read from the sqlite table and get the polygon features and pass it to the new element
            //DataBase_Reader.SqLiteReader sqr = new DataBase_Reader.SqLiteReader(Storage.Connection.Instance.ProjectFile);
            //LifeSimGIS.GeoPackageReader gpr = new LifeSimGIS.GeoPackageReader(sqr);
            //LifeSimGIS.PolygonFeatures polyFeatures = (LifeSimGIS.PolygonFeatures)gpr.ConvertToGisFeatures("Impact Areas - " + rowData[0]);

            ImpactAreaElement iae = new ImpactAreaElement((string)rowData[0], (string)rowData[1], dummyCollection);

           // int lastRow = Storage.Connection.Instance.GetTable(TableName).NumberOfRows - 1;
            ObservableCollection<object> tempCollection = new ObservableCollection<object>();
            DatabaseManager.DataTableView indexTable = Storage.Connection.Instance.GetTable(IndexPointTableNameConstant + rowData[0]);
            foreach (object[] row in indexTable.GetRows(0, indexTable.NumberOfRows-1))
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

        private void SaveImpactAreaTable(ImpactAreaElement element)
        {
            LifeSimGIS.ShapefileReader shp = new LifeSimGIS.ShapefileReader(element.SelectedPath);
            LifeSimGIS.PolygonFeatures polyFeatures = (LifeSimGIS.PolygonFeatures)shp.ToFeatures();
            WriteImpactAreaTableToSqlite(element,polyFeatures);
        }
        private void WriteImpactAreaTableToSqlite(ImpactAreaElement element, LifeSimGIS.PolygonFeatures polyFeatures)
        {
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                if (Storage.Connection.Instance.TableNames().Contains(IndexPointTableNameConstant + element.Name))
                {
                    //already exists... delete?
                    Storage.Connection.Instance.DeleteTable(IndexPointTableNameConstant + element.Name);
                }
                LifeSimGIS.GeoPackageWriter gpw = new LifeSimGIS.GeoPackageWriter(Storage.Connection.Instance.Reader);

                System.Data.DataTable dt = new System.Data.DataTable(IndexPointTableNameConstant + element.Name);
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("IndexPoint", typeof(double));

                foreach (ImpactAreaRowItem row in element.ImpactAreaRows)
                {
                    dt.Rows.Add(row.Name, row.IndexPoint);
                }

                DatabaseManager.InMemoryReader imr = new DatabaseManager.InMemoryReader(dt);
                gpw.AddFeatures(IndexPointTableNameConstant + element.Name, polyFeatures, imr.GetTableManager(imr.TableNames[0]));
            }

        }
        private void UpdateExistingTable(ImpactAreaElement element)
        {

            DatabaseManager.DataTableView dtv = Storage.Connection.Instance.GetTable(IndexPointTableNameConstant + element.Name);

            object[] nameArray = new object[element.ImpactAreaRows.Count];
            object[] indexPointArray = new object[element.ImpactAreaRows.Count];
            int i = 0;
            foreach (ImpactAreaRowItem row in element.ImpactAreaRows)
            {
                nameArray[i] = row.Name;
                indexPointArray[i] = row.IndexPoint;
                i++;
            }

            dtv.EditColumn(2, nameArray);
            dtv.EditColumn(3, indexPointArray);

            dtv.ApplyEdits();
        }


        //public void ImpactAreasToMapWindow(ImpactAreaElement element)
        //{
        //    DataBase_Reader.SqLiteReader sqr = new DataBase_Reader.SqLiteReader(Storage.Connection.Instance.ProjectFile);
        //    LifeSimGIS.GeoPackageReader gpr = new LifeSimGIS.GeoPackageReader(sqr);
        //    LifeSimGIS.PolygonFeatures polyFeatures = (LifeSimGIS.PolygonFeatures)gpr.ConvertToGisFeatures(IndexPointTableNameConstant + element.Name);
        //    LifeSimGIS.VectorFeatures features = polyFeatures;
        //    //read from table.
        //    DataBase_Reader.DataTableView dtv = sqr.GetTableManager(IndexPointTableNameConstant + element.Name);
        //    int[] geometryColumns = { 0, 1 };
        //    dtv.DeleteColumns(geometryColumns);

        //    OpenGLMapping.OpenGLDrawInfo ogldi = new OpenGLMapping.OpenGLDrawInfo(true, new OpenTK.Graphics.Color4((byte)255, 0, 0, 255), 1, true, new OpenTK.Graphics.Color4((byte)0, 255, 0, 200));
        //    Utilities.AddShapefileEventArgs args = new Utilities.AddShapefileEventArgs(Name, features, dtv, ogldi);
        //    element.AddToMapWindow(this, args);
        //    //_featureNodeHash = args.MapFeatureHash;
           

        //}

        #endregion

        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(ImpactAreaElement))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;

                SaveNewElementToParentTable(GetRowDataFromElement((ImpactAreaElement)element), TableName, TableColumnNames, TableColumnTypes);
                //SaveElementToChangeTable(element.Name, GetRowDataFromElement((ImpactAreaElement)element), ChangeTableConstant, TableColumnNames, TableColumnTypes);
                SaveImpactAreaTable((ImpactAreaElement)element);
                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCacheForSaving.AddElement((ImpactAreaElement)element);
            }
        }


        public void Remove(ChildElement element)
        {
            //RemoveFromParentTable(element, TableName);
            //DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            //StudyCacheForSaving.RemoveElement((ImpactAreaElement)element);

        }
        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex  )
        {
            string editDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = editDate;

            //need to add, did Index point values change
            //if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((ImpactAreaElement)elementToSave), oldName, TableName))
            {
                UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((ImpactAreaElement)elementToSave), oldElement.Name, TableName, false, ChangeTableConstant);
                if(!oldElement.Name.Equals(elementToSave.Name))
                {
                    Storage.Connection.Instance.RenameTable(IndexPointTableNameConstant + oldElement.Name, IndexPointTableNameConstant + elementToSave.Name);
                }
                UpdateExistingTable((ImpactAreaElement)elementToSave);
                // update the existing element. This will actually remove the old element and do an insert at that location with the new element.
                StudyCacheForSaving.UpdateImpactAreaElement((ImpactAreaElement)oldElement, (ImpactAreaElement)elementToSave);
            }
        }

        public void Load()
        {
            List<ChildElement> impAreas = CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (ImpactAreaElement elem in impAreas)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public ObservableCollection<FdaLogging.LogItem> GetLogMessages(ChildElement element)
        {
            return new ObservableCollection<FdaLogging.LogItem>();
        }

        /// <summary>
        /// This will put a log into the log tables. Logs are only unique by element id and
        /// element type. ie. Rating Curve id=3.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="elementName"></param>
        public void Log(FdaLogging.LoggingLevel level, string message, string elementName)
        {
            int elementId = GetElementId(TableName, elementName);
            LOGGER.Log(level, message, ELEMENT_TYPE, elementId);
        }

        /// <summary>
        /// This will look in the parent table for the element id using the element name. 
        /// Then it will sweep through the log tables pulling out any logs with that id
        /// and element type. 
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public ObservableCollection<FdaLogging.LogItem> GetLogMessages(string elementName)
        {
            int id = GetElementId(TableName, elementName);
            return FdaLogging.RetrieveFromDB.GetLogMessages(id, ELEMENT_TYPE);
        }

        /// <summary>
        /// Gets all the log messages for this element from the specified log level table.
        /// This is used by the MessageExpander to filter by log level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public ObservableCollection<FdaLogging.LogItem> GetLogMessagesByLevel(FdaLogging.LoggingLevel level, string elementName)
        {
            int id = GetElementId(TableName, elementName);
            return FdaLogging.RetrieveFromDB.GetLogMessagesByLevel(level, id, ELEMENT_TYPE);
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            throw new NotImplementedException();
        }
    }
}
