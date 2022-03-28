using System.Collections.Generic;
using System.IO;

namespace HEC.FDA.ViewModel.Watershed
{
    //[Author("q0heccdm", "10 / 11 / 2016 11:13:25 AM")]
    public class TerrainBrowserVM:Editors.BaseEditorVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/11/2016 11:13:25 AM
        #endregion
        #region Fields
        private string _TerrainPath;
        private string _OriginalPath;
        private List<string> _AvailablePaths;
        #endregion
        #region Properties
     
        /// <summary>
        /// This is the original path that the user selected.
        /// </summary>
        //public string OriginalPath
        //{
        //    get { return _OriginalPath; }
        //    set { _OriginalPath = value; CreateNewPathName(); NotifyPropertyChanged(); }
        //}

        //public List<string> AvailablePaths
        //{
        //    get { return _AvailablePaths; }
        //    set { _AvailablePaths = value; NotifyPropertyChanged(); }
        //}

        /// <summary>
        /// This is the new location for the terrain file. We are copying the original file and placing it within the study directory.
        /// </summary>
        public string TerrainPath
        {
            get { return _TerrainPath; }
            set
            {
                _TerrainPath = value;NotifyPropertyChanged();

            }
        }
        #endregion
        #region Constructors
        public TerrainBrowserVM(List<string> availablePaths, Editors.EditorActionManager actionManager) : base(actionManager)
        {
            //AvailablePaths = availablePaths;
        }       

        //public override void AddValidationRules()
        //{
        //    AddRule(nameof(Name), () => Name != null, "Terrain Name cannot be empty.");
        //    AddRule(nameof(Name), () => Name != "", "Terrain Name cannot be empty.");

        //    AddRule(nameof(OriginalPath), () => OriginalPath != null, "Path cannot be null.");
        //    AddRule(nameof(OriginalPath), () => OriginalPath != "", "Path cannot be null.");

        //    AddRule(nameof(TerrainPath), () =>
        //    { return System.IO.File.Exists(TerrainPath) != true; }, "A file with this name already exists.");
        //}

        public override void Save()
        {
            //todo: validate.

            if (TerrainPath != null && TerrainPath != "")
            {
                //todo: i need to change the path on the element

                int id = Saving.PersistenceFactory.GetTerrainManager().GetNextAvailableId();
                //add a dummy element to the parent
                string studyPath = CreateNewPathName();
                TerrainElement t = new TerrainElement(Name,studyPath, id, true); // file extention?
                StudyCache.GetParentElementOfType<TerrainOwnerElement>().AddElement(t);
                TerrainElement newElement = new TerrainElement(Name, studyPath, id);

                Saving.PersistenceManagers.TerrainElementPersistenceManager manager = Saving.PersistenceFactory.GetTerrainManager();
                //manager.OriginalTerrainPath = OriginalPath;
                manager.SaveNew(TerrainPath, newElement);
            }   
        }

        #endregion
        #region Voids
        public void FileSelected(string filePath)
        {
            TerrainPath = filePath;
        }

        private string CreateNewPathName()
        {
            //if (Name == null || Name == "") { return; }
            //if (OriginalPath == null || OriginalPath == "") { return; }
            string fileName = Path.GetFileName(TerrainPath);
            //string originalExtension = System.IO.Path.GetExtension(TerrainPath);
            return Storage.Connection.Instance.TerrainDirectory + "\\" + Name + "\\" + fileName;
        }
        #endregion
    }
}
