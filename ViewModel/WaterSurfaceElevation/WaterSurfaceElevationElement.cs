using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using LifeSimGIS;
using OpenGLMapping;

namespace HEC.FDA.ViewModel.WaterSurfaceElevation
{
    //[Author(q0heccdm, 9 / 6 / 2017 9:47:42 AM)]
    public class WaterSurfaceElevationElement : ChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 9/6/2017 9:47:42 AM
        #endregion
        #region Fields
        private List<PathAndProbability> _RelativePathAndProbability;
        private List<int> _featureNodeHashs;
        #endregion
        #region Properties
     
        public bool IsDepthGrids { get; set; }
       
        public List<PathAndProbability> RelativePathAndProbability
        {
            get { return _RelativePathAndProbability; }
            set { _RelativePathAndProbability = value;  }
        }
        public bool HasAssociatedFiles { get; set; }

        #endregion
        #region Constructors
        /// <summary>
        /// This constructor is only used when importing from old fda files. Old fda does not have paths to map layer files.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="isDepthGrids"></param>
        public WaterSurfaceElevationElement(string name, string description,List<double> probabilites, bool isDepthGrids, int id):base(id)
        {
            HasAssociatedFiles = false;
            List<PathAndProbability> pathAndProbs = new List<PathAndProbability>();
            foreach(double p in probabilites)
            {
                pathAndProbs.Add(new PathAndProbability("NA", p));
            }
            SetConstructorParams(name, description,pathAndProbs, isDepthGrids);
        }
        public WaterSurfaceElevationElement(string name, string description, List<PathAndProbability> relativePathAndProbabilities,bool isDepthGrids, int id) : base(id)
        {
            HasAssociatedFiles = true;
            SetConstructorParams(name, description,relativePathAndProbabilities, isDepthGrids);
        }

        private void SetConstructorParams(string name, string description,List<PathAndProbability> pathAndProbs, bool isDepthGrids)
        {
            RelativePathAndProbability = pathAndProbs;
            Name = name;
            Description = description;
            if (Description == null)
            {
                Description = "";
            }
            IsDepthGrids = isDepthGrids;
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/WaterSurfaceElevation.png");

            NamedAction editElement = new NamedAction(this);
            editElement.Header = "Edit Hydraulics...";
            editElement.Action = EditElement;

            NamedAction remove = new NamedAction();
            remove.Header = StringConstants.REMOVE_MENU;
            remove.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = StringConstants.RENAME_MENU;
            renameElement.Action = Rename;

            NamedAction mapWindow = new NamedAction();
            mapWindow.Header = StringConstants.ADD_TO_MAP_WINDOW_MENU;
            mapWindow.Action = AddWSEToMapWindow;

            //"NA" has been placed in the "path" column of the database. That means that this WSE came
            //from old FDA and doesn't have a path associated with it and so we disable this menu item.
            bool hasMapLayers = true;
            if(pathAndProbs.Count>0)
            {
                if(pathAndProbs[0].Path.Equals("NA"))
                {
                    hasMapLayers = false;
                }
            }

            if (!hasMapLayers)
            {
                mapWindow.IsEnabled = false;
                mapWindow.ToolTip = "No map layers exist when imported from HEC-FDA 1.4.3";
            }

            List<NamedAction> localactions = new List<NamedAction>();
            localactions.Add(editElement);
            localactions.Add(remove);
            localactions.Add(renameElement);
            localactions.Add(mapWindow);

            Actions = localactions;
            TableContainsGeoData = true;
        }

        #endregion
        #region Voids
        public void EditElement(object sender, EventArgs e)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
               .WithSiblingRules(this);

            WaterSurfaceElevationImporterVM vm = new WaterSurfaceElevationImporterVM(this, actionManager);

            string header = "Edit Hydraulics -" + Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditWatSurfElev" + Name);
            Navigate(tab, false, false);
        }
        public void RemoveElement(object sender, EventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete '" + Name + "'?", "Delete " + Name + "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Saving.PersistenceFactory.GetWaterSurfaceManager().Remove(this);
            }
        }
        public override void RemoveElementFromMapWindow(object arg1, EventArgs arg2)
        {
            if (_featureNodeHashs != null)
            {
                foreach (int hash in _featureNodeHashs)
                {
                    RemoveFromMapWindow(this, new RemoveMapFeatureEventArgs(hash));
                }
                foreach (NamedAction a in Actions)
                {
                    if (a.Header.Equals(StringConstants.REMOVE_FROM_MAP_WINDOW_MENU))
                    {
                        a.Header = StringConstants.ADD_TO_MAP_WINDOW_MENU;
                        a.Action = AddWSEToMapWindow;
                    }
                }
            }
        }

        public void RemovedCallback(FeatureNodeHeader node, bool includeSelected)
        {
            foreach (NamedAction a in Actions)
            {
                if (a.Header.Equals(StringConstants.REMOVE_FROM_MAP_WINDOW_MENU))
                {
                    a.Header = StringConstants.ADD_TO_MAP_WINDOW_MENU;
                    a.Action = AddWSEToMapWindow;
                }
            }
        }

        private string GetVRTFilePath(string vrtDirectoryPath)
        {
            string vrtFilePath = null;
            try
            {
                //get the vrt from the directory 
                string[] fileList = Directory.GetFiles(vrtDirectoryPath);
                foreach (string file in fileList)
                {
                    if (Path.GetExtension(file) == ".vrt")
                    {
                        vrtFilePath = file;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "Unable to find a .vrt file in directory: " + vrtDirectoryPath +
                    Environment.NewLine + ex.Message;
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            return vrtFilePath;
        }

        private void AddWSEToMapWindow(object arg1, EventArgs arg2)
        {
            _featureNodeHashs = new List<int>();
            foreach (PathAndProbability directory in RelativePathAndProbability)
            {
                string vrtDirectoryPath = Storage.Connection.Instance.HydraulicsDirectory + "\\" + directory.Path;
                string vrtFilePath = GetVRTFilePath(vrtDirectoryPath);
                if (vrtFilePath != null)
                {                 
                    RasterFeatures r = new RasterFeatures(vrtFilePath);
                    ColorRamp c = new ColorRamp(ColorRamp.RampType.LightBlueDarkBlue, r.GridReader.Max, r.GridReader.Min, r.GridReader.Mean, r.GridReader.StdDev);
                    AddGriddedDataEventArgs args = new AddGriddedDataEventArgs(r, c);
                    args.FeatureName = Name + " - " + Path.GetFileName(vrtDirectoryPath);
                    AddToMapWindow(this, args);

                    _featureNodeHashs.Add(args.MapFeatureHash);
                }
            }

            foreach (NamedAction a in Actions)
            {
                if (a.Header.Equals(StringConstants.ADD_TO_MAP_WINDOW_MENU))
                {
                    a.Header = StringConstants.REMOVE_FROM_MAP_WINDOW_MENU;
                    a.Action = RemoveElementFromMapWindow;
                }
            }
        }

        public override void Rename(object sender, EventArgs e)
        {
            string originalName = Name;
            RenameVM renameViewModel = new RenameVM(this, CloneElement);
            string header = "Rename";
            DynamicTabVM tab = new DynamicTabVM(header, renameViewModel, "Rename");
            Navigate(tab);
            string newName = renameViewModel.Name;
            //rename the folders in the study.
            if (!originalName.Equals(newName))
            {
                string sourceFilePath = Connection.Instance.HydraulicsDirectory + "\\" + originalName;
                string destinationFilePath = Connection.Instance.HydraulicsDirectory + "\\" + newName;
                Directory.Move(sourceFilePath, destinationFilePath);
            }
            //rename the child table in the DB
            Saving.PersistenceFactory.GetWaterSurfaceManager().RenamePathAndProbabilitesTableName(originalName, newName);
        }


        #endregion
        #region Functions
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            WaterSurfaceElevationElement elem = (WaterSurfaceElevationElement)elementToClone;
            return new WaterSurfaceElevationElement(elem.Name, elem.Description,elem.RelativePathAndProbability,elem.IsDepthGrids, elem.ID);
        }
        #endregion
    }
}
