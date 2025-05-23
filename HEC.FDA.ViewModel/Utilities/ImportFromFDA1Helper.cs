﻿using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using Importer;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using static HEC.FDA.ViewModel.Inventory.OccupancyTypes.OccTypeAsset;
using static Importer.ProbabilityFunction;
using OccupancyType = HEC.FDA.ViewModel.Inventory.OccupancyTypes.OccupancyType;

namespace HEC.FDA.ViewModel.Utilities
{
    public static class ImportFromFDA1Helper
    {
        #region Stage-Discharge (Rating)
        public static List<StageDischargeElement> CreateRatingElements(RatingFunctionList ratings)
        {
            List<StageDischargeElement> elems = new();
            int id = PersistenceFactory.GetElementManager<StageDischargeElement>().GetNextAvailableId();
            int i = 0;
            foreach (KeyValuePair<string, RatingFunction> rat in ratings.RatingFunctions)
            {
                int elemID = id + i;
                StageDischargeElement elem = CreateRatingElement(rat.Value, elemID);
                if (elem != null)
                {
                    elems.Add(elem);
                    i++;
                }
            }
            return elems;
        }
        private static StageDischargeElement CreateRatingElement(RatingFunction rat, int elemID)
        {
            string description = CreatePYSRDescription(rat);

            UncertainPairedData ratingPairedData = CreateRatingPairedData(rat);

            CurveComponentVM curveComponentVM = new(StringConstants.STAGE_DISCHARGE, StringConstants.DISCHARGE, StringConstants.STAGE);
            curveComponentVM.SetPairedData(ratingPairedData);
            StageDischargeElement elem = new(rat.Name, rat.CalculationDate, description, curveComponentVM, elemID);
            return elem;
        }

        private static UncertainPairedData CreateRatingPairedData(RatingFunction rat)
        {
            //x-values are discharge
            //y-values are stage
            List<IDistribution> ys = new();
            switch (rat.ErrorTypesId)
            {
                case ErrorType.LOGNORMAL:
                    ys = CreateLogNormalDistributions(rat);
                    break;
                case ErrorType.NORMAL:
                    ys = CreateNormalDistributions(rat);
                    break;
                case ErrorType.TRIANGULAR:
                    ys = CreateTriangularDistributions(rat);
                    break;
                case ErrorType.UNIFORM:
                    ys = CreateUniformDistributions(rat);
                    break;
                case ErrorType.NONE:
                    ys = CreateDeterministicDistributions(rat);
                    break;
            }
            List<double> discharges = new();
            for (int i = 0; i < rat.NumberOfPoints; i++)
            {
                discharges.Add(rat.GetDischarge()[i]);
            }
            CurveMetaData curveMetaData = new("Stage", "Flow", "Rating", "");
            return new UncertainPairedData(discharges.ToArray(), ys.ToArray(), curveMetaData);
        }

        private static List<IDistribution> CreateLogNormalDistributions(RatingFunction rat)
        {
            if (rat.UsesGlobalError)
            {
                return CreateInterpolatedLogNormalStDevs(rat);
            }
            else
            {
                List<IDistribution> ys = new();
                for (int i = 0; i < rat.NumberOfPoints; i++)
                {
                    ys.Add(new LogNormal(rat.GetStage()[i], rat.IndividualLogStDevs[i]));
                }
                return ys;
            }
        }

        private static List<IDistribution> CreateNormalDistributions(RatingFunction rat)
        {
            if (rat.UsesGlobalError)
            {
                return CreateInterpolatedNormalStDevs(rat);
            }
            else
            {
                List<IDistribution> ys = new();
                for (int i = 0; i < rat.NumberOfPoints; i++)
                {
                    ys.Add(new Normal(rat.GetStage()[i], rat.IndividualStDevs[i]));
                }
                return ys;
            }
        }

        private static List<IDistribution> CreateInterpolatedNormalStDevs(RatingFunction rat)
        {
            List<IDistribution> ys = new();
            double globalStdDev = rat.GlobalStdDev;
            double baseStage = rat.BaseStage;
            double firstStage = rat.GetStage()[0];
            double ratio = globalStdDev / (baseStage - firstStage);
            double firstStDev = 0;
            int baseStageIndex = FindBaseStageIndex(baseStage, rat.GetStage());
            if (baseStageIndex != -1)
            {
                //add the first dist
                ys.Add(new Normal(firstStage, firstStDev));
                //add interpolated points
                for (int i = 1; i < baseStageIndex; i++)
                {
                    double stage = rat.GetStage()[i];
                    double stDev = ratio * (stage - firstStage);
                    ys.Add(new Normal(rat.GetStage()[i], stDev));
                }
                //add the constant st dev for base stage and up
                for (int i = baseStageIndex; i < rat.NumberOfPoints; i++)
                {
                    ys.Add(new Normal(rat.GetStage()[i], globalStdDev));
                }
            }
            return ys;
        }

        private static List<IDistribution> CreateInterpolatedLogNormalStDevs(RatingFunction rat)
        {
            List<IDistribution> ys = new();
            double globalStdDev = rat.GlobalStdDev;
            double baseStage = rat.BaseStage;
            double firstStage = rat.GetStage()[0];
            double ratio = globalStdDev / (baseStage - firstStage);
            double firstStDev = 0;
            int baseStageIndex = FindBaseStageIndex(baseStage, rat.GetStage());
            if (baseStageIndex != -1)
            {
                //add the first dist
                ys.Add(new LogNormal(firstStage, firstStDev));
                //add interpolated points
                for (int i = 1; i < baseStageIndex; i++)
                {
                    double stage = rat.GetStage()[i];
                    double stDev = ratio * (stage - firstStage);
                    ys.Add(new LogNormal(rat.GetStage()[i], stDev));
                }
                //add the constant st dev for base stage and up
                for (int i = baseStageIndex; i < rat.NumberOfPoints; i++)
                {
                    ys.Add(new LogNormal(rat.GetStage()[i], globalStdDev));
                }
            }
            return ys;
        }

        private static int FindBaseStageIndex(double baseStage, double[] stages)
        {
            double epsilon = .1;
            for (int i = 0; i < stages.Length; i++)
            {
                if (Math.Abs(stages[i] - baseStage) <= epsilon)
                {
                    return i;
                }
            }
            return -1;
        }

        private static List<IDistribution> CreateTriangularDistributions(RatingFunction rat)
        {

            List<IDistribution> ys = new();
            if (rat.NumberOfPoints > 0)
            {
                //add first point
                double firstStage = rat.GetStage()[0];
                ys.Add(new Triangular(firstStage, firstStage, firstStage));

                double baseStage = rat.BaseStage;
                double globalStdDevLow = rat.GlobalStdDevLow;
                double globalStdDevHigh = rat.GlobalStdDevHigh;

                double deltaMax = globalStdDevHigh - baseStage;
                double deltaMin = globalStdDevLow - baseStage;

                double minMostLikelyStageDelta = baseStage - globalStdDevLow;
                double maxMostLikelyStageDelta = globalStdDevHigh - baseStage;

                double stageDelta = baseStage - firstStage;

                for (int i = 1; i < rat.NumberOfPoints; i++)
                {
                    if (rat.UsesGlobalError)
                    {
                        double mostLikely = rat.GetStage()[i];
                        if (mostLikely < baseStage)
                        {
                            double minDifference = (minMostLikelyStageDelta / stageDelta) * (mostLikely - firstStage);
                            double minVal = mostLikely - minDifference;

                            double maxDifference = (maxMostLikelyStageDelta / stageDelta) * (mostLikely - firstStage);
                            double maxVal = mostLikely + maxDifference;

                            ys.Add(new Triangular(minVal, mostLikely, maxVal));
                        }
                        else
                        {
                            double min = mostLikely + deltaMin;
                            double max = mostLikely + deltaMax;
                            ys.Add(new Triangular(min, mostLikely, max));
                        }
                    }
                    else
                    {
                        ys.Add(new Triangular(rat.IndividualLowStDevs[i], rat.GetStage()[i], rat.IndividualHighStDevs[i]));
                    }
                }
            }
            return ys;
        }

        private static List<IDistribution> CreateUniformDistributions(RatingFunction rat)
        {
            List<IDistribution> ys = new();
            for (int i = 0; i < rat.NumberOfPoints; i++)
            {
                if (rat.UsesGlobalError)
                {
                    ys.Add(new Uniform(rat.GlobalStdDevLow, rat.GlobalStdDevHigh));
                }
                else
                {
                    ys.Add(new Uniform(rat.IndividualLowStDevs[i], rat.IndividualHighStDevs[i]));
                }
            }
            return ys;
        }
        private static List<IDistribution> CreateDeterministicDistributions(RatingFunction rat)
        {
            List<IDistribution> ys = new();
            for (int i = 0; i < rat.NumberOfPoints; i++)
            {
                ys.Add(new Deterministic(rat.GetStage()[i]));
            }
            return ys;
        }
        #endregion

        #region Stage Damage

        public static List<AggregatedStageDamageElement> ImportStageDamages(AggregateDamageFunctionList aggDamageList, List<ImpactAreaElement> impactAreaElements, ref String messages)
        {
            List<AggregatedStageDamageElement> Elements = new();

            //get the curves from the importer
            IList<AggregateDamageFunction> curves = aggDamageList.GetAggDamageFunctions.Values;

            //sort the curves by their plan and year.
            List<List<AggregateDamageFunction>> groupedCurves = curves.GroupBy(curve => new { curve.PlanName, curve.YearName })
                .Select(group => group.ToList())
                .ToList();

            messages += "\nGrouping stage damage functions by plan and year: \n";

            int id = PersistenceFactory.GetElementManager<AggregatedStageDamageElement>().GetNextAvailableId();
            int i = 0;
            //now create elements from the groups of curves
            foreach (List<AggregateDamageFunction> funcs in groupedCurves)
            {
                int elemID = id + i;
                AggregatedStageDamageElement stageDamElem = CreateElement(funcs, impactAreaElements, elemID, ref messages);
                if (stageDamElem != null)
                {
                    Elements.Add(stageDamElem);
                    i++;
                }
            }

            return Elements;
        }

        private static List<StageDamageCurve> CreateDamageCurves(AggregateDamageFunction function, 
            List<ImpactAreaElement> impactAreaElements, ref string messages)
        {
            List<StageDamageCurve> curves = new();

            SingleDamageFunction structDamageFunc = function.DamageFunctions[(int)StructureValueType.STRUCTURE];
            StageDamageCurve stageDamageCurve = CreateStageDamageCurve(structDamageFunc, "Structure", function.DamageReachName, function.CategoryName, impactAreaElements, ref messages);
            if (stageDamageCurve != null)
            {
                curves.Add(stageDamageCurve);
            }

            SingleDamageFunction contentDamageFunc = function.DamageFunctions[(int)StructureValueType.CONTENT];
            stageDamageCurve = CreateStageDamageCurve(contentDamageFunc, "Content", function.DamageReachName, function.CategoryName, impactAreaElements, ref messages);
            if (stageDamageCurve != null)
            {
                curves.Add(stageDamageCurve);
            }

            SingleDamageFunction otherDamageFunc = function.DamageFunctions[(int)StructureValueType.OTHER];
            stageDamageCurve = CreateStageDamageCurve(otherDamageFunc, "Other", function.DamageReachName, function.CategoryName, impactAreaElements, ref messages);
            if (stageDamageCurve != null)
            {
                curves.Add(stageDamageCurve);
            }

            SingleDamageFunction carDamageFunc = function.DamageFunctions[(int)StructureValueType.CAR];
            if(carDamageFunc.GetNumRows() >0)
            {
                stageDamageCurve = CreateStageDamageCurve(carDamageFunc, "Vehicle", function.DamageReachName, function.CategoryName, impactAreaElements, ref messages);
                if (stageDamageCurve != null)
                {
                    curves.Add(stageDamageCurve);
                }
            }


            return curves;
        }

        private static AggregatedStageDamageElement CreateElement(List<AggregateDamageFunction> funcs, List<ImpactAreaElement> impactAreaElements, int elemID, ref string messages)
        {
            AggregatedStageDamageElement elem = null;
            if (funcs.Count > 0)
            {
                string name = funcs[0].PlanName.Trim() + " - " + funcs[0].YearName.Trim();
                messages += "\nAttempting to create group: '" + name + "' from " + funcs.Count + " curves\n";
                //for the creation date, i am grabbing the creation date from one of the curves
                List<StageDamageCurve> curves = new();
                foreach (AggregateDamageFunction func in funcs)
                {
                    curves.AddRange(CreateDamageCurves(func, impactAreaElements,ref messages));
                }

                messages += "\nNumber of curves successfully created: " + curves.Count;

                if (curves.Count > 0)
                {
                    List<ImpactAreaFrequencyFunctionRowItem> impAreaFrequencyRows = new();
                    int analysisYear = DateTime.Now.Year;
                    elem = new AggregatedStageDamageElement(name, funcs[0].CalculationDate, funcs[0].Description,analysisYear, -1, -1, curves, impAreaFrequencyRows, true,false, elemID);
                }
                else
                {
                    messages += "\nUnable to create group.\n\n";
                }
            }
            return elem;
        }

        private static UncertainPairedData CreateStageDamagePairedData(SingleDamageFunction sdf)
        {
            double[] depths = sdf.Depth;
            double[] damages = sdf.Damage;
            double[] stDevs = sdf.StdDev;

            ErrorType uncertaintyType = sdf.GetTypeError();
            List<double> depthsList = new();
            List<IDistribution> damagesList = new();
            //stage damage curves can only be deterministic or normal.
            if (uncertaintyType == ErrorType.NONE)
            {
                //create deterministic
                for (int i = 0; i < sdf.GetNumRows(); i++)
                {
                    depthsList.Add(depths[i]);
                    damagesList.Add(new Deterministic(damages[i]));
                }
            }
            else if (uncertaintyType == ErrorType.NORMAL)
            {
                for (int i = 0; i < sdf.GetNumRows(); i++)
                {
                    depthsList.Add(depths[i]);
                    damagesList.Add(new Normal(damages[i], stDevs[i]));
                }
            }
            CurveMetaData curveMetaData = new("Stage", "Damage", "Stage-Damage", "");
            return new UncertainPairedData(depthsList.ToArray(), damagesList.ToArray(), curveMetaData);
        }

        private static StageDamageCurve CreateStageDamageCurve(SingleDamageFunction sdf, string assetCategory, string damageReachName, string damCat,
            List<ImpactAreaElement> impactAreaElements, ref string messages)
        {
            damageReachName = damageReachName.Trim();
            damCat = damCat.Trim();

            UncertainPairedData stageDamagePairedData = CreateStageDamagePairedData(sdf);

            StageDamageCurve curve = null;
            //there should only ever be 0 or 1 impact area elements
            if (impactAreaElements.Count > 0)
            {
                List<ImpactAreaRowItem> impactAreaRows = ((ImpactAreaElement)impactAreaElements[0]).ImpactAreaRows;

                bool nameMatchedImpactArea = false;
                //does this curve's damage reach equal an existing impact area?
                foreach (ImpactAreaRowItem row in impactAreaRows)
                {
                    //the damage reach name needs to match an existing impact area to be included.
                    //message user if it does not.
                    if (row.Name.Equals(damageReachName))
                    {
                        CurveComponentVM vm = new(StringConstants.STAGE_DAMAGE, StringConstants.STAGE, StringConstants.DAMAGE);
                        vm.SetPairedData(stageDamagePairedData);
                        curve = new StageDamageCurve(row, damCat, vm, assetCategory, StageDamageConstructionType.COMPUTED);
                        nameMatchedImpactArea = true;
                        break;
                    }
                }
                if (!nameMatchedImpactArea)
                {
                    messages += Environment.NewLine + "The stage damage curve with damage reach of '" + damageReachName + "' could not be imported because it does not match any existing impact area names.";
                }
            }

            return curve;
        }

        #endregion

        #region LP3
        public static List<FrequencyElement> CreateFlowFrequencyElements(ProbabilityFunctionList probFuncs)
        {
            List<FrequencyElement> elems = new();
            int id = PersistenceFactory.GetElementManager<FrequencyElement>().GetNextAvailableId();
            int i = 0;
            foreach (KeyValuePair<string, ProbabilityFunction> kvp in probFuncs.ProbabilityFunctions)
            {
                int elemID = id + i;
                ProbabilityFunction pf = kvp.Value;
                FrequencyFunctionType typeID = pf.ProbabilityFunctionTypeId;
                if (typeID == FrequencyFunctionType.ANALYTICAL || typeID == FrequencyFunctionType.GRAPHICAL)
                {
                    FrequencyElement freqElem = CreateFrequencyElement(pf, elemID);
                    if (freqElem != null)
                    {
                        elems.Add(freqElem);
                        i++;
                    }
                }
            }
            return elems;
        }



        private static FrequencyElement CreateManualAnalyticalElement(ProbabilityFunction pf, int elemID)
        {
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            double mean = pf.MomentsLp3[0];
            double stDev = pf.MomentsLp3[1];
            double skew = pf.MomentsLp3[2];

            int por = pf.EquivalentLengthOfRecord;

            bool isAnalytical = true;

            FrequencyEditorVM vm = new()
            {
                IsGraphical = !isAnalytical
            };
            vm.ParameterEntryVM.LP3Distribution = new LogPearson3(mean,stDev, skew, por);

            return new FrequencyElement(pf.Name, editDate, CreatePYSRDescription(pf), elemID,vm);

        }

        private static FrequencyElement CreateFrequencyElement(ProbabilityFunction pf, int elemID)
        {
            CurveComponentVM curveComponentVM = new(StringConstants.ANALYTICAL_FREQUENCY, StringConstants.EXCEEDANCE_PROBABILITY, StringConstants.DISCHARGE);
            FrequencyElement elem = null;
            if (pf.ProbabilityFunctionTypeId == FrequencyFunctionType.ANALYTICAL)
            {
                if (pf.SourceOfStatisticsId == SourceOfStatistics.ENTERED)
                {
                    elem = CreateManualAnalyticalElement(pf, elemID);
                }
            }
            else if (pf.ProbabilityFunctionTypeId == FrequencyFunctionType.GRAPHICAL)
            {
                FrequencyEditorVM vm = new()
                {
                    IsGraphical = true
                };
                vm.MyGraphicalVM.LoadFromProbabilityFunction(pf);
                elem = new FrequencyElement(pf.Name, DateTime.Now.ToString(), pf.Description, elemID,vm);
            }
            return elem;
        }



        #endregion

        #region inflow outflow

        public static List<InflowOutflowElement> CreateInflowOutflowElements(ProbabilityFunctionList probFuncs)
        {
            List<InflowOutflowElement> elems = new();
            int id = Saving.PersistenceFactory.GetElementManager<InflowOutflowElement>().GetNextAvailableId();
            int i = 0;
            foreach (KeyValuePair<string, ProbabilityFunction> kvp in probFuncs.ProbabilityFunctions)
            {
                ProbabilityFunction pf = kvp.Value;
                if (pf.NumberOfTransFlowPoints > 0)
                {
                    int elemID = id + i;
                    InflowOutflowElement elem = CreateInflowOutflow(pf, elemID);
                    if (elem != null)
                    {
                        elems.Add(elem);
                        i++;
                    }
                }
            }
            return elems;
        }

        private static InflowOutflowElement CreateInflowOutflow(ProbabilityFunction probFunction, int elemID)
        {
            List<IDistribution> distributedOrdinates = GetUncertaintyValues(probFunction);
            CurveMetaData metaData = new("Inflow", "Outflow", "Inflow-Outflow", "");
            UncertainPairedData func = new(probFunction.TransFlowInflow, distributedOrdinates.ToArray(), metaData);
            CurveComponentVM curveComponentVM = new(StringConstants.REGULATED_UNREGULATED, StringConstants.UNREGULATED, StringConstants.REGULATED);
            curveComponentVM.SetPairedData(func);
            return new InflowOutflowElement(probFunction.Name, probFunction.CalculationDate, CreatePYSRDescription(probFunction), curveComponentVM, elemID);
        }

        private static List<IDistribution> GetUncertaintyValues(ProbabilityFunction probFunction)
        {
            List<IDistribution> ords = new();
            if (probFunction.ErrorTypeTransformFlow == ErrorType.NORMAL)
            {
                for (int i = 0; i < probFunction.NumberOfTransFlowPoints; i++)
                {
                    ords.Add(new Normal(probFunction.TransFlowOutflow[i], probFunction.TransFlowStdDev[i]));
                }
            }
            else if (probFunction.ErrorTypeTransformFlow == ErrorType.LOGNORMAL)
            {
                for (int i = 0; i < probFunction.NumberOfTransFlowPoints; i++)
                {
                    ords.Add(new LogNormal(probFunction.TransFlowOutflow[i], probFunction.TransFlowStdDev[i]));
                }
            }
            else if (probFunction.ErrorTypeTransformFlow == ErrorType.TRIANGULAR)
            {
                for (int i = 0; i < probFunction.NumberOfTransFlowPoints; i++)
                {
                    ords.Add(new Triangular(probFunction.TransFlowLower[i], probFunction.TransFlowOutflow[i], probFunction.TransFlowUpper[i]));
                }
            }
            else if (probFunction.ErrorTypeTransformFlow == ErrorType.UNIFORM)
            {
                for (int i = 0; i < probFunction.NumberOfTransFlowPoints; i++)
                {
                    ords.Add(new Uniform(probFunction.TransFlowLower[i], probFunction.TransFlowUpper[i]));
                }
            }
            else if (probFunction.ErrorTypeTransformFlow == ErrorType.NONE)
            {
                for (int i = 0; i < probFunction.NumberOfTransFlowPoints; i++)
                {
                    ords.Add(new Deterministic(probFunction.TransFlowOutflow[i]));
                }
            }

            return ords;
        }

        #endregion

        #region Occtypes

        public static ChildElement CreateOcctypes(OccupancyTypeList ots, string groupName, ref string messages)
        {
            int groupID = PersistenceFactory.GetElementManager<OccupancyTypesElement>().GetNextAvailableId();
            return CreateOcctypes(ots, groupName, ref messages, groupID);
        }

        public static ChildElement CreateOcctypes(OccupancyTypeList ots, string groupName, ref string messages, int groupID)
        {
            List<OccupancyType> fda2Occtypes = new();
            int occtypeID = 1;
            foreach (Importer.OccupancyType ot in ots.Occtypes)
            {
                try
                {
                    fda2Occtypes.Add(GetFDA2OccupancyType(ot, groupID, occtypeID));
                    occtypeID++;
                }
                catch (Exception e)
                {
                    string errorMsg = Environment.NewLine + "Failed to import occupancy type '" + ot.Name + "' because of the following exception:";
                    messages += errorMsg + Environment.NewLine + e.Message + Environment.NewLine;
                }
            }

            string lastEditDate = DateTime.Now.ToString("G");
            OccupancyTypesElement elem = new(groupName, lastEditDate, "", fda2Occtypes, groupID);
            return elem;
        }

        private static OccupancyType GetFDA2OccupancyType(Importer.OccupancyType importedOT, int groupID, int ID)
        {
            List<string> errorMessages = new();

            //the single damage functions will always be in this order
            //public enum StructureValueType { STRUCTURE, CONTENT, OTHER, CAR, TOTAL };
            //this list is in the order of the enum
            List<UncertainPairedData> coordFunctions = TranslateSingleDamageFunctionToCoordinatesFunctions(importedOT, errorMessages);
            UncertainPairedData StructureDepthDamageFunction = coordFunctions[(int)StructureValueType.STRUCTURE];
            UncertainPairedData ContentDepthDamageFunction = coordFunctions[(int)StructureValueType.CONTENT];
            UncertainPairedData VehicleDepthDamageFunction = coordFunctions[(int)StructureValueType.CAR];
            UncertainPairedData OtherDepthDamageFunction = coordFunctions[(int)StructureValueType.OTHER];

            bool CalculateStructureDamage = !IsEmptyFunction(importedOT._SingleDamageFunction[(int)StructureValueType.STRUCTURE]);
            bool CalculateContentDamage = !IsEmptyFunction(importedOT._SingleDamageFunction[(int)StructureValueType.CONTENT]); ;
            bool CalculateVehicleDamage = !IsEmptyFunction(importedOT._SingleDamageFunction[(int)StructureValueType.CAR]); ;
            bool CalculateOtherDamage = !IsEmptyFunction(importedOT._SingleDamageFunction[(int)StructureValueType.OTHER]); ;

            CurveComponentVM structureComponent = new(StringConstants.OCCTYPE_PLOT_TITLE, StringConstants.OCCTYPE_DEPTH, StringConstants.OCCTYPE_PERCENT_DAMAGE, isDepthPercentDamage: true);
            structureComponent.SetPairedData(StructureDepthDamageFunction);

            CurveComponentVM contentComponent = new(StringConstants.OCCTYPE_PLOT_TITLE, StringConstants.OCCTYPE_DEPTH, StringConstants.OCCTYPE_PERCENT_DAMAGE, isDepthPercentDamage: true);
            contentComponent.SetPairedData(ContentDepthDamageFunction);

            CurveComponentVM vehicleComponent = new(StringConstants.OCCTYPE_PLOT_TITLE, StringConstants.OCCTYPE_DEPTH, StringConstants.OCCTYPE_PERCENT_DAMAGE, isDepthPercentDamage: true);
            vehicleComponent.SetPairedData(VehicleDepthDamageFunction);

            CurveComponentVM otherComponent = new(StringConstants.OCCTYPE_PLOT_TITLE, StringConstants.OCCTYPE_DEPTH, StringConstants.OCCTYPE_PERCENT_DAMAGE, isDepthPercentDamage: true);
            otherComponent.SetPairedData(OtherDepthDamageFunction);

            List<ContinuousDistribution> uncertainties = TranslateErrorDistributionsToIOrdinates(importedOT._ErrorDistribution);
            ContinuousDistribution foundationHeightUncertainty = uncertainties[(int)OccTypeStrucComponent.FFLOOR];
            ContinuousDistribution structureValueUncertainty = uncertainties[(int)OccTypeStrucComponent.STRUCTURE];
            ContinuousDistribution vehicleValueUncertainty = uncertainties[(int)OccTypeStrucComponent.AUTO];

            //the content and other value uncertainties can either be "by value" or "ratio to structures". 
            //if the value is null, then it is "By value". If there is a value in the array then it is "by ratio".
            //The ratio uncertainties also need to be converted to FDA 2.0 standard. 
            ContinuousDistribution contentValueUncertaintyByRatio = uncertainties[(int)OccTypeStrucComponent.CONTENT];
            ContinuousDistribution otherValueUncertaintyByRatio = uncertainties[(int)OccTypeStrucComponent.OTHER];
            if (contentValueUncertaintyByRatio != null)
            {
                contentValueUncertaintyByRatio = ConvertRatioValueToFDA2(contentValueUncertaintyByRatio);
            }
            if (otherValueUncertaintyByRatio != null)
            {
                otherValueUncertaintyByRatio = ConvertRatioValueToFDA2(otherValueUncertaintyByRatio);
            }

            bool isContentByValue = contentValueUncertaintyByRatio == null; //this is shitty. needs to evaluate to false. needs to check for -901
            bool isOtherByValue = otherValueUncertaintyByRatio == null;

            ContinuousDistribution contentValueUncertaintyByValue = new Deterministic();
            ContinuousDistribution otherValueUncertaintyByValue = new Deterministic();

            OccTypeAsset StructureItem = new(OcctypeAssetType.structure, CalculateStructureDamage, structureComponent, structureValueUncertainty);
            OccTypeAssetWithRatio ContentItem = new(OcctypeAssetType.content, CalculateContentDamage, contentComponent, contentValueUncertaintyByValue, contentValueUncertaintyByRatio, isContentByValue);
            OccTypeAsset VehicleItem = new(OcctypeAssetType.vehicle, CalculateVehicleDamage, vehicleComponent, vehicleValueUncertainty);
            OccTypeAssetWithRatio OtherItem = new(OcctypeAssetType.other, CalculateOtherDamage, otherComponent, otherValueUncertaintyByValue, otherValueUncertaintyByRatio, isOtherByValue);

            ContinuousDistribution FoundationHeightUncertainty = foundationHeightUncertainty;

            OccupancyType ot = new(importedOT.Name, importedOT.Description, importedOT.CategoryName,
                StructureItem, ContentItem, VehicleItem, OtherItem, FoundationHeightUncertainty, ID);
            return ot;
        }

        /// <summary>
        /// 1.4.x specified distributions with coefficient of variance instead of standard deviation, and specified min and max for triangular as percentages of the mean. 
        /// This method converts that to the 2.0 standard. 
        /// </summary>
        private static ContinuousDistribution ConvertRatioValueToFDA2(ContinuousDistribution dist)
        {
            ContinuousDistribution retval = dist;
            switch (dist.Type)
            {
                case IDistributionEnum.Normal:
                    Normal normalDist = dist as Normal;
                    double mean = normalDist.Mean;
                    double stDev = normalDist.StandardDeviation;
                    //divide stDev by 100 to convert to decimal percentage
                    double newStDev = mean * stDev / 100;
                    retval = new Normal(mean, newStDev);
                    break;
                case IDistributionEnum.LogNormal:
                    LogNormal logNormalDist = dist as LogNormal;
                    double logMean = logNormalDist.Mean;
                    double logStDev = logNormalDist.StandardDeviation;
                    //divide stDev by 100 to convert to decimal percentage
                    double newLogStDev = logMean * logStDev / 100;
                    retval = new LogNormal(logMean, newLogStDev);
                    break;
                case IDistributionEnum.Triangular:
                    Triangular triDist = dist as Triangular;
                    double mostLikely = triDist.MostLikely;
                    double min = triDist.Min;
                    double max = triDist.Max;
                    double newMin = mostLikely * min / 100;
                    double newMax = mostLikely * max / 100;
                    retval = new Triangular(newMin, mostLikely, newMax);                  
                    break;
            }
            return retval;
        }

        /// <summary>
        /// This translates error distributions for assets whos' central tendency is described in the structure inventory. Most likely value is set to 100, which indicates 100% of the inventory value. 
        /// </summary>
        private static ContinuousDistribution TranslateStructureValueUncertainty(ErrorDistribution errorDist)
        {
            //It looks like the only options that will actually come in here is Normal, Triangular, Log Normal.
            double mostLikelyValue = 100;
            //st dev gets reused as min
            double stDev = errorDist.StandardDeviationOrMin;
            double max = errorDist.Maximum;
            ErrorType type = errorDist.ErrorType;
            ContinuousDistribution dist = new Deterministic(mostLikelyValue);
            switch (type)
            {
                case ErrorType.NONE:
                    dist = new Deterministic(mostLikelyValue);
                    break;
                case ErrorType.NORMAL:
                    dist = new Normal(mostLikelyValue, stDev);
                    break;
                case ErrorType.TRIANGULAR:
                    dist = new Triangular(stDev, mostLikelyValue, max);
                    break;
                case ErrorType.UNIFORM:
                    dist = new Uniform(mostLikelyValue, max);
                    break;
                case ErrorType.LOGNORMAL:
                    dist = new LogNormal(mostLikelyValue, stDev);
                    break;
            }
            return dist;
        }

        private static ContinuousDistribution TranslateRatioValueUncertainty(ErrorDistribution errorDist)
        {
            //It looks like the only options that will actually come in here is Normal, Triangular, Log Normal.
            double mean = errorDist.CentralValue;
            //st dev gets reused as min
            double stDevOrMin = errorDist.StandardDeviationOrMin;
            double max = errorDist.Maximum;
            ErrorType type = errorDist.ErrorType;
            ContinuousDistribution dist = new Deterministic(mean);
            switch (type)
            {
                case ErrorType.NONE:
                    dist = new Deterministic(mean);
                    break;
                    //ErrorDistribution actually specifies the Coefficient of Variance, so this is a dirty object that will need to be processed later.  
                case ErrorType.NORMAL:
                    dist = new Normal(mean, stDevOrMin);
                    break;
                    //Error Distribution specifies min and max as percentages of the mean, so this is another dirty object. 
                case ErrorType.TRIANGULAR:
                    dist = new Triangular(stDevOrMin, mean, max);
                    break;
                case ErrorType.UNIFORM:
                    dist = new Uniform(mean, max);
                    break;
                //ErrorDistribution actually specifies the Coefficient of Variance, so this is a dirty object that will need to be processed later.  
                case ErrorType.LOGNORMAL:
                    dist = new LogNormal(mean, stDevOrMin);
                    break;
            }
            return dist;
        }

        private static ContinuousDistribution TranslateErrorDistToOrdinate(ErrorDistribution errorDist, OccTypeStrucComponent componentType)
        {
            ContinuousDistribution dist = null;
            if (errorDist != null)
            {
                switch (componentType)
                {
                    case OccTypeStrucComponent.FFLOOR:
                    case OccTypeStrucComponent.STRUCTURE:
                        dist = TranslateStructureValueUncertainty(errorDist);
                        break;
                    case OccTypeStrucComponent.CONTENT:
                    case OccTypeStrucComponent.AUTO:
                    case OccTypeStrucComponent.OTHER:
                        dist = TranslateRatioValueUncertainty(errorDist);
                        break;
                }
            }
            return dist;
        }
        private static List<ContinuousDistribution> TranslateErrorDistributionsToIOrdinates(ErrorDistribution[] errorDists)
        {
            List<ContinuousDistribution> ordinates = new();
            if (errorDists.Length >= 5)
            {
                ordinates.Add(TranslateErrorDistToOrdinate(errorDists[0], OccTypeStrucComponent.FFLOOR));
                ordinates.Add(TranslateErrorDistToOrdinate(errorDists[1], OccTypeStrucComponent.STRUCTURE));
                ordinates.Add(TranslateErrorDistToOrdinate(errorDists[2], OccTypeStrucComponent.CONTENT));
                ordinates.Add(TranslateErrorDistToOrdinate(errorDists[3], OccTypeStrucComponent.OTHER));
                ordinates.Add(TranslateErrorDistToOrdinate(errorDists[4], OccTypeStrucComponent.AUTO));
            }
            return ordinates;
        }

        private static List<UncertainPairedData> TranslateSingleDamageFunctionToCoordinatesFunctions(Importer.OccupancyType ot, List<string> errorMessages)
        {
            SingleDamageFunction[] singleDamageFunctions = ot._SingleDamageFunction;
            //the single damage functions will always be in this order
            //public enum StructureValueType { STRUCTURE, CONTENT, OTHER, CAR, TOTAL };
            List<UncertainPairedData> coordinatesFunctions = new();
            //if an occtype fails to read in properly we will make a message and keep trying
            //to import other occtypes.
            for (int i = 0; i < singleDamageFunctions.Length; i++)
            {
                SingleDamageFunction func = singleDamageFunctions[i];
                StructureValueType type = (StructureValueType)i;
                UncertainPairedData function;

                if (IsEmptyFunction(func) || ot.UsesDollar)
                {
                    //create a function with just (0,0)
                    function = CreateEmptyFunction();
                }
                else
                {
                    function = CreateCoordinatesFunction(ot.Name, func, type, errorMessages);
                }

                //the coordinates function will be null if it was not able to be created
                //create an empty coord function?
                function ??= CreateEmptyFunction();

                coordinatesFunctions.Add(function);
            }
            return coordinatesFunctions;
        }

        private static UncertainPairedData CreateEmptyFunction()
        {
            List<double> xs = new() { 0 };
            List<Deterministic> ys = new() { new Deterministic(0) };
            CurveMetaData metaData = new("Stage", "Damage", "Stage Damage", "");
            return new UncertainPairedData(xs.ToArray(), ys.ToArray(), metaData);
        }

        private static bool IsEmptyFunction(SingleDamageFunction function)
        {
            return function.NumOrdinates == 0;
        }

        private static UncertainPairedData CreateCoordinatesFunction(string name, SingleDamageFunction function, StructureValueType structureValueType, List<string> errors)
        {
            List<double> depths = function.Depth.ToList<double>();
            List<double> damages = function.Damage.ToList<double>();
            //stDevs get reused as min values if the type is triangular
            List<double> stDevs = function.StdDev.ToList<double>();
            List<double> maxValues = function.ErrHi.ToList<double>();
            switch (function.GetTypeError())
            {
                case ErrorType.NONE:
                    {
                        return CreateNoneFunction(name, depths, damages, structureValueType, errors);
                    }
                case ErrorType.NORMAL:
                    {
                        //search for constant values up front (ie: 0,0)
                        FindConstantValuesForNormal(depths, damages, stDevs);
                        return CreateNormalFunction(name, depths, damages, stDevs, structureValueType, errors);
                    }
                case ErrorType.TRIANGULAR:
                    {
                        return CreateTriangularFunction(name, depths, damages, stDevs, maxValues, structureValueType, errors);
                    }
                case ErrorType.UNIFORM:
                    {
                        return CreateUniformFunction(name, depths, stDevs, maxValues, structureValueType, errors);
                    }
                case ErrorType.LOGNORMAL:
                    {
                        throw new NotImplementedException("");
                    }
                default:
                    {
                        errors.Add("Could not create a '" + structureValueType + "' coordinates function for the occupancy type: " + name + " because '" + structureValueType + "' is" +
                            " not defined in FDA.");
                        return null;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="structureValueType">This is just to add more info to the error msg if there is one</param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static UncertainPairedData CreateNoneFunction(string name, List<double> xs, List<double> ys, StructureValueType structureValueType, List<string> errors)
        {
            try
            {
                return UncertainPairedDataFactory.CreateDeterminateData(xs, ys, "Stage", "Damage", "Occupancy Type");
            }
            catch (ArgumentException e)
            {
                errors.Add(CreateFailedCoordFunctionErrorMsg(structureValueType, name, e.Message));
                return null;
            }
        }

        private static UncertainPairedData CreateNormalFunction(string name, List<double> xs, List<double> ys, List<double> stDevs, StructureValueType structureValueType, List<string> errors)
        {
            List<Normal> yVals = new();
            for (int i = 0; i < xs.Count; i++)
            {
                yVals.Add(new Normal(ys[i], stDevs[i]));
            }

            try
            {
                CurveMetaData metaData = new("Stage", "Damage", "Occupancy Type", "");
                return new UncertainPairedData(xs.ToArray(), yVals.ToArray(), metaData);
            }
            catch (ArgumentException e)
            {
                errors.Add(CreateFailedCoordFunctionErrorMsg(structureValueType, name, e.Message));
                return null;
            }
        }

        private static UncertainPairedData CreateTriangularFunction(string name, List<double> xs, List<double> ys, List<double> mins, List<double> maxs, StructureValueType structureValueType, List<string> errors)
        {
            List<Triangular> yVals = new();
            for (int i = 0; i < xs.Count; i++)
            {
                yVals.Add(new Triangular(mins[i], ys[i], maxs[i]));
            }

            try
            {
                CurveMetaData metaData = new("Stage", "Damage", "Occupancy Type", "");
                return new UncertainPairedData(xs.ToArray(), yVals.ToArray(), metaData);
            }
            catch (ArgumentException e)
            {
                errors.Add(CreateFailedCoordFunctionErrorMsg(structureValueType, name, e.Message));
                return null;
            }
        }

        private static UncertainPairedData CreateUniformFunction(string name, List<double> xs, List<double> mins, List<double> maxs, StructureValueType structureValueType, List<string> errors)
        {
            List<Uniform> yVals = new();
            for (int i = 0; i < xs.Count; i++)
            {
                yVals.Add(new Uniform(mins[i], maxs[i]));
            }

            try
            {
                CurveMetaData metaData = new("Stage", "Damage", "Occupancy Type", "");
                return new UncertainPairedData(xs.ToArray(), yVals.ToArray(), metaData);
            }
            catch (ArgumentException e)
            {
                errors.Add(CreateFailedCoordFunctionErrorMsg(structureValueType, name, e.Message));
                return null;
            }
        }

        private static string CreateFailedCoordFunctionErrorMsg(StructureValueType type, string occtypeName, string exceptionMsg)
        {
            return "Could not create a '" + type + "' coordinates function for the occupancy type: " + occtypeName + ". " + exceptionMsg;
        }

        private static void FindConstantValuesForNormal(List<double> xs, List<double> ys, List<double> stDevs)
        {
            List<List<int>> masterListOfConstantIndexes = new();
            List<int> indexOfConstants = GetIndexesWithConstantValuesForNormal(ys, stDevs);

            //so now we have all the indexes of constant values.
            //if there are any then we know we need to do a linked function
            //add the first point to a list
            List<int> currentConstantFunction = new()
            {
                indexOfConstants[0]
            };
            for (int i = 0; i < indexOfConstants.Count - 1; i++)
            {
                int currentIndex = indexOfConstants[i];
                int nextIndex = indexOfConstants[i + 1];
                if (currentIndex + 1 == nextIndex)
                {
                    //then they are next to each other and should be added to the same function\
                    currentConstantFunction.Add(nextIndex);
                }
                else
                {
                    //these two indexes are not next to each other
                    //add the current list to master and start a new one
                    masterListOfConstantIndexes.Add(currentConstantFunction);
                    currentConstantFunction = new List<int>
                    {
                        nextIndex
                    };
                }
            }
            //the factory that makes the linked function will sort the functions by the min x value of each function
            //i don't need them in the correct order.
            //flush whatever is in the current list
            masterListOfConstantIndexes.Add(currentConstantFunction);
        }

        /// <summary>
        /// Gets all the indexes from the lists that have constant values. (ie: y = stdev)
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="stDevs"></param>
        /// <returns></returns>
        private static List<int> GetIndexesWithConstantValuesForNormal(List<double> ys, List<double> stDevs)
        {
            List<int> indexOfConstants = new();
            for (int i = 0; i < ys.Count; i++)
            {
                if (ys[i] == stDevs[i])
                {
                    indexOfConstants.Add(i);
                }
            }
            return indexOfConstants;
        }

        #endregion

        #region Failure Function / levee

        public static List<ChildElement> CreateLeveeElements(LeveeList leveeList, ref string message)
        {
            List<ChildElement> elems = new();
            int id = PersistenceFactory.GetElementManager<LateralStructureElement>().GetNextAvailableId();
            int i = 0;
            foreach (KeyValuePair<string, Levee> kvp in leveeList.Levees)
            {
                int elemID = id + i;
                Levee lev = kvp.Value;
                elems.Add(CreateLeveeElement(lev, elemID, ref message));
                i++;
            }
            return elems;
        }


        private static ChildElement CreateLeveeElement(Levee lev, int elemID, ref string message)
        {
            List<double> xs = new();
            List<double> ys = new();

            foreach (Pair_xy xy in lev.FailureFunctionPairs)
            {
                xs.Add(xy.GetX());
                ys.Add(xy.GetY());
            }

            bool isDefault = true;
            UncertainPairedData func = new();
            if (xs.Count == 0)
            {
                //create default curve
                List<double> defaultXs = new() { lev.ElevationTopOfLevee, lev.ElevationTopOfLevee + .000000000000001 };
                List<Deterministic> defaultYs = new() { new Deterministic(0), new Deterministic(1) };

                CurveMetaData cm = new("Elevation", "Probability", "Failure Function", "");
                func = new UncertainPairedData(defaultXs.ToArray(), defaultYs.ToArray(), cm);
                message += "No failure function was detected.\nCreating default failure function at top of levee.";
            }
            else
            {
                List<Deterministic> yVals = new();
                foreach (double d in ys)
                {
                    yVals.Add(new Deterministic(d));
                }
                CurveMetaData cm = new("Elevation", "Probability", "Failure Function", "");
                func = new UncertainPairedData(xs.ToArray(), yVals.ToArray(), cm);
                isDefault = false;
            }
            CurveComponentVM curveComponentVM = new(StringConstants.SYSTEM_RESPONSE_CURVE, StringConstants.STAGE, StringConstants.FAILURE_FREQUENCY);
            curveComponentVM.SetPairedData(func);
            LateralStructureElement leveeFeatureElement = new(lev.Name, lev.CalculationDate, CreatePYSRDescription(lev), lev.ElevationTopOfLevee, isDefault, curveComponentVM, elemID);
            return leveeFeatureElement;
        }

        #endregion

        #region Exterior Interior
        public static List<ChildElement> CreateExteriorInteriors(LeveeList leveeList)
        {
            List<ChildElement> elems = new();
            int id = PersistenceFactory.GetElementManager<ExteriorInteriorElement>().GetNextAvailableId();
            int i = 0;
            foreach (KeyValuePair<string, Levee> kvp in leveeList.Levees)
            {
                int elemID = id + i;
                Levee lev = kvp.Value;
                if (lev.ExteriorInteriorPairs.Count > 0)
                {
                    elems.Add(CreateExteriorInterior(lev, elemID));
                    i++;
                }
            }
            return elems;
        }

        private static ChildElement CreateExteriorInterior(Levee lev, int elemID)
        {
            List<double> xs = new();
            List<Deterministic> ys = new();
            foreach (Pair_xy xy in lev.ExteriorInteriorPairs)
            {
                xs.Add(xy.GetX());
                ys.Add(new Deterministic(xy.GetY()));
            }
            CurveMetaData cm = new("Exterior Stage", "Interior Stage", "Exterior-Interior", "");
            UncertainPairedData func = new(xs.ToArray(), ys.ToArray(), cm);

            CurveComponentVM curveComponentVM = new(StringConstants.EXT_INT, StringConstants.EXT_STAGE, StringConstants.INT_STAGE);
            curveComponentVM.SetPairedData(func);
            ExteriorInteriorElement elem = new(lev.Name, lev.CalculationDate, CreatePYSRDescription(lev), curveComponentVM, elemID);
            return elem;
        }
        #endregion

        #region PYSR Description
        private static string CreatePYSRDescription(ProbabilityFunction pf)
        {
            return CreatePYSRDescription(pf.PlanName, pf.YearName, pf.StreamName, pf.DamageReachName, pf.Description);
        }

        private static string CreatePYSRDescription(Levee lev)
        {
            return CreatePYSRDescription(lev.PlanName, lev.YearName, lev.StreamName, lev.DamageReachName, lev.Description);
        }

        private static string CreatePYSRDescription(RatingFunction rat)
        {
            return CreatePYSRDescription(rat.PlanName, rat.YearName, rat.StreamName, rat.DamageReachName, rat.Description);
        }

        private static string CreatePYSRDescription(string plan, string year, string stream, string reach, string description)
        {
            string pysr = description + " (Retrieved from: " + plan.Trim() + ", " + year.Trim() + ", " + stream.Trim() + ", " + reach.Trim() + ")";
            return pysr;
        }

        #endregion
    }
}
