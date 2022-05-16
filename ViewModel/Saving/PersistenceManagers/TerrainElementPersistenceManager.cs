using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class TerrainElementPersistenceManager : SavingBase
    {
        private const int NAME_COL = 1;
        private const int DESC_COL = 2;

        private const string TABLE_NAME = "terrains";
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
            string newPath = Connection.Instance.TerrainDirectory + "\\" + element.Name + "\\" + element.FileName;
            string newDirectory = Path.GetDirectoryName(newPath);
            DirectoryInfo directoryInfo = Directory.CreateDirectory(newDirectory);
            
            bool isVRT = Path.GetExtension(element.FileName).Equals(".vrt");

            if(isVRT)
            {
                //then copy all the vrt and tif files
                string originalDirName = Path.GetDirectoryName(OriginalTerrainPath);

                string[] paths = Directory.GetFiles(originalDirName);
                foreach(string path in paths)
                {
                    string extension = Path.GetExtension(path);
                    if(extension.Equals(".vrt") || extension.Equals(".tif"))
                    {
                        await Task.Run(() => File.Copy(path, newDirectory + "\\"+ Path.GetFileName(path)));
                    }
                }
            }
            else
            {
                //.tifs and .flts i just copy the file.
                await Task.Run(() => File.Copy(OriginalTerrainPath, newPath)); 
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
                    string directoryName = Connection.Instance.TerrainDirectory + "\\" + element.Name;
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
                element.CustomTreeViewHeader = new CustomHeaderVM(element.Name, ImageSources.TERRAIN_IMAGE);
                return;
            }
            StudyCacheForSaving.RemoveElement((TerrainElement)element);
        }

        private async void RenameTheTerrainFileOnBackgroundThread(ChildElement oldElement, ChildElement newElement)
        {
            if (!newElement.Name.Equals(oldElement.Name))
            {
                string oldFilePath = Connection.Instance.GetTerrainFile(oldElement.Name);
                if(oldFilePath != null)
                {
                    // at least one matching file exists
                    List<NamedAction> actions = new List<NamedAction>();
                    foreach(NamedAction act in newElement.Actions)
                    {
                        actions.Add(act);
                    }
                    newElement.Actions.Clear();
                    newElement.CustomTreeViewHeader = new CustomHeaderVM(newElement.Name)
                    {
                        ImageSource = ImageSources.TERRAIN_IMAGE,
                        Tooltip = StringConstants.CreateChildNodeTooltip(newElement.LastEditDate),
                        Decoration = " -Renaming File",
                        GifVisible = true
                    };

                    try
                    {
                        await Task.Run(() =>
                        {
                            FileInfo currentFile = new FileInfo(oldFilePath);
                            currentFile.MoveTo(currentFile.Directory.FullName + "\\" + newElement.Name + currentFile.Extension);
                            
                            newElement.CustomTreeViewHeader = new CustomHeaderVM(newElement.Name)
                            {
                                ImageSource = ImageSources.TERRAIN_IMAGE,
                                Tooltip = StringConstants.CreateChildNodeTooltip(newElement.LastEditDate),
                            };

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
            element.CustomTreeViewHeader = new CustomHeaderVM(element.Name)
            {
                ImageSource = ImageSources.TERRAIN_IMAGE,
                Tooltip = StringConstants.CreateChildNodeTooltip(element.LastEditDate),
                Decoration = " -Deleting",
                GifVisible = true
            };

            element.Actions.Clear();
            RemoveTerrainFileOnBackgroundThread((TerrainElement)element);
        }
        public void SaveExisting(ChildElement oldElement, ChildElement element, int changeTableIndex)
        {
            RenameTheTerrainFileOnBackgroundThread(oldElement, element);
            //the path needs to get updated with the new name and set on the new element.
            TerrainElement elem = (TerrainElement)oldElement;
            string originalExtension = Path.GetExtension(elem.FileName);
            string destinationFilePath = Storage.Connection.Instance.TerrainDirectory + "\\" + element.Name + originalExtension;
            ((TerrainElement)element).FileName = destinationFilePath;
            base.SaveExisting( element);
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((TerrainElement)elem);
        }
    }
}
