using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using paireddata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class StageDamageConfiguration
    {
        public List<ImpactAreaFrequencyFunctionRowItem> ImpactAreaFrequencyRows { get; } = new List<ImpactAreaFrequencyFunctionRowItem>();
        public InventoryElement SelectedStructures { get; }
        public HydraulicElement SelectedHydraulics { get; }
        public IndexPointsElement SelectedIndexPoints { get; }
        public ImpactAreaElement SelectedImpactArea { get; }

        public StageDamageConfiguration(ImpactAreaElement impAreaElem, HydraulicElement hydroElem, InventoryElement inventoryElem,
            IndexPointsElement indexPointsElem, List<ImpactAreaFrequencyFunctionRowItem> impactAreaRows)
        {
            SelectedImpactArea = impAreaElem;
            SelectedStructures = inventoryElem;
            SelectedIndexPoints = indexPointsElem;
            SelectedHydraulics = hydroElem;
            ImpactAreaFrequencyRows = impactAreaRows;
        }

        #region validation

        public FdaValidationResult Validate()
        {
            FdaValidationResult vr = new FdaValidationResult();

            vr.AddErrorMessage(GetAreAllSelectionsValidResult().ErrorMessage);

            vr.AddErrorMessage(DoAllRequiredFilesExist().ErrorMessage);


            return vr;
        }

        public FdaValidationResult GetAreAllSelectionsValidResult()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (SelectedHydraulics == null)
            {
                vr.AddErrorMessage("A hydraulics data set is required to compute.");
            }
            if (SelectedStructures == null)
            {
                vr.AddErrorMessage("A structure inventory selection is required to compute.");
            }
            if (SelectedIndexPoints == null)
            {
                vr.AddErrorMessage("An index points selection is required to compute.");
            }

            vr.AddErrorMessage(ValidateImpactAreaFrequencyFunctionTable().ErrorMessage);

            return vr;
        }


        private FdaValidationResult ValidateImpactAreaFrequencyFunctionTable()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (ImpactAreaFrequencyRows.Count == 0)
            {
                vr.AddErrorMessage("Impact area table has no rows.");
            }

            foreach (ImpactAreaFrequencyFunctionRowItem row in ImpactAreaFrequencyRows)
            {
                vr.AddErrorMessage(row.ValidateRow().ErrorMessage);
            }
            return vr;
        }


        private FdaValidationResult DirectoryHasOneFileMatchingPattern(string directoryPath, string pattern)
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath, pattern);
                if (files.Length == 0)
                {
                    vr.AddErrorMessage("The directory does not contain a file that matches the pattern: " + pattern);
                }
                else if (files.Length > 1)
                {
                    //more than one shapefile discovered
                    vr.AddErrorMessage("The directory contains multiple files that matche the pattern: " + pattern);
                }
            }
            else
            {
                vr.AddErrorMessage("The directory does not exist: " + directoryPath);
            }
            return vr;
        }

        /// <summary>
        /// Always validate that this file exists before calling. Use the method above, DirectoryHasOneFileMatchingPattern(). 
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private string GetFilePath(string directoryPath, string pattern)
        {
            return Directory.GetFiles(directoryPath, pattern)[0];
        }

        private FdaValidationResult DoAllRequiredFilesExist()
        {
            //required files
            //impact area shapefile 
            //Index Points Shapefile
            //hydro file? need to know the type:
            //gridded
            //steady path to hdf
            //unsteady - all 8 hdf files
            //path and probs 

            FdaValidationResult vr = new FdaValidationResult();

            vr.AddErrorMessage(GetImpactAreaFilesValidResult().ErrorMessage);
            vr.AddErrorMessage(GetHydroFilesValidResult().ErrorMessage);
            vr.AddErrorMessage(GetIndexPointsFilesValidResult().ErrorMessage);
            //todo: others?


            return vr;
        }
        private FdaValidationResult GetHydroFilesValidResult()
        {
            //todo: finish this
            FdaValidationResult vr = new FdaValidationResult();
            string hydroDirectoryPath = Connection.Instance.HydraulicsDirectory + "\\" + SelectedHydraulics.Name;

            switch(SelectedHydraulics.HydroType)
            {
                case HydraulicType.Gridded:

                    break;
                case HydraulicType.Steady:

                    break;
                case HydraulicType.Unsteady:

                    break;

            }

            //todo: these lines no longer work. Hydros can come in different forms now: gridded, steady, unsteady
            //vr.AddErrorMessage(DirectoryHasOneFileMatchingPattern(hydroDirectoryPath, "*.shp").ErrorMessage);
            //todo: do we need to check that a dbf exists?
            //vr.AddErrorMessage(DirectoryHasOneFileMatchingPattern(hydroDirectoryPath, "*.dbf").ErrorMessage);

            return vr;
        }

        private FdaValidationResult GetImpactAreaFilesValidResult()
        {
            //impact areas will only be of type *.shp
            FdaValidationResult vr = new FdaValidationResult();
            string impactAreaDirectoryPath = GetImpactAreaDirectory();
            vr.AddErrorMessage( DirectoryHasOneFileMatchingPattern(impactAreaDirectoryPath, "*.shp").ErrorMessage);
            //todo: do we need to check that a dbf exists?
            vr.AddErrorMessage(DirectoryHasOneFileMatchingPattern(impactAreaDirectoryPath, "*.dbf").ErrorMessage);

            return vr;
        }

        private FdaValidationResult GetIndexPointsFilesValidResult()
        {
            //impact areas will only be of type *.shp
            FdaValidationResult vr = new FdaValidationResult();
            string indexPointsDirectoryPath = GetIndexPointsDirectory();
            vr.AddErrorMessage(DirectoryHasOneFileMatchingPattern(indexPointsDirectoryPath, "*.shp").ErrorMessage);
            //todo: do we need to check that a dbf exists?
            vr.AddErrorMessage(DirectoryHasOneFileMatchingPattern(indexPointsDirectoryPath, "*.dbf").ErrorMessage);

            return vr;
        }

        #endregion


        public string GetImpactAreaDirectory()
        {
            return Connection.Instance.ImpactAreaDirectory + "\\" + SelectedImpactArea.Name;
        }

        public string GetImpactAreaShapefile()
        {
            return Directory.GetFiles(GetImpactAreaDirectory(), "*.shp")[0];
        }

        public string GetIndexPointsDirectory()
        {
            return Connection.Instance.IndexPointsDirectory + "\\" + SelectedIndexPoints.Name;
        }

        public string GetIndexPointsShapefile()
        {
            return Directory.GetFiles(GetIndexPointsDirectory(), "*.shp")[0];
        }

        public string GetHydraulicsDirectory()
        {
            return Connection.Instance.HydraulicsDirectory + "\\" + SelectedHydraulics.Name;
        }

        public List<ImpactAreaFrequencyFunctionConfigurationRowItem> GetImpactAreaFrequencyRowItems()
        {
            List<ImpactAreaFrequencyFunctionConfigurationRowItem> rows = new List<ImpactAreaFrequencyFunctionConfigurationRowItem>();
            foreach (ImpactAreaFrequencyFunctionRowItem item in ImpactAreaFrequencyRows)
            {
                UncertainPairedData freqUPD = item.FrequencyFunction.Element.ComputeComponentVM.SelectedItemToPairedData();
                UncertainPairedData stageDischargeUPD = null;
                if (item.IsStageDischargeRequired())
                {
                    stageDischargeUPD = item.StageDischargeFunction.Element.ComputeComponentVM.SelectedItemToPairedData();
                }

                rows.Add(new ImpactAreaFrequencyFunctionConfigurationRowItem(item.ImpactArea.Name, freqUPD, stageDischargeUPD));
            }
            return rows;
        }

    }
}
