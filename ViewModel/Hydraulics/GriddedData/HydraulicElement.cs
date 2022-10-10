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

        #endregion
        #region Properties

        public HydraulicDataset DataSet { get; set; }

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
            List<HydraulicProfile> pathAndProbs = new List<HydraulicProfile>();
            foreach (double p in probabilites)
            {
                pathAndProbs.Add(new HydraulicProfile(p, "NA"));
            }
            DataSet = new HydraulicDataset(pathAndProbs, hydroType, isDepthGrids);
            AddDefaultActions(EditElement, StringConstants.EDIT_HYDRAULICS_MENU);
        }

        public HydraulicElement(string name, string description, List<HydraulicProfile> relativePathAndProbabilities,bool isDepthGrids, HydraulicDataSource hydroType, int id) 
            : base(name, "", description, id)
        {
            DataSet = new HydraulicDataset(relativePathAndProbabilities, hydroType, isDepthGrids);
            AddDefaultActions(EditElement, StringConstants.EDIT_HYDRAULICS_MENU);
        }

        public HydraulicElement(XElement childElement, int id):base(childElement, id)
        {
            DataSet = new HydraulicDataset(childElement.Element(HydraulicDataset.HYDRAULIC_DATA_SET));
            AddDefaultActions(EditElement, StringConstants.EDIT_HYDRAULICS_MENU);
        }

        #endregion
        #region Voids
        public void EditElement(object sender, EventArgs e)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
               .WithSiblingRules(this);

            string header = "Edit Hydraulics -" + Name;

            switch (DataSet.DataSource)
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
            elem.Add(DataSet.ToXML());
            return elem;
        }

        public bool Equals(HydraulicElement elem)
        {
            bool isEqual = true;

            if (!AreHeaderDataEqual(elem))
            {
                isEqual = false;
            }
            if (DataSet.DataSource != elem.DataSet.DataSource)
            {
                isEqual = false;
            }
            if(DataSet.IsDepthGrids != elem.DataSet.IsDepthGrids)
            {
                isEqual = false;
            }
            if(DataSet.HydraulicProfiles.Count != elem.DataSet.HydraulicProfiles.Count)
            {
                isEqual = false;
            }
            for(int i = 0;i< DataSet.HydraulicProfiles.Count;i++)
            {
                if(!DataSet.HydraulicProfiles[i].Equals(elem.DataSet.HydraulicProfiles[i]))
                {
                    isEqual = false;
                    break;
                }
            }

            return isEqual;
        }

        public string GetDirectoryInStudy()
        {
            return Storage.Connection.Instance.HydraulicsDirectory + "\\" + Name;
        }

        #endregion
    }
}
