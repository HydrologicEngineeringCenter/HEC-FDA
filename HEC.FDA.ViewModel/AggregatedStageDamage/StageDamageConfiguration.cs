﻿using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.stageDamage;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System.Collections.Generic;

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

        public FdaValidationResult ValidateConfiguration()
        {
            FdaValidationResult vr = new FdaValidationResult();
            vr.AddErrorMessage(GetAreAllSelectionsValidResult().ErrorMessage);
            //early exit if selections havn't been made
            if(vr.IsValid)
            {
                vr.AddErrorMessage(DoAllRequiredFilesExist().ErrorMessage);
            }
            if(vr.IsValid)
            {
                vr.AddErrorMessage(SelectedStructures.AreMappingsValid().ErrorMessage);
            }
            return vr;
        }

        /// <summary>
        /// Validates that the hydros, structures, and frequency function table selections are all valid.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Validates that the rows in the table are fully filled out and that there is at least one row.
        /// </summary>
        /// <returns></returns>
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

        private FdaValidationResult DoAllRequiredFilesExist()
        {
            FdaValidationResult vr = new FdaValidationResult();

            vr.AddErrorMessage(GetImpactAreaFilesValidResult().ErrorMessage);
            vr.AddErrorMessage(GetHydroFilesValidResult().ErrorMessage);
            vr.AddErrorMessage(GetStructureInventoryFilesValidResult().ErrorMessage);

            return vr;
        }
        private FdaValidationResult GetHydroFilesValidResult()
        {
            return SelectedHydraulics.AreFilesValidResult();
        }

        private FdaValidationResult GetImpactAreaFilesValidResult()
        {
            //impact areas will only be of type *.shp
            FdaValidationResult vr = new FdaValidationResult();
            string impactAreaDirectoryPath = GetImpactAreaDirectory();
            vr.AddErrorMessage(FileValidation.DirectoryHasOneFileMatchingPattern(impactAreaDirectoryPath, "*.shp").ErrorMessage);
            //todo: do we need to check that a dbf exists?
            vr.AddErrorMessage(FileValidation.DirectoryHasOneFileMatchingPattern(impactAreaDirectoryPath, "*.dbf").ErrorMessage);

            if(!vr.IsValid)
            {
                vr.InsertMessage(0, "Failed to find required impact area file(s):");
            }

            return vr;
        }

        private FdaValidationResult GetStructureInventoryFilesValidResult()
        {
            //impact areas will only be of type *.shp
            FdaValidationResult vr = new FdaValidationResult();
            string structuresDirectory = GetStructuresDirectory();
            vr.AddErrorMessage(FileValidation.DirectoryHasOneFileMatchingPattern(structuresDirectory, "*.shp").ErrorMessage);
            vr.AddErrorMessage(FileValidation.DirectoryHasOneFileMatchingPattern(structuresDirectory, "*.dbf").ErrorMessage);

            if (!vr.IsValid)
            {
                vr.InsertMessage(0, "Failed to find required structure inventory file(s):");
            }

            return vr;
        }

        #endregion


        private string GetImpactAreaDirectory()
        {
            return Connection.Instance.ImpactAreaDirectory + "\\" + SelectedImpactArea.Name;
        }

        private string GetStructuresDirectory()
        {
            return Connection.Instance.InventoryDirectory + "\\" + SelectedStructures.Name;
        }
      

        /// <summary>
        /// Make sure to call the Validate() method before calling this.
        /// </summary>
        /// <returns></returns>
        public List<ImpactAreaStageDamage> CreateStageDamages()
        {
            Model.structures.Inventory inv = SelectedStructures.CreateModelInventory(SelectedImpactArea);
            
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
                        UncertainPairedData stageDischargePairedData = impactAreaRow.StageDischargeFunction.Element.CurveComponentVM.SelectedItemToPairedData();
                        stageDamages.Add(new ImpactAreaStageDamage(impactAreaId, inv, SelectedHydraulics.DataSet, convergenceCriteria, hydroParentDirectory,
                            graphicalFrequency: graphicaluncertPairedData, dischargeStage: stageDischargePairedData));

                    }
                    else
                    {
                        GraphicalUncertainPairedData graphicaluncertPairedData = freqElement.MyGraphicalVM.GraphicalUncertainPairedData;
                        stageDamages.Add(new ImpactAreaStageDamage(impactAreaId, inv, SelectedHydraulics.DataSet, convergenceCriteria, hydroParentDirectory,
                            graphicalFrequency: graphicaluncertPairedData));
                    }
                }
                else
                {
                    Statistics.Distributions.LogPearson3 logPearson3 = freqElement.CreateAnalyticalLP3Distribution();
                    UncertainPairedData stageDischargePairedData = impactAreaRow.StageDischargeFunction.Element.CurveComponentVM.SelectedItemToPairedData();
                    stageDamages.Add(new ImpactAreaStageDamage(impactAreaId, inv, SelectedHydraulics.DataSet, convergenceCriteria, hydroParentDirectory,
                        analyticalFlowFrequency: logPearson3, dischargeStage:stageDischargePairedData));
                }

            }

            return stageDamages;

        }


    }
}