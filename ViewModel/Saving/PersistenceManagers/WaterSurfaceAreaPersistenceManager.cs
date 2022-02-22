using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.WaterSurfaceElevation;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class WaterSurfaceAreaPersistenceManager : SavingBase
    {
        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "Water_Surface_Area";
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("WaterSurfaceAreaPersistenceManager");

        private const string TABLE_NAME = "water_surface_elevations";
        private static readonly string[] TableColNames = { NAME, DESCRIPTION, "is_depth_grids" };
        private static readonly Type[] TableColTypes = { typeof(string), typeof(string), typeof(bool) };
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }
        private static string PathAndProbTableConstant = "WSE -";
        internal override string ChangeTableConstant
        {
            get { return "???"; }
        }

        public override string TableName
        {
            get { return TABLE_NAME; }
        }

        public override string[] TableColumnNames
        {
            get
            {
                return TableColNames;
            }
        }

        public WaterSurfaceAreaPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(WaterSurfaceElevationElement element)
        {
            return new object[] { element.Name, element.Description, element.IsDepthGrids };
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            List<PathAndProbability> ppList = new List<PathAndProbability>();

            DatabaseManager.DataTableView tableView = Storage.Connection.Instance.GetTable(PathAndProbTableConstant + rowData[1]);
            foreach (object[] row in tableView.GetRows(0, tableView.NumberOfRows-1))
            {
                ppList.Add(new PathAndProbability(row[0].ToString(), Convert.ToDouble(row[1])));
            }
            int id = Convert.ToInt32(rowData[ID_COL]);
            WaterSurfaceElevationElement wse = new WaterSurfaceElevationElement((string)rowData[1], (string)rowData[2], ppList, Convert.ToBoolean(rowData[3]), id);
            if(ppList.Count>0 && ppList[0].Path.Equals("NA"))
            {
                wse.HasAssociatedFiles = false;
            }
            return wse;
        }

        private void SavePathAndProbabilitiesTable(WaterSurfaceElevationElement element)
        {
            //gets called if savestotable is true
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                if (Storage.Connection.Instance.TableNames().Contains(PathAndProbTableConstant + element.Name))
                {
                    //already exists... delete?
                    Storage.Connection.Instance.DeleteTable(PathAndProbTableConstant + element.Name);
                }

                string[] colNames = new string[] { "Name", "Probability", "LastEdited" };
                Type[] colTypes = new Type[] { typeof(string), typeof(string), typeof(string) };

                Storage.Connection.Instance.CreateTable(PathAndProbTableConstant + element.Name, colNames, colTypes);
                DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(PathAndProbTableConstant + element.Name);

                object[][] rows = new object[element.RelativePathAndProbability.Count][];
                int i = 0;
                foreach (PathAndProbability p in element.RelativePathAndProbability)
                {
                    rows[i] = new object[] { p.Path, p.Probability, DateTime.Now.ToString() };
                    i++;
                }
                for (int j = 0; j < rows.Count(); j++)
                {
                    tbl.AddRow(rows[j]);
                }
                tbl.ApplyEdits();
            }
        }

        private void RemoveWaterSurfElevFiles(WaterSurfaceElevationElement element)
        {
            try
            {
                Directory.Delete(Storage.Connection.Instance.HydraulicsDirectory + "\\" + element.Name,true);
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }   
        }

        private void RenameHydraulicsDirectory(string oldName, string newName)
        {
            string oldPath = Storage.Connection.Instance.HydraulicsDirectory + "\\" + oldName;
            if(Directory.Exists(oldPath))
            {
                string newPath = Storage.Connection.Instance.HydraulicsDirectory + "\\" + newName;

                try
                {
                    Directory.Move(oldPath, newPath);
                }
                catch (Exception e)
                {
                    throw new Exception();
                }
            }
            else
            {
                throw new Exception();
            }
        }

        private void SaveFilesToStudyDirectory(string directoryName)
        {
            string path = Storage.Connection.Instance.HydraulicsDirectory + "\\" + directoryName;
            if(Directory.Exists(path))
            {
                throw new Exception();
            }

        }

        #endregion

        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(WaterSurfaceElevationElement))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;

                SaveNewElementToParentTable(GetRowDataFromElement((WaterSurfaceElevationElement)element), TableName, TableColumnNames, TableColumnTypes);
                SavePathAndProbabilitiesTable((WaterSurfaceElevationElement)element);
                //save files to the study directory
                StudyCacheForSaving.AddElement((WaterSurfaceElevationElement)element);
            }
        }

        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            RemoveTable(PathAndProbTableConstant + element.Name); 
            //if the wse was imported from old fda, then it won't have associated files.
            WaterSurfaceElevationElement elem = (WaterSurfaceElevationElement)element;
            if (elem.HasAssociatedFiles)
            {
                RemoveWaterSurfElevFiles((WaterSurfaceElevationElement)element);
            }
            StudyCacheForSaving.RemoveElement((WaterSurfaceElevationElement)element);

        }

        public void SaveExisting( ChildElement element )
        {
            //base.SaveExisting( element);
            //UpdateThePaths((WaterSurfaceElevationElement)element);
            //Storage.Connection.Instance.RenameTable(PathAndProbTableConstant + oldElement.Name, PathAndProbTableConstant + element.Name);
            //SavePathAndProbabilitiesTable((WaterSurfaceElevationElement)element);
            ////rename the folder in the study directory
            //RenameHydraulicsDirectory(oldElement.Name, element.Name);
        }

        private void UpdateThePaths(WaterSurfaceElevationElement element)
        {
            foreach(PathAndProbability pp in element.RelativePathAndProbability)
            {
                string fileName = Path.GetFileName(pp.Path);
                pp.Path = element.Name + "\\" + fileName;
            }
        }

        public override void Load()
        {
            List<ChildElement> waterSurfaceElevs = CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (WaterSurfaceElevationElement elem in waterSurfaceElevs)
            {
                StudyCacheForSaving.AddElement(elem);
            }
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
        public override void Log(FdaLogging.LoggingLevel level, string message, string elementName)
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
        public override ObservableCollection<FdaLogging.LogItem> GetLogMessages(string elementName)
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
        public override ObservableCollection<FdaLogging.LogItem> GetLogMessagesByLevel(FdaLogging.LoggingLevel level, string elementName)
        {
            int id = GetElementId(TableName, elementName);
            return FdaLogging.RetrieveFromDB.GetLogMessagesByLevel(level, id, ELEMENT_TYPE);
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((WaterSurfaceElevationElement)elem);
        }
    }
}
