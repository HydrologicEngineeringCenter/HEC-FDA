using FdaViewModel.Utilities;
using FdaViewModel.Watershed;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class TerrainElementPersistenceManager : SavingBase, IPersistable
    {


        private const string TableName = "Terrains";
        internal override string ChangeTableConstant { get { return "?????"; } }
        private static readonly string[] TableColumnNames = { "Terrain Name", "Path Name" };
        private static readonly Type[] TableColumnTypes = { typeof(string), typeof(string) };


      


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
            StudyCacheForSaving.AddTerrainElement(element);
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
                Navigate(messageBox);
                element.CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/Terrain.png");
                return;
            }
            StudyCacheForSaving.RemoveTerrainElement((TerrainElement)element);

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
                        Navigate(messageBox);
                    }
                }
               
            }
        }

        #endregion


        public List<Utilities.ChildElement> Load()
        {
            return CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
        }

        public void SaveNew(Utilities.ChildElement element)
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

    }
}
