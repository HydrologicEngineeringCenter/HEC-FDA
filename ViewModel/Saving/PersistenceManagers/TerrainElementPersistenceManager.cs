using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class TerrainElementPersistenceManager : SavingBase
    {
        private const int NAME_COL = 1;
        private const int DESC_COL = 2;

        private const string TABLE_NAME = "terrains";
        internal override string ChangeTableConstant { get { return "?????"; } }
        private static readonly string[] TableColNames = { NAME, "path" };
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

        private object[] GetRowDataFromElement(TerrainElement element)
        {
              return new object[] { element.Name, element.FileName };

        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            int id = Convert.ToInt32(rowData[ID_COL]);
            return new TerrainElement((string)rowData[NAME_COL], (string)rowData[DESC_COL], id);
        }

        private async void CopyFileOnBackgroundThread(string OriginalTerrainPath, TerrainElement element)
        {
            string terrainPath = element.FileName;
            Directory.CreateDirectory(Path.GetDirectoryName(terrainPath));

            bool isVRT = Path.GetExtension(terrainPath).Equals(".vrt");

            if(isVRT)
            {
                //then copy all the vrt and tif files
                string newDirName = Path.GetDirectoryName(terrainPath);
                string originalDirName = Path.GetDirectoryName(OriginalTerrainPath);

                string[] paths = Directory.GetFiles(originalDirName);
                foreach(string path in paths)
                {
                    string extension = Path.GetExtension(path);
                    if(extension.Equals(".vrt") || extension.Equals(".tif"))
                    {
                        await Task.Run(() => File.Copy(path, newDirName + "\\"+ Path.GetFileName(path)));
                    }
                }
            }
            else
            {
                //.tifs and .flts i just copy the file.
                await Task.Run(() => File.Copy(OriginalTerrainPath, element.FileName)); 
            }

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
                    string directoryName = Path.GetDirectoryName(element.FileName);
                    if(Directory.Exists(directoryName))
                    {
                        Directory.Delete(directoryName,true);
                    }
                });
            }
            catch (Exception e)
            {
                CustomMessageBoxVM messageBox = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "Could not delete terrain file: " + element.FileName);
                string header = "Error";
                DynamicTabVM tab = new DynamicTabVM(header, messageBox, "MessageBoxError");
                Navigate(tab);
                element.CustomTreeViewHeader = new CustomHeaderVM(element.Name, "pack://application:,,,/View;component/Resources/Terrain.png");
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
                    newElement.CustomTreeViewHeader = new CustomHeaderVM(newElement.Name, "pack://application:,,,/View;component/Resources/Terrain.png",  " -Renaming File", true);
                    try
                    {
                        await Task.Run(() =>
                        {
                            FileInfo currentFile = new FileInfo(oldFilePath);
                            currentFile.MoveTo(currentFile.Directory.FullName + "\\" + newElement.Name + currentFile.Extension);
                            newElement.CustomTreeViewHeader = new CustomHeaderVM(newElement.Name, "pack://application:,,,/View;component/Resources/Terrain.png");
                            newElement.Actions = actions;
                        });
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

        public override void Load()
        {
            List<ChildElement> terrains = CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (TerrainElement elem in terrains)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public void SaveNew(string OriginalTerrainPath, ChildElement element)
        {
            SaveNewElementToParentTable(GetRowDataFromElement((TerrainElement)element), TableName, TableColumnNames, TableColumnTypes);
            CopyFileOnBackgroundThread(OriginalTerrainPath,(TerrainElement)element);
        }
        public override void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            element.CustomTreeViewHeader = new CustomHeaderVM(element.Name, "pack://application:,,,/View;component/Resources/Terrain.png", element.Name + " -Deleting", true);
            element.Actions.Clear();
            RemoveTerrainFileOnBackgroundThread((TerrainElement)element);
        }
        public void SaveExisting(ChildElement oldElement, ChildElement element, int changeTableIndex)
        {
            RenameTheTerrainFileOnBackgroundThread(oldElement, element);
            //the path needs to get updated with the new name and set on the new element.
            TerrainElement elem = (TerrainElement)oldElement;
            string originalExtension = System.IO.Path.GetExtension(elem.FileName);
            string destinationFilePath = Storage.Connection.Instance.TerrainDirectory + "\\" + element.Name + originalExtension;
            ((TerrainElement)element).FileName = destinationFilePath;
            base.SaveExisting( element);
            oldElement.AddMapTreeViewItemBackIn(((TerrainElement)oldElement).NodeToAddBackToMapWindow, new EventArgs());
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((TerrainElement)elem);
        }
    }
}
