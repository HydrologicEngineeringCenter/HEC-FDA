using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.ViewModel.Hydraulics.SteadyHDF;
using HEC.FDA.ViewModel.Hydraulics.UnsteadyHDF;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Hydraulics.GriddedData
{
    //[Author(q0heccdm, 9 / 6 / 2017 9:47:42 AM)]
    public class HydraulicElement : ChildElement, IHaveStudyFiles
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

        //todo: replace this enum with model enum
        public HydraulicDataSource HydroType {get;set;}
        public bool IsDepthGrids { get; set; }

        public List<HydraulicProfile> Profiles { get; } = new List<HydraulicProfile>();

        #endregion
        #region Constructors
        /// <summary>
        /// This constructor is only used when importing from old fda files. Old fda does not have paths to map layer files.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="isDepthGrids"></param>
        public HydraulicElement(string name, string description,List<double> probabilites, bool isDepthGrids, HydraulicDataSource hydroType, int id)
            :base(name, "", description,  id)
        {
            HydroType = hydroType;
            List<HydraulicProfile> pathAndProbs = new List<HydraulicProfile>();
            foreach(double p in probabilites)
            {
                pathAndProbs.Add(new HydraulicProfile(p,"NA", hydroType,name));
            }
            Profiles.AddRange(pathAndProbs);
            IsDepthGrids = isDepthGrids;
            AddDefaultActions(EditElement, StringConstants.EDIT_HYDRAULICS_MENU);
        }

        public HydraulicElement(string name, string description, List<HydraulicProfile> relativePathAndProbabilities,bool isDepthGrids, HydraulicDataSource hydroType, int id) 
            : base(name, "", description, id)
        {
            HydroType = hydroType;
            Profiles.AddRange(relativePathAndProbabilities);
            IsDepthGrids = isDepthGrids;
            AddDefaultActions(EditElement, StringConstants.EDIT_HYDRAULICS_MENU);
        }


        public HydraulicElement(XElement childElement, int id):base(childElement, id)
        {
            string hydroType = childElement.Attribute(HYDRAULIC_TYPE_XML_TAG).Value;
            Enum.TryParse(hydroType, out HydraulicDataSource myHydroType);
            HydroType = myHydroType;

            IsDepthGrids = Convert.ToBoolean(childElement.Attribute(IS_DEPTH_GRID_XML_TAG).Value);

            XElement rowsElem = childElement.Element(PATH_AND_PROBS);

            IEnumerable<XElement> rowElems = rowsElem.Elements(PathAndProbability.PATH_AND_PROB);
            foreach (XElement elem in rowElems)
            {
                Profiles.Add(new HydraulicProfile(elem));
            }

            AddDefaultActions(EditElement, StringConstants.EDIT_HYDRAULICS_MENU);
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
                case HydraulicDataSource.WSEGrid:
                    GriddedImporterVM vm = new GriddedImporterVM(this, actionManager);
                    DynamicTabVM tab = new DynamicTabVM(header, vm, "EditWatSurfElevGridded" + Name);
                    Navigate(tab, false, false);
                    break;
                case HydraulicDataSource.SteadyHDF:
                    SteadyHDFImporterVM steadyImporter = new SteadyHDFImporterVM(this, actionManager);
                    DynamicTabVM steadyTab = new DynamicTabVM(header, steadyImporter, "EditWatSurfElevSteady" + Name);
                    Navigate(steadyTab, false, false);
                    break;
                case HydraulicDataSource.UnsteadyHDF:
                    UnsteadyHDFImporterVM unsteadyVM = new UnsteadyHDFImporterVM(this, actionManager);
                    DynamicTabVM unsteadyTab = new DynamicTabVM(header, unsteadyVM, "EditWatSurfElevUnsteady" + Name);
                    Navigate(unsteadyTab, false, false);
                    break;
            }

        }            
        #endregion
        #region Functions

        public override XElement ToXML()
        {
            XElement elem = new XElement(StringConstants.ELEMENT_XML_TAG);
            elem.Add(CreateHeaderElement());
            elem.SetAttributeValue(HYDRAULIC_TYPE_XML_TAG, HydroType);
            elem.SetAttributeValue(IS_DEPTH_GRID_XML_TAG, IsDepthGrids);

            //path and probs
            XElement pathAndProbsElem = new XElement(PATH_AND_PROBS);
            foreach (HydraulicProfile pathAndProb in Profiles)
            {
                pathAndProbsElem.Add(pathAndProb.ToXML());
            }

            elem.Add(pathAndProbsElem);

            return elem;
        }

        public bool Equals(HydraulicElement elem)
        {
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
            if(Profiles.Count != elem.Profiles.Count)
            {
                isEqual = false;
            }
            for(int i = 0;i<Profiles.Count;i++)
            {
                if(!Profiles[i].Equals(elem.Profiles[i]))
                {
                    isEqual = false;
                    break;
                }
            }

            return isEqual;
        }

        //todo: fix this. Maybe use this object yourself - yes, do that.
        //public List<HydraulicProfile> CreateProfiles()
        //{
        //    //todo: find the names of these rows.
        //    List <HydraulicProfile> profiles = new List<HydraulicProfile>();
        //    foreach (HydraulicProfile pathAndProb in RelativePathAndProbability)
        //    {
        //        //todo: put name in
        //        string name = "";
        //        //i need the whole path
        //        string path = Storage.Connection.Instance.HydraulicsDirectory + "\\" + Name + "\\" + pathAndProb.FilePath;
        //        profiles.Add( new HydraulicProfile(pathAndProb.Probability, path, HydroType, name));
        //    }
        //    return profiles;
        //}

        #endregion
    }
}
