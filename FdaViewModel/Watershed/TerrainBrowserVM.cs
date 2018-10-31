using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FdaViewModel.Watershed
{
    //[Author("q0heccdm", "10 / 11 / 2016 11:13:25 AM")]
    public class TerrainBrowserVM:Editors.BaseEditorVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/11/2016 11:13:25 AM
        #endregion
        #region Fields
        //public event EventHandler TerrainFileFinishedCopying;

        private string _TerrainPath;
        private string _OriginalPath;

       // private BackgroundWorker bw = new BackgroundWorker();

        #endregion
        #region Properties

      
        /// <summary>
        /// This is the original path that the user selected.
        /// </summary>
        public string OriginalPath
        {
            get { return _OriginalPath; }
            set { _OriginalPath = value; CreateNewPathName(); NotifyPropertyChanged(); }
        }

        

        //public string Name
        //{
        //    get { return _TerrainName; }
        //    set
        //    {
        //        _TerrainName = value; CreateNewPathName(); NotifyPropertyChanged();

        //    }
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
        //public TerrainBrowserVM(TerrainOwnerElement owner, Action<BaseViewModel> ownerValidationRules) :base()
        public TerrainBrowserVM(TerrainOwnerElement owner, Editors.EditorActionManager actionManager) : base(actionManager)
        {
            
        }

        

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != null, "Terrain Name cannot be empty.");
            AddRule(nameof(Name), () => Name != "", "Terrain Name cannot be empty.");

            AddRule(nameof(OriginalPath), () => OriginalPath != null, "Path cannot be null.");
            AddRule(nameof(OriginalPath), () => OriginalPath != "", "Path cannot be null.");

            AddRule(nameof(TerrainPath), () =>
            { return System.IO.File.Exists(TerrainPath) != true; }, "A file with this name already exists.");

        }

        public override void Save()
        {
           

            if (TerrainPath != null && TerrainPath != "")
            {
                // string[] pathNames = new string[] { OriginalPath, TerrainPath };
                // CopyFileOnBackgroundThread(this, new DoWorkEventArgs(pathNames));

                //add a dummy element to the parent
                TerrainElement t = new TerrainElement(Name,System.IO.Path.GetFileName(TerrainPath),true); // file extention?
                StudyCache.TerrainParent.AddElement(t);
                TerrainElement newElement = new TerrainElement(Name, TerrainPath);

                Saving.PersistenceManagers.TerrainElementPersistenceManager manager = Saving.PersistenceFactory.GetTerrainManager();
                manager.OriginalTerrainPath = OriginalPath;
                manager.SaveNew(newElement);
            }
            
            
        }

        //private async  void CopyFileOnBackgroundThread(object sender, DoWorkEventArgs e)
        //{
        //    string[] pathNames = (string[])e.Argument;
        //    //System.IO.File.Copy(pathNames[0], pathNames[1]);
        //    await Task.Run(() => System.IO.File.Copy(pathNames[0], pathNames[1]));
        //    //TerrainFileFinishedCopying?.Invoke(sender, e);
        //    TerrainBrowserVM vm = (TerrainBrowserVM)sender;
        //    string name = vm.Name;

        //    //remove the temporary node and replace it
        //    foreach (Utilities.ChildElement elem in vm.TerrainOwnerElement.Elements)
        //    {
        //        if (elem.Name.Equals(name))
        //        {
        //            vm.TerrainOwnerElement.Elements.Remove(elem);
        //            break;
        //        }
        //    }
        //    vm.TerrainOwnerElement.AddElement(new TerrainElement(name, System.IO.Path.GetFileName(vm.TerrainPath), false));


        //}

        #endregion
        #region Voids
        private void CreateNewPathName()
        {
            if(Name == null || Name == "") { return; }
            if(OriginalPath == null || OriginalPath == "") { return; }
            string originalExtension = System.IO.Path.GetExtension(OriginalPath);
            string destinationFilePath = Storage.Connection.Instance.TerrainDirectory + "\\" + Name + originalExtension;
            TerrainPath = destinationFilePath;
        }
        #endregion
        #region Functions
        #endregion
    }
}
