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
        private static readonly string[] TableColumnNames = { "Name", "Description", "IsDepthGrids" };
        private static readonly Type[] TableColumnTypes = { typeof(string), typeof(string), typeof(bool) };
        private static string PathAndProbTableConstant = "WSE -";
        internal override string ChangeTableConstant
        {
            get { return "???"; }
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


            WaterSurfaceElevationElement wse = new WaterSurfaceElevationElement((string)rowData[0], (string)rowData[1], ppList, (bool)rowData[2]);

            //this isn't going to be correct. The table name here is not the parent table but the change table name.
            DatabaseManager.DataTableView tableView = Storage.Connection.Instance.GetTable(PathAndProbTableConstant + rowData[0]);
            foreach (object[] row in tableView.GetRows(0, tableView.NumberOfRows-1))
            {
                wse.RelativePathAndProbability.Add(new PathAndProbability(row[0].ToString(), Convert.ToDouble(row[1])));
            }
            return wse;
            //AddElement(wse, false);
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
                System.IO.Directory.Delete(Storage.Connection.Instance.HydraulicsDirectory + "\\" + element.Name,true);
                //foreach (PathAndProbability pap in element.RelativePathAndProbability)
                //{
                //    System.IO.File.Delete(Storage.Connection.Instance.HydraulicsDirectory + "\\" + pap.Path);
                //}

            }
            catch (Exception e)
            {
                throw new NotImplementedException();
                //CustomMessageBoxVM messageBox = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "Could not delete water surface elevation files");
                //Navigate(messageBox);
                //CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/WaterSurfaceElevation.png");
                //return;
            }
            
        }

        //public void AddToMapWindow(WaterSurfaceElevationElement element)
        //{
        //    foreach (PathAndProbability file in element.RelativePathAndProbability)
        //    {
        //        LifeSimGIS.RasterFeatures r = new LifeSimGIS.RasterFeatures(Storage.Connection.Instance.HydraulicsDirectory + "\\" + file.Path);
        //        OpenGLMapping.ColorRamp c = new OpenGLMapping.ColorRamp(OpenGLMapping.ColorRamp.RampType.LightBlueDarkBlue, r.GridReader.Max, r.GridReader.Min, r.GridReader.Mean, r.GridReader.StdDev);
        //        Utilities.AddGriddedDataEventArgs args = new Utilities.AddGriddedDataEventArgs(r, c);
        //        args.FeatureName = Name;
        //        element.AddToMapWindow(this, args);
        //    }
        //}

        private void RenameHydraulicsDirectory(string oldName, string newName)
        {
            string oldPath = Storage.Connection.Instance.HydraulicsDirectory + "\\" + oldName;
            if(System.IO.Directory.Exists(oldPath))
            {
                string newPath = Storage.Connection.Instance.HydraulicsDirectory + "\\" + newName;

                try
                {
                    System.IO.Directory.Move(oldPath, newPath);
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
            if(System.IO.Directory.Exists(path))
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
            Storage.Connection.Instance.DeleteTable(PathAndProbTableConstant + element.Name);
            RemoveWaterSurfElevFiles((WaterSurfaceElevationElement)element);
            StudyCacheForSaving.RemoveElement((WaterSurfaceElevationElement)element);

        }

        public void SaveExisting(ChildElement oldElement, ChildElement element, int changeTableIndex )
        {
            UpdateParentTableRow(element.Name, changeTableIndex, GetRowDataFromElement((WaterSurfaceElevationElement)element), oldElement.Name, TableName, false, ChangeTableConstant);
            Storage.Connection.Instance.RenameTable(PathAndProbTableConstant + oldElement.Name, PathAndProbTableConstant + element.Name);
            SavePathAndProbabilitiesTable((WaterSurfaceElevationElement)element);
            //rename the folder in the study directory
            RenameHydraulicsDirectory(oldElement.Name, element.Name);
            StudyCacheForSaving.UpdateWaterSurfaceElevationElement((WaterSurfaceElevationElement)oldElement, (WaterSurfaceElevationElement)element);

        }

        public void Load()
        {
            List<Utilities.ChildElement> waterSurfaceElevs = CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (WaterSurfaceElevationElement elem in waterSurfaceElevs)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public override void AddValidationRules()
        {
           // throw new NotImplementedException();
        }
    }
}
