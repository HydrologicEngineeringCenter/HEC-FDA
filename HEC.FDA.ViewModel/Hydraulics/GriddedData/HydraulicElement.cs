﻿using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.Model.hydraulics.Interfaces;
using HEC.FDA.ViewModel.Hydraulics.SteadyHDF;
using HEC.FDA.ViewModel.Hydraulics.UnsteadyHDF;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        public HydraulicElement(string name, string description, List<HydraulicProfile> relativePathAndProbabilities, HydraulicDataSource hydroType, int id)
            : base(name, "", description, id)
        {
            DataSet = new HydraulicDataset(new List<IHydraulicProfile>(relativePathAndProbabilities), hydroType);
            AddDefaultActions(EditElement, StringConstants.EDIT_HYDRAULICS_MENU);
        }

        public HydraulicElement(XElement childElement, int id) : base(childElement, id)
        {
            DataSet = new HydraulicDataset();
            bool success = DataSet.LoadFromXML(childElement.Element(HydraulicDataset.HYDRAULIC_DATA_SET));
            if (!success)
            {
                //if we were able to get the dataSource, we use it. Else, it'll just be the default so it will show in the tree. 
                DataSet.DataSource = HydraulicDataSource.SteadyHDF;
                MessageBox.Show("Error loading hydraulic element from database.");
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

            switch (DataSet.DataSource)
            {
                case HydraulicDataSource.WSEGrid:
                    GriddedImporterVM vm = new GriddedImporterVM(this, actionManager);
                    vm.RequestNavigation += Navigate;
                    DynamicTabVM tab = new DynamicTabVM(header, vm, "EditWatSurfElevGridded" + Name);
                    Navigate(tab, false, false);
                    break;
                case HydraulicDataSource.SteadyHDF:
                    SteadyHDFImporterVM steadyImporter = new SteadyHDFImporterVM(this, actionManager);
                    steadyImporter.RequestNavigation += Navigate;
                    DynamicTabVM steadyTab = new DynamicTabVM(header, steadyImporter, "EditWatSurfElevSteady" + Name);
                    Navigate(steadyTab, false, false);
                    break;
                case HydraulicDataSource.UnsteadyHDF:
                    UnsteadyHDFImporterVM unsteadyVM = new UnsteadyHDFImporterVM(this, actionManager);
                    unsteadyVM.RequestNavigation += Navigate;
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
            if (DataSet.HydraulicProfiles.Count != elem.DataSet.HydraulicProfiles.Count)
            {
                isEqual = false;
            }
            for (int i = 0; i < DataSet.HydraulicProfiles.Count; i++)
            {
                if (!DataSet.HydraulicProfiles[i].Equals(elem.DataSet.HydraulicProfiles[i]))
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

        private FdaValidationResult AreUnsteadyFilesValid()
        {
            FdaValidationResult vr = new FdaValidationResult();

            foreach (HydraulicProfile profile in DataSet.HydraulicProfiles)
            {
                string filePath = profile.GetFilePath(GetDirectoryInStudy());
                if (!File.Exists(filePath))
                {
                    vr.AddErrorMessage("Missing file: " + filePath);
                }
            }
            if (!vr.IsValid)
            {
                vr.InsertMessage(0, "The selected hydraulics is missing expected files:");
            }
            return vr;
        }

        private FdaValidationResult AreGriddedFilesValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            foreach (HydraulicProfile profile in DataSet.HydraulicProfiles)
            {
                string filePath = profile.GetFilePath(GetDirectoryInStudy());
                if (!File.Exists(filePath))
                {
                    vr.AddErrorMessage("Missing file: " + filePath);
                }
            }
            if (!vr.IsValid)
            {
                vr.InsertMessage(0, "The selected hydraulics is missing expected files:");
            }
            return vr;
        }

        private FdaValidationResult AreSteadyFilesValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            vr.AddErrorMessage(FileValidation.DirectoryHasOneFileMatchingPattern(GetDirectoryInStudy(), "*.hdf").ErrorMessage);
            if (!vr.IsValid)
            {
                vr.InsertMessage(0, "The selected hydraulic is missing expected files:");
            }
            return vr;
        }

        public FdaValidationResult AreFilesValidResult()
        {
            FdaValidationResult vr = new FdaValidationResult();

            switch (DataSet.DataSource)
            {
                case HydraulicDataSource.UnsteadyHDF:
                    {
                        vr.AddErrorMessage(AreUnsteadyFilesValid().ErrorMessage);
                        break;
                    }
                case HydraulicDataSource.WSEGrid:
                    {
                        vr.AddErrorMessage(AreGriddedFilesValid().ErrorMessage);
                        break;
                    }
                case HydraulicDataSource.SteadyHDF:
                    {
                        vr.AddErrorMessage(AreSteadyFilesValid().ErrorMessage);
                        break;
                    }
            }

            return vr;
        }



        #endregion
    }
}
