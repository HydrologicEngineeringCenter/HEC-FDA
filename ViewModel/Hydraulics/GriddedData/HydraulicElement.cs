using HEC.FDA.ViewModel.Hydraulics.SteadyHDF;
using HEC.FDA.ViewModel.Hydraulics.UnsteadyHDF;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Hydraulics.GriddedData
{
    //[Author(q0heccdm, 9 / 6 / 2017 9:47:42 AM)]
    public class HydraulicElement : ChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 9/6/2017 9:47:42 AM
        #endregion
        #region Fields

        public const string HYDRAULIC_XML_TAG = "HydraulicElement";
        public const string HYDRAULIC_TYPE_XML_TAG = "HydroType";
        public const string IS_DEPTH_GRID_XML_TAG = "IsDepthGrid";
        private const string PATH_AND_PROBS = "PathAndProbabilities";

        #endregion
        #region Properties
        public HydraulicType HydroType{get;set;}
        public bool IsDepthGrids { get; set; }

        public List<PathAndProbability> RelativePathAndProbability { get; } = new List<PathAndProbability>();


        #endregion
        #region Constructors
        /// <summary>
        /// This constructor is only used when importing from old fda files. Old fda does not have paths to map layer files.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="isDepthGrids"></param>
        public HydraulicElement(string name, string description,List<double> probabilites, bool isDepthGrids, HydraulicType hydroType, int id)
            :base(name, "", description,  id)
        {
            HydroType = hydroType;
            List<PathAndProbability> pathAndProbs = new List<PathAndProbability>();
            foreach(double p in probabilites)
            {
                pathAndProbs.Add(new PathAndProbability("NA", p));
            }
            SetConstructorParams(name, description,pathAndProbs, isDepthGrids);
        }

        public HydraulicElement(string name, string description, List<PathAndProbability> relativePathAndProbabilities,bool isDepthGrids, HydraulicType hydroType, int id) : base(id)
        {
            HydroType = hydroType;
            SetConstructorParams(name, description,relativePathAndProbabilities, isDepthGrids);
        }



        public HydraulicElement(XElement childElement, int id):base(id)
        {
            ID = id;
            ReadHeaderXElement(childElement.Element(HEADER_XML_TAG));
            string hydroType = childElement.Attribute(HYDRAULIC_TYPE_XML_TAG).Value;
            Enum.TryParse(hydroType, out HydraulicType myHydroType);
            HydroType = myHydroType;

            IsDepthGrids = Convert.ToBoolean(childElement.Attribute(IS_DEPTH_GRID_XML_TAG).Value);


            XElement rowsElem = childElement.Element(PATH_AND_PROBS);


            IEnumerable<XElement> rowElems = rowsElem.Elements(PathAndProbability.PATH_AND_PROB);
            foreach (XElement elem in rowElems)
            {
                RelativePathAndProbability.Add(new PathAndProbability(elem));
            }

            CustomTreeViewHeader = new CustomHeaderVM(Name, ImageSources.IMPACT_AREAS_IMAGE);
            AddActions();
        }

   


        private void SetConstructorParams(string name, string description,List<PathAndProbability> pathAndProbs, bool isDepthGrids)
        {
            RelativePathAndProbability.AddRange( pathAndProbs);
            Name = name;
            Description = description;
            IsDepthGrids = isDepthGrids;
            CustomTreeViewHeader = new CustomHeaderVM(Name, ImageSources.WATER_SURFACE_ELEVATION_IMAGE);

            AddActions();
        }

        private void AddActions()
        {
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
        }

        #endregion
        #region Voids
        public void EditElement(object sender, EventArgs e)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
               .WithSiblingRules(this);

            string header = "Edit Hydraulics -" + Name;

            switch (HydroType)
            {
                case HydraulicType.Gridded:
                    GriddedImporterVM vm = new GriddedImporterVM(this, actionManager);
                    DynamicTabVM tab = new DynamicTabVM(header, vm, "EditWatSurfElevGridded" + Name);
                    Navigate(tab, false, false);
                    break;
                case HydraulicType.Steady:
                    SteadyHDFImporterVM steadyImporter = new SteadyHDFImporterVM(this, actionManager);
                    DynamicTabVM steadyTab = new DynamicTabVM(header, steadyImporter, "EditWatSurfElevSteady" + Name);
                    Navigate(steadyTab, false, false);
                    break;
                case HydraulicType.Unsteady:
                    UnsteadyHDFImporterVM unsteadyVM = new UnsteadyHDFImporterVM(this, actionManager);
                    DynamicTabVM unsteadyTab = new DynamicTabVM(header, unsteadyVM, "EditWatSurfElevUnsteady" + Name);
                    Navigate(unsteadyTab, false, false);
                    break;
            }

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
                    //Saving.PersistenceFactory.GetWaterSurfaceManager().RenamePathAndProbabilitesTableName(originalName, newName);
                }
            }
        }


        #endregion
        #region Functions
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            HydraulicElement elem = (HydraulicElement)elementToClone;
            return new HydraulicElement(elem.Name, elem.Description,elem.RelativePathAndProbability,elem.IsDepthGrids, elem.HydroType, elem.ID);
        }

        public override XElement ToXML()
        {
            XElement elem = new XElement(HYDRAULIC_XML_TAG);
            elem.Add(CreateHeaderElement());
            elem.SetAttributeValue(HYDRAULIC_TYPE_XML_TAG, HydroType);
            elem.SetAttributeValue(IS_DEPTH_GRID_XML_TAG, IsDepthGrids);

            //path and probs
            XElement pathAndProbsElem = new XElement(PATH_AND_PROBS);
            foreach (PathAndProbability pathAndProb in RelativePathAndProbability)
            {
                pathAndProbsElem.Add(pathAndProb.ToXML());
            }

            elem.Add(pathAndProbsElem);

            return elem;
        }


        public bool Equals(HydraulicElement elem)
        {
            /*
             *         public HydraulicType HydroType{get;set;}
        public bool IsDepthGrids { get; set; }

        public List<PathAndProbability> RelativePathAndProbability { get; } = new List<PathAndProbability>();
             */

            bool isEqual = true;

            if (!AreHeaderDataEqual(elem))
            {
                isEqual = false;
            }
            if (HydroType != elem.HydroType)
            {
                isEqual = false;
            }
            if(IsDepthGrids != elem.IsDepthGrids)
            {
                isEqual = false;
            }
            if(RelativePathAndProbability.Count != elem.RelativePathAndProbability.Count)
            {
                isEqual = false;
            }
            for(int i = 0;i<RelativePathAndProbability.Count;i++)
            {
                if(!RelativePathAndProbability[i].Equals(elem.RelativePathAndProbability[i]))
                {
                    isEqual = false;
                    break;
                }
            }

            return isEqual;
        }

        #endregion
    }
}
