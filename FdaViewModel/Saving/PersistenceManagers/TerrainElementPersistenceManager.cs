using FdaViewModel.Utilities;
using FdaViewModel.Watershed;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            foreach (Utilities.ChildElement elem in StudyCache.GetChildElementsOfType<TerrainElement>())
            {
                if (elem.Name.Equals(name))
                {
                    StudyCache.GetParentElementOfType<TerrainOwnerElement>().Elements.Remove(elem);
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
                    if (System.IO.File.Exists(element.FileName))
                    {
                        System.IO.File.Delete(element.FileName);
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
            UpdateParentTableRow(element.Name, changeTableIndex, GetRowDataFromElement((TerrainElement)element), oldElement.Name, TableName, false, ChangeTableConstant);
            StudyCacheForSaving.UpdateTerrain((TerrainElement)oldElement, (TerrainElement)element);
        }

    }
}
