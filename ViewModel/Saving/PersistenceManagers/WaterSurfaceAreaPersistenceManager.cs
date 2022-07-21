using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Hydraulics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class WaterSurfaceAreaPersistenceManager : SavingBase
    {
        private const string TABLE_NAME = "hydraulics";
        private static readonly string[] TableColNames = { NAME, DESCRIPTION, "is_depth_grids" };
        private static readonly Type[] TableColTypes = { typeof(string), typeof(string), typeof(bool) };
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }
        private const string PATH_AND_PROB_TABLE = "hydraulic_data -";

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

            DatabaseManager.DataTableView tableView = Connection.Instance.GetTable(PATH_AND_PROB_TABLE + rowData[1]);
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
            if (!Connection.Instance.IsConnectionNull)
            {
                if (Connection.Instance.TableNames().Contains(PATH_AND_PROB_TABLE + element.Name))
                {
                    //already exists... delete?
                    Connection.Instance.DeleteTable(PATH_AND_PROB_TABLE + element.Name);
                }

                string[] colNames = new string[] { "Name", "Probability", "LastEdited" };
                Type[] colTypes = new Type[] { typeof(string), typeof(string), typeof(string) };

                Connection.Instance.CreateTable(PATH_AND_PROB_TABLE + element.Name, colNames, colTypes);
                DatabaseManager.DataTableView tbl = Connection.Instance.GetTable(PATH_AND_PROB_TABLE + element.Name);

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
                Directory.Delete(Connection.Instance.HydraulicsDirectory + "\\" + element.Name,true);
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }   
        }

        #endregion

        public override void SaveNew(ChildElement element)
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

        public override void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            RemoveTable(PATH_AND_PROB_TABLE + element.Name);
            //if the wse was imported from old fda, then it won't have associated files.
            WaterSurfaceElevationElement elem = (WaterSurfaceElevationElement)element;
            if (elem.HasAssociatedFiles)
            {
                RemoveWaterSurfElevFiles((WaterSurfaceElevationElement)element);
            }
            StudyCacheForSaving.RemoveElement((WaterSurfaceElevationElement)element);
        }

        public void SaveExisting( ChildElement element, string oldName )
        {
            base.SaveExisting( element);
            //delete the old table and create a new one
            Connection.Instance.DeleteTable(PATH_AND_PROB_TABLE + oldName);
            SavePathAndProbabilitiesTable((WaterSurfaceElevationElement)element);        
        }

        public void RenamePathAndProbabilitesTableName(string oldName, string newName)
        {
            Connection.Instance.RenameTable(PATH_AND_PROB_TABLE + oldName, PATH_AND_PROB_TABLE + newName);
        }

        public override void Load()
        {
            List<ChildElement> waterSurfaceElevs = CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (WaterSurfaceElevationElement elem in waterSurfaceElevs)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((WaterSurfaceElevationElement)elem);
        }
    }
}
