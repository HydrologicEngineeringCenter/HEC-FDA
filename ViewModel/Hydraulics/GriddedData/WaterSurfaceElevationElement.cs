using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using LifeSimGIS;
using OpenGLMapping;

namespace HEC.FDA.ViewModel.Hydraulics.GriddedData
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
            CustomTreeViewHeader = new CustomHeaderVM(Name, ImageSources.WATER_SURFACE_ELEVATION_IMAGE);

            NamedAction editElement = new NamedAction(this);
            editElement.Header = StringConstants.EDIT_HYDRAULICS_MENU;
            editElement.Action = EditElement;

            NamedAction remove = new NamedAction();
            remove.Header = StringConstants.REMOVE_MENU;
            remove.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = StringConstants.RENAME_MENU;
            renameElement.Action = Rename;

            List<NamedAction> localactions = new List<NamedAction>();
            localactions.Add(editElement);
            localactions.Add(remove);
            localactions.Add(renameElement);

            Actions = localactions;
            TableContainsGeoData = true;
        }

        #endregion
        #region Voids
        public void EditElement(object sender, EventArgs e)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
               .WithSiblingRules(this);

            GriddedImporterVM vm = new GriddedImporterVM(this, actionManager);

            string header = "Edit Hydraulics -" + Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditWatSurfElev" + Name);
            Navigate(tab, false, false);
        }            

        public override void Rename(object sender, EventArgs e)
        {
            string originalName = Name;
            RenameVM renameViewModel = new RenameVM(this, CloneElement);
            string header = "Rename";
            DynamicTabVM tab = new DynamicTabVM(header, renameViewModel, "Rename");
            Navigate(tab);
            if (!renameViewModel.WasCanceled)
            {
                string newName = renameViewModel.Name;
                //rename the folders in the study.
                if (!originalName.Equals(newName))
                {
                    string sourceFilePath = Connection.Instance.HydraulicsDirectory + "\\" + originalName;
                    string destinationFilePath = Connection.Instance.HydraulicsDirectory + "\\" + newName;
                    Directory.Move(sourceFilePath, destinationFilePath);

                    //rename the child table in the DB
                    Saving.PersistenceFactory.GetWaterSurfaceManager().RenamePathAndProbabilitesTableName(originalName, newName);
                }
            }
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
