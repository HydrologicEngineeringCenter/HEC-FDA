using HEC.FDA.Model.Spatial;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.stageDamage;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System.Collections.Generic;
using System.IO;
using HEC.FDA.ViewModel.Hydraulics;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class StageDamageConfiguration:BaseViewModel
    {
        private int _AnalysisYear;
        public List<ImpactAreaFrequencyFunctionRowItem> ImpactAreaFrequencyRows { get; } = new List<ImpactAreaFrequencyFunctionRowItem>();
        public InventoryElement SelectedStructures { get; }
        public HydraulicElement SelectedHydraulics { get; }
        public ImpactAreaElement SelectedImpactArea { get; }

        public StageDamageConfiguration(ImpactAreaElement impAreaElem, HydraulicElement hydroElem, InventoryElement inventoryElem,
             List<ImpactAreaFrequencyFunctionRowItem> impactAreaRows, int analysisYear)
        {
            _AnalysisYear = analysisYear;
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
                vr.AddErrorMessage(GetIsYearValidResult().ErrorMessage);
            }
            if(vr.IsValid)
            {
                vr.AddErrorMessage(SelectedStructures.AreMappingsValid().ErrorMessage);
            }
            if(vr.IsValid)
            {
                //validate the terrain only if we're using one
                if (SelectedStructures.SelectionMappings.IsUsingTerrainFile)
                {
                    vr.AddErrorMessage(GetIsTerrainValidResult().ErrorMessage);
                }
                else
                {
                    vr.AddErrorMessage(GetIsProjectionFileSet().ErrorMessage);
                }
            }
            return vr;
        }
        private static FdaValidationResult GetIsProjectionFileSet()
        {
            FdaValidationResult vr = new();
            if(!File.Exists(Connection.Instance.ProjectionFile))
            {
                vr.AddErrorMessage("The projection file has not been set.");
            }
            return vr;
        }
        private static FdaValidationResult GetIsTerrainValidResult()
        {
            FdaValidationResult vr = new();
            string terrainFile = InventoryColumnSelectionsVM.getTerrainFile();
            string error = "";
            if(!RASHelper.TerrainIsValid(terrainFile, ref error))
            {
                vr.AddErrorMessage(error);
                return vr;
            }
            return vr;
        }

        private FdaValidationResult GetIsYearValidResult()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (_AnalysisYear < 1900 || _AnalysisYear > 3000)
            {
                vr.AddErrorMessage("The analysis year must be greater than 1900 and less than 3000.");
            }
            return vr;
        }

        /// <summary>
        /// Validates that the hydros, structures, and frequency function table selections are all valid.
        /// </summary>
        /// <returns></returns>
        private FdaValidationResult GetAreAllSelectionsValidResult()
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
            FdaValidationResult vr = new();

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
            FdaValidationResult vr = new();
            string file = GetImpactAreaFile();
            string error = "";
            if (!RASHelper.ShapefileIsValid(file,ref error))
            {
                vr.AddErrorMessage(error);
                return vr;
            }
            if(!RASHelper.IsPolygonShapefile(file, ref error))
            {
                vr.AddErrorMessage(error);
                return vr;
            }   
            return vr;
        }

        private FdaValidationResult GetStructureInventoryFilesValidResult()
        {
            //impact areas will only be of type *.shp
            FdaValidationResult vr = new();
            string error = "";
            string file = GetStructuresShapefile();
            if(!RASHelper.ShapefileIsValid(file, ref error))
            {
                vr.AddErrorMessage(error);
                return vr;
            }
            if(!RASHelper.IsPointShapefile(file, ref error))
            {
                vr.AddErrorMessage(error);
                return vr;
            }
            return vr;
        }

        #endregion


        private string GetImpactAreaFile()
        {
            string directory = Path.Combine(Connection.Instance.ImpactAreaDirectory, SelectedImpactArea.Name);
            string file = Directory.GetFiles(directory, "*.shp")[0]; //Should check that there's not more than 1 here
            return file;
        }

        private string GetStructuresShapefile()
        {
            string directory = Path.Combine(Connection.Instance.InventoryDirectory, SelectedStructures.Name);
            string file = Directory.GetFiles(directory, "*.shp")[0]; //Should check that there's not more than 1 here   
            return file;
        }
      

        /// <summary>
        /// Make sure to call the Validate() method before calling this.
        /// </summary>
        /// <returns></returns>
        public List<ImpactAreaStageDamage> CreateStageDamages()
        {
            Model.structures.Inventory inv = SelectedStructures.CreateModelInventory(SelectedImpactArea);
            
            string hydroParentDirectory = SelectedHydraulics.GetDirectoryInStudy();

            List<ImpactAreaStageDamage> stageDamages = new List<ImpactAreaStageDamage>();

            foreach (ImpactAreaFrequencyFunctionRowItem impactAreaRow in ImpactAreaFrequencyRows)
            {
                int impactAreaId = impactAreaRow.ImpactArea.ID;

                //we want to know if it is flow or stage
                FrequencyElement freqElement = impactAreaRow.FrequencyFunction.Element;
                bool isGraphical = !freqElement.IsAnalytical;
                bool isFlow = true;
                if (isGraphical)
                {
                    isFlow = freqElement.GraphicalUsesFlow;
                }
                UncertainPairedData regulatedUnregulatedFunction = null;
                if(impactAreaRow.RegulatedUnregulatedFunction != null && impactAreaRow.RegulatedUnregulatedFunction.Element != null)
                {
                    regulatedUnregulatedFunction = impactAreaRow.RegulatedUnregulatedFunction.Element.CurveComponentVM.SelectedItemToPairedData();
                }
                GraphicalUncertainPairedData graphicaluncertPairedData = freqElement.GraphicalUncertainPairedData;

                if (isGraphical)
                {
                    if (isFlow)
                    {
                        UncertainPairedData stageDischargePairedData = impactAreaRow.StageDischargeFunction.Element.CurveComponentVM.SelectedItemToPairedData();
                        stageDamages.Add(new ImpactAreaStageDamage(impactAreaId, inv, SelectedHydraulics.DataSet, hydroParentDirectory, graphicalFrequency: graphicaluncertPairedData,
                            dischargeStage: stageDischargePairedData, unregulatedRegulated: regulatedUnregulatedFunction, analysisYear: _AnalysisYear));

                    }
                    else
                    {
                        stageDamages.Add(new ImpactAreaStageDamage(impactAreaId, inv, SelectedHydraulics.DataSet, hydroParentDirectory, graphicalFrequency: graphicaluncertPairedData,
                            unregulatedRegulated: regulatedUnregulatedFunction, analysisYear: _AnalysisYear));
                    }
                }
                else
                {
                    Statistics.Distributions.LogPearson3 logPearson3 = freqElement.LPIII;
                    UncertainPairedData stageDischargePairedData = impactAreaRow.StageDischargeFunction.Element.CurveComponentVM.SelectedItemToPairedData();
                    stageDamages.Add(new ImpactAreaStageDamage(impactAreaId, inv, SelectedHydraulics.DataSet, hydroParentDirectory, analyticalFlowFrequency: logPearson3,
                        dischargeStage: stageDischargePairedData, unregulatedRegulated: regulatedUnregulatedFunction, analysisYear: _AnalysisYear));
                }

            }

            return stageDamages;

        }


    }
}
