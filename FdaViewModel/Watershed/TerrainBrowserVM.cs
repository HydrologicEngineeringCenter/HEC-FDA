using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Watershed
{
    //[Author("q0heccdm", "10 / 11 / 2016 11:13:25 AM")]
    public class TerrainBrowserVM:BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/11/2016 11:13:25 AM
        #endregion
        #region Fields
        private string _TerrainName;
        private string _TerrainPath;
        private string _OriginalPath;
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

        

        public string TerrainName
        {
            get { return _TerrainName; }
            set
            {
                _TerrainName = value; CreateNewPathName(); NotifyPropertyChanged();

            }
        }
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
        public TerrainBrowserVM():base()
        {
            //_TerrainPath = "C:\\temp\\FDA\\";
            //_TerrainName = "Example";
            
        }

        public override void AddValidationRules()
        {
            AddRule(nameof(TerrainName), () => TerrainName != null, "Terrain Name cannot be empty.");
            AddRule(nameof(TerrainName), () => TerrainName != "", "Terrain Name cannot be empty.");

            AddRule(nameof(OriginalPath), () => OriginalPath != null, "Path cannot be null.");
            AddRule(nameof(OriginalPath), () => OriginalPath != "", "Path cannot be null.");

            AddRule(nameof(TerrainName), () =>
            {
                if (System.IO.File.Exists(TerrainPath) == true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }, "A file with this name already exists.");

        }

        public override void Save()
        {
            
            if (TerrainPath == null || TerrainPath == "")
            { }//this shouldn't be possible
            else
            {
                System.IO.File.Copy(OriginalPath, TerrainPath);
            }
            
            
        }


        #endregion
        #region Voids
        private void CreateNewPathName()
        {
            if(TerrainName == null || TerrainName == "") { return; }
            if(OriginalPath == null || OriginalPath == "") { return; }
            string originalExtension = System.IO.Path.GetExtension(OriginalPath);
            string destinationFilePath = Storage.Connection.Instance.TerrainDirectory + "\\" + TerrainName + originalExtension;
            TerrainPath = destinationFilePath;
        }
        #endregion
        #region Functions
        #endregion
    }
}
