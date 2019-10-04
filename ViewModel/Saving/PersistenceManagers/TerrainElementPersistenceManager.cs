using FdaViewModel.Utilities;
using FdaViewModel.Watershed;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class TerrainElementPersistenceManager : SavingBase, IElementManager
    {
        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "Terrain";
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("TerrainElementPersistenceManager");


        private const string TABLE_NAME = "Terrains";
        internal override string ChangeTableConstant { get { return "?????"; } }
        private static readonly string[] TableColNames = { "Terrain Name", "Path Name" };
        private static readonly Type[] TableColTypes = { typeof(string), typeof(string) };

        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }



        public TerrainElementPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        //public string GetTerrainPathBase()
        //{
        //    return Storage.Connection.Instance.TerrainDirectory + "\\";

        //}
        public string OriginalTerrainPath { get; set; }

        public override string TableName
        {
            get { return TABLE_NAME; }
        }

        public override string[] TableColumnNames => throw new NotImplementedException();

        private object[] GetRowDataFromElement(TerrainElement element)
        {
              return new object[] { element.Name, element.FileName };

        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            return new TerrainElement((string)rowData[0], (string)rowData[1]);
        }

        private async void CopyFileOnBackgroundThread(TerrainElement element) //object sender, DoWorkEventArgs e)
        {
            await Task.Run(() => System.IO.File.Copy(OriginalTerrainPath, element.FileName)); //pathNames[0], pathNames[1]));
            string name = element.Name;
            //remove the temporary node and replace it
            TerrainOwnerElement terrainParent = StudyCache.GetParentElementOfType<TerrainOwnerElement>();

            for (int i = 0;i< terrainParent.Elements.Count;i++)
            {
                if (terrainParent.Elements[i].Name.Equals(name))
                {
                    terrainParent.Elements.Remove(terrainParent.Elements[i]);
                    break;
                }
            }
            StudyCacheForSaving.AddElement(element);
        }
        private async void RemoveTerrainFileOnBackgroundThread(TerrainElement element)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (System.IO.File.Exists(element.FileName))//System.IO.File.Exists(Storage.Connection.Instance.TerrainDirectory + "\\" + element.Name))
                    {
                        System.IO.File.Delete(element.FileName);//Storage.Connection.Instance.TerrainDirectory + "\\" + element.Name);
                    }
                });
            }
            catch (Exception e)
            {
                CustomMessageBoxVM messageBox = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "Could not delete terrain file: " + element.FileName);
                string header = "Error";
                DynamicTabVM tab = new DynamicTabVM(header, messageBox, "MessageBoxError");
                Navigate(tab);
                element.CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/Terrain.png");
                return;
            }
            StudyCacheForSaving.RemoveElement((TerrainElement)element);

        }

        private async void RenameTheTerrainFileOnBackgroundThread(ChildElement oldElement, ChildElement newElement)
        {
            if (!newElement.Name.Equals(oldElement.Name))
            {
                string oldFilePath = Storage.Connection.Instance.GetTerrainFile(oldElement.Name);
                if(oldFilePath != null)
                {
                    // at least one matching file exists
                    List<NamedAction> actions = new List<NamedAction>();
                    foreach(NamedAction act in newElement.Actions)
                    {
                        actions.Add(act);
                    }
                    newElement.Actions.Clear();
                    newElement.CustomTreeViewHeader = new CustomHeaderVM(newElement.Name, "pack://application:,,,/Fda;component/Resources/Terrain.png",  " -Renaming File", true);
                    try
                    {
                        await Task.Run(() =>
                        {
                            FileInfo currentFile = new FileInfo(oldFilePath);
                            currentFile.MoveTo(currentFile.Directory.FullName + "\\" + newElement.Name + currentFile.Extension);
                            newElement.CustomTreeViewHeader = new CustomHeaderVM(newElement.Name, "pack://application:,,,/Fda;component/Resources/Terrain.png");
                            newElement.Actions = actions;
                           // newElement.AddToMapWindow
                        });
                        //System.IO.File.Move(files[0], Storage.Connection.Instance.TerrainDirectory + "\\" + newName);

                    }
                    catch (Exception e)
                    {
                        CustomMessageBoxVM messageBox = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "Could not rename the terrain file at location: " + Storage.Connection.Instance.TerrainDirectory + "\\" + oldElement.Name);
                        string header = "Error";
                        DynamicTabVM tab = new DynamicTabVM(header, messageBox, "MessageBoxError");
                        Navigate(tab);
                    }
                }
               
            }
        }

        #endregion


        public void Load()
        {
            List<ChildElement> terrains = CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (TerrainElement elem in terrains)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public void SaveNew(ChildElement element)
        {
            SaveNewElementToParentTable(GetRowDataFromElement((TerrainElement)element), TableName, TableColumnNames, TableColumnTypes);
            CopyFileOnBackgroundThread((TerrainElement)element);
        }
        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            element.CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/Terrain.png", element.Name + " -Deleting", true);
            element.Actions.Clear();
            RemoveTerrainFileOnBackgroundThread((TerrainElement)element);
            //System.IO.File.Delete(((TerrainElement)element).FileName);

        }
        public void SaveExisting(ChildElement oldElement, ChildElement element, int changeTableIndex)
        {
            RenameTheTerrainFileOnBackgroundThread(oldElement, element);
            UpdateParentTableRow(element.Name, changeTableIndex, GetRowDataFromElement((TerrainElement)element), oldElement.Name, TableName, false, ChangeTableConstant);
            StudyCacheForSaving.UpdateTerrain((TerrainElement)oldElement, (TerrainElement)element);
            oldElement.AddMapTreeViewItemBackIn(((TerrainElement)oldElement).NodeToAddBackToMapWindow, new EventArgs());
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
