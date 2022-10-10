using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.stageDamage;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System.Collections.Generic;
using System.IO;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class StageDamageConfiguration:BaseViewModel
    {
        public List<ImpactAreaFrequencyFunctionRowItem> ImpactAreaFrequencyRows { get; } = new List<ImpactAreaFrequencyFunctionRowItem>();
        public InventoryElement SelectedStructures { get; }
        public HydraulicElement SelectedHydraulics { get; }
        public ImpactAreaElement SelectedImpactArea { get; }

        public StageDamageConfiguration(ImpactAreaElement impAreaElem, HydraulicElement hydroElem, InventoryElement inventoryElem,
             List<ImpactAreaFrequencyFunctionRowItem> impactAreaRows)
        {
            SelectedImpactArea = impAreaElem;
            SelectedStructures = inventoryElem;
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
            //todo: others?


            return vr;
        }
        private FdaValidationResult GetHydroFilesValidResult()
        {
            //todo: finish this
            FdaValidationResult vr = new FdaValidationResult();
            string hydroDirectoryPath = Connection.Instance.HydraulicsDirectory + "\\" + SelectedHydraulics.Name;

            switch(SelectedHydraulics.DataSet.DataSource)
            {
                case HydraulicDataSource.WSEGrid:

                    break;
                case HydraulicDataSource.SteadyHDF:

                    break;
                case HydraulicDataSource.UnsteadyHDF:

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

        #endregion


        public string GetImpactAreaDirectory()
        {
            return Connection.Instance.ImpactAreaDirectory + "\\" + SelectedImpactArea.Name;
        }

        public string GetImpactAreaShapefile()
        {
            return Directory.GetFiles(GetImpactAreaDirectory(), "*.shp")[0];
        }

        public string GetHydraulicsDirectory()
        {
            return Connection.Instance.HydraulicsDirectory + "\\" + SelectedHydraulics.Name;
        }

        private string GetStructuresDirectory()
        {
            return Connection.Instance.InventoryDirectory + "\\" + SelectedStructures.Name;
        }

        //todo: add this to the validation to make sure it exists. Make all these methods private?
        private string GetStructuresPointShapefile()
        {
            return Directory.GetFiles(GetStructuresDirectory(), "*.shp")[0];
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

        

        public List<ImpactAreaStageDamage> CreateStageDamages()
        {
            Model.structures.Inventory inv = SelectedStructures.CreateModelInventory(SelectedImpactArea.Name);

            Model.hydraulics.HydraulicDataset hydros = new Model.hydraulics.HydraulicDataset(SelectedHydraulics.DataSet.HydraulicProfiles, SelectedHydraulics.DataSet.DataSource);
            string hydroParentDirectory = SelectedHydraulics.GetDirectoryInStudy();


            Study.StudyPropertiesElement propElem = StudyCache.GetStudyPropertiesElement();
            Statistics.ConvergenceCriteria convergenceCriteria = propElem.GetStudyConvergenceCriteria();

            List<ImpactAreaStageDamage> stageDamages = new List<ImpactAreaStageDamage>();

            foreach (ImpactAreaFrequencyFunctionRowItem impactAreaRow in ImpactAreaFrequencyRows)
            {
                int impactAreaId = impactAreaRow.ImpactArea.ID;

                //we want to know if it is flow or stage
                AnalyticalFrequencyElement freqElement = impactAreaRow.FrequencyFunction.Element;
                bool isGraphical = !freqElement.IsAnalytical;
                bool isFlow = true;
                if (isGraphical)
                {
                    isFlow = freqElement.MyGraphicalVM.UseFlow;
                }

                if (isGraphical)
                {
                    if (isFlow)
                    {
                        GraphicalUncertainPairedData graphicaluncertPairedData = freqElement.MyGraphicalVM.GraphicalUncertainPairedData;
                        UncertainPairedData stageDischargePairedData = impactAreaRow.StageDischargeFunction.Element.ComputeComponentVM.SelectedItemToPairedData();
                        stageDamages.Add(new ImpactAreaStageDamage(impactAreaId, inv, hydros, convergenceCriteria, hydroParentDirectory,
                            graphicalFrequency: graphicaluncertPairedData, dischargeStage: stageDischargePairedData));

                    }
                    else
                    {
                        GraphicalUncertainPairedData graphicaluncertPairedData = freqElement.MyGraphicalVM.GraphicalUncertainPairedData;
                        stageDamages.Add(new ImpactAreaStageDamage(impactAreaId, inv, hydros, convergenceCriteria, hydroParentDirectory,
                            graphicalFrequency: graphicaluncertPairedData));
                    }
                }
                else
                {
                    Statistics.Distributions.LogPearson3 logPearson3 = freqElement.CreateAnalyticalLP3Distribution();
                    UncertainPairedData stageDischargePairedData = impactAreaRow.StageDischargeFunction.Element.ComputeComponentVM.SelectedItemToPairedData();
                    stageDamages.Add(new ImpactAreaStageDamage(impactAreaId, inv, hydros, convergenceCriteria, hydroParentDirectory,
                        analyticalFlowFrequency: logPearson3, dischargeStage:stageDischargePairedData));
                }

            }

            return stageDamages;

        }


    }
}
