using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class TerrainElementPersistenceManager : SavingBase<TerrainElement>
    {

        public TerrainElementPersistenceManager(Study.FDACache studyCache, string tableName):base(studyCache, tableName)
        {
        }

        private async void CopyFileOnBackgroundThread(string OriginalTerrainPath, TerrainElement element)
        {
            string newPath = Connection.Instance.TerrainDirectory + "\\" + element.Name + "\\" + element.FileName;
            string newDirectory = Path.GetDirectoryName(newPath);
            Directory.CreateDirectory(newDirectory);
            
            bool isVRT = Path.GetExtension(element.FileName).Equals(".vrt");
            bool isHDF = Path.GetExtension(element.FileName).Equals(".hdf");

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
            else if(isHDF)
            {
                //copy all files at this level
                string originalDirName = Path.GetDirectoryName(OriginalTerrainPath);

                string[] paths = Directory.GetFiles(originalDirName);
                foreach (string path in paths)
                {
                    await Task.Run(() => File.Copy(path, newDirectory + "\\" + Path.GetFileName(path)));
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
                MessageBox.Show("Could not delete terrain file: " + element.FileName + ":\n" + e.Message, "Error Deleting Terrain", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        Tooltip = StringConstants.CreateLastEditTooltip(newElement.LastEditDate),
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
                                Tooltip = StringConstants.CreateLastEditTooltip(newElement.LastEditDate),
                            };

                            newElement.Actions = actions;
                        });
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Could not rename the terrain file at location: " + Connection.Instance.TerrainDirectory + "\\" + oldElement.Name + ":\n" + e.Message, "Error Renaming Terrain", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
               
            }
        }



        public void SaveNew(string OriginalTerrainPath, ChildElement element)
        {
            SaveNewElementToTable(GetRowDataFromElement((TerrainElement)element), TableColumnNames, TableColumnTypes);
            CopyFileOnBackgroundThread(OriginalTerrainPath,(TerrainElement)element);
        }
        public override void Remove(ChildElement element)
        {
            RemoveElementFromTable(element);
            element.CustomTreeViewHeader = new CustomHeaderVM(element.Name)
            {
                ImageSource = ImageSources.TERRAIN_IMAGE,
                Tooltip = StringConstants.CreateLastEditTooltip(element.LastEditDate),
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
            string destinationFilePath = Connection.Instance.TerrainDirectory + "\\" + element.Name + originalExtension;
            ((TerrainElement)element).FileName = destinationFilePath;
            base.SaveExisting( element);
        }
    }
}
