using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.ViewModel.Hydraulics.SteadyHDF;
using HEC.FDA.ViewModel.Hydraulics.UnsteadyHDF;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Hydraulics.GriddedData;

public class HydraulicElement : ChildElement, IHaveStudyFiles
{
    public const string HYDRAULIC_XML_TAG = "HydraulicElement";
    public HydraulicDataset DataSet { get; set; }

    public HydraulicElement(string name, string description, List<HydraulicProfile> relativePathAndProbabilities, HydraulicDataSource hydroType, int id)
        : base(name, "", description, id)
    {
        DataSet = new HydraulicDataset([.. relativePathAndProbabilities], hydroType);
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
    public void EditElement(object sender, EventArgs e)
    {
        Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
           .WithSiblingRules(this);

        string header = "Edit Hydraulics -" + Name;

        switch (DataSet.DataSource)
        {
            case HydraulicDataSource.WSEGrid:
                GriddedImporterVM griddenImporter = new(this, actionManager);
                griddenImporter.RequestNavigation += Navigate;
                DynamicTabVM gridTab = new(header, griddenImporter, "EditWatSurfElevGridded" + Name);
                Navigate(gridTab, false, false);
                break;
            case HydraulicDataSource.SteadyHDF:
                SteadyHDFImporterVM steadyImporter = new(this, actionManager);
                steadyImporter.RequestNavigation += Navigate;
                DynamicTabVM steadyTab = new(header, steadyImporter, "EditWatSurfElevSteady" + Name);
                Navigate(steadyTab, false, false);
                break;
            case HydraulicDataSource.UnsteadyHDF:
                UnsteadyHDFImporterVM unsteadyVM = new(this, actionManager);
                unsteadyVM.RequestNavigation += Navigate;
                DynamicTabVM unsteadyTab = new(header, unsteadyVM, "EditWatSurfElevUnsteady" + Name);
                Navigate(unsteadyTab, false, false);
                break;
        }

    }
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
}

