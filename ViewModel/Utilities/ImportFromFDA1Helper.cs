using Importer;
using paireddata;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ViewModel.AggregatedStageDamage;
using ViewModel.FlowTransforms;
using ViewModel.FrequencyRelationships;
using ViewModel.GeoTech;
using ViewModel.ImpactArea;
using ViewModel.Inventory.DamageCategory;
using ViewModel.Inventory.OccupancyTypes;
using ViewModel.StageTransforms;
using static Importer.ProbabilityFunction;

namespace ViewModel.Utilities
{
    public static class ImportFromFDA1Helper
    {

        #region Rating

        public static List<RatingCurveElement> CreateRatingElements(RatingFunctionList ratings)
        {
            List<RatingCurveElement> elems = new List<RatingCurveElement>();
            foreach (KeyValuePair<string, RatingFunction> rat in ratings.RatingFunctions)
            {
                RatingCurveElement elem = CreateRatingElement(rat.Value);
                if (elem != null)
                {
                    elems.Add(elem);
                }
            }
            return elems;
        }
        private static RatingCurveElement CreateRatingElement(RatingFunction rat)
        {
            string pysr = "(" + rat.PlanName.Trim() + " " + rat.YearName.Trim() + " " + rat.StreamName.Trim() + " " + rat.DamageReachName.Trim() + ") ";
            string description = pysr + rat.Description;
            double[] stages = rat.GetStage();
            double[] flows = rat.GetDischarge();
            //these arrays might have a bunch of "Study.badNumber" (-901). I need to get rid of them by only grabbing the correct number of points.
            List<double> stagesList = new List<double>();
            List<double> flowsList = new List<double>();
            for (int i = 0; i < rat.NumberOfPoints; i++)
            {
                stagesList.Add(stages[i]);
                flowsList.Add(flows[i]);
            }
            //todo: how do we handle uncertainty here?

            UncertainPairedData ratingPairedData = CreateRatingPairedData(rat);
            //always use linear. This is the only option in Old Fda.
            //ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(stagesList, flowsList, InterpolationEnum.Linear);
            //IFdaFunction rating = IFdaFunctionFactory.Factory(IParameterEnum.Rating, (IFunction)func);
            //add the plan year stream reach for the description
            RatingCurveElement elem = new RatingCurveElement(rat.Name, rat.CalculationDate, description, ratingPairedData);
            return elem;
        }

        private static UncertainPairedData CreateRatingPairedData(RatingFunction rat)
        {
            if (rat.UsesGlobalError)
            {
                return CreateRatingPairedDataWithGlobalUncertainty(rat);
            }
            else
            {
                //not sure if this is possible. Look at the importer project class RatingFunction. I could do it but i would have to make the arrays public.
                //if this is possible, maybe i could just set the global variables. I think the logic is the same.
                return CreateRatingPairedDataWithPointUncertainty(rat);
            }
        }

        private static UncertainPairedData CreateRatingPairedDataWithPointUncertainty(RatingFunction rat)
        {
            UncertainPairedData upd = new UncertainPairedData();
            //if (rat.ErrorTypesId == ErrorType.LOGNORMAL)
            //{
            //    rat._st
            //    List<Deterministic> yVals = new List<Deterministic>();
            //    foreach (double d in ys)
            //    {
            //        yVals.Add(new Deterministic(d));
            //    }
            //    UncertainPairedData curve = new UncertainPairedData(xs.ToArray(), yVals.ToArray(), xLabel, yLabel, name, "", -1);
            //    return curve;
            //}
            //else if (rat.ErrorTypesId == ErrorType.NORMAL)
            //{

            //}
            //else if (rat.ErrorTypesId == ErrorType.TRIANGULAR)
            //{

            //}
            //else if (rat.ErrorTypesId == ErrorType.UNIFORM)
            //{

            //}
            //else if (rat.ErrorTypesId == ErrorType.NONE)
            //{

            //}
            return upd;
        }

        private static UncertainPairedData CreateRatingPairedDataWithGlobalUncertainty(RatingFunction rat)
        {
            UncertainPairedData upd = new UncertainPairedData();
            string xLabel = "Stage";
            string yLabel = "Flow";
            string name = "Rating";

            double[] stages = rat.GetStage();
            double[] flows = rat.GetDischarge();

            if (rat.ErrorTypesId == ErrorType.LOGNORMAL)
            {
                List<IDistribution> ys = new List<IDistribution>();
                for (int i = 0; i < rat.NumberOfPoints; i++)
                {
                    ys.Add(new LogNormal(flows[i], rat.GlobalStdDev));
                }
                upd = new UncertainPairedData(stages, ys.ToArray(), xLabel, yLabel, name, "", -1);
            }
            else if (rat.ErrorTypesId == ErrorType.NORMAL)
            {
                List<IDistribution> ys = new List<IDistribution>();
                for (int i = 0; i < rat.NumberOfPoints; i++)
                {
                    ys.Add(new Normal(flows[i], rat.GlobalStdDev));
                }
                upd = new UncertainPairedData(stages, ys.ToArray(), xLabel, yLabel, name, "", -1);
            }
            else if (rat.ErrorTypesId == ErrorType.TRIANGULAR)
            {
                List<IDistribution> ys = new List<IDistribution>();
                for (int i = 0; i < rat.NumberOfPoints; i++)
                {
                    ys.Add(new Triangular(rat.GlobalStdDevLow, flows[i], rat.GlobalStdDevHigh));
                }
                upd = new UncertainPairedData(stages, ys.ToArray(), xLabel, yLabel, name, "", -1);
            }
            else if (rat.ErrorTypesId == ErrorType.UNIFORM)
            {
                List<IDistribution> ys = new List<IDistribution>();
                for (int i = 0; i < rat.NumberOfPoints; i++)
                {
                    ys.Add(new Uniform(rat.GlobalStdDevLow, rat.GlobalStdDevHigh));
                }
                upd = new UncertainPairedData(stages, ys.ToArray(), xLabel, yLabel, name, "", -1);
            }
            else if (rat.ErrorTypesId == ErrorType.NONE)
            {
                List<IDistribution> ys = new List<IDistribution>();
                for (int i = 0; i < rat.NumberOfPoints; i++)
                {
                    ys.Add(new Deterministic(flows[i]));
                }
                upd = new UncertainPairedData(stages, ys.ToArray(), xLabel, yLabel, name, "", -1);
            }
            return upd;
        }


        #endregion

        #region Stage Damage

        public static List<AggregatedStageDamageElement> ImportStageDamages(AggregateDamageFunctionList aggDamageList, List<ImpactAreaElement> impactAreaElements,ref String messages)
        {
            List<AggregatedStageDamageElement> Elements = new List<AggregatedStageDamageElement>();

            //get the curves from the importer
            IList<AggregateDamageFunction> curves = aggDamageList.GetAggDamageFunctions.Values;
            //sort the curves by their plan and year.
            List<List<AggregateDamageFunction>> groupedCurves = curves.GroupBy(curve => new { curve.PlanName, curve.YearName })
                .Select(group => group.ToList())
                .ToList();

            //todo: write out how we are grouping the curves?
            messages += "\nGrouping stage damage functions by plan and year: \n";

            //now create elements from the groups of curves
            foreach (List<AggregateDamageFunction> funcs in groupedCurves)
            {
                AggregatedStageDamageElement stageDamElem = CreateElement(funcs, impactAreaElements, ref messages);
                if (stageDamElem != null)
                {
                    Elements.Add(stageDamElem);
                }
            }

            return Elements;
        }

        private static AggregatedStageDamageElement CreateElement(List<AggregateDamageFunction> funcs, List<ImpactAreaElement> impactAreaElements, ref string messages)
        {
            //for the creation date, i am grabbing the creation date from one of the curves

            List<StageDamageCurve> curves = new List<StageDamageCurve>();
            foreach (AggregateDamageFunction func in funcs)
            {

                SingleDamageFunction totalDamageFunc = func.DamageFunctions[(int)StructureValueType.TOTAL];
                StageDamageCurve stageDamageCurve = CreateStageDamageCurve(totalDamageFunc, func.DamageReachName, func.CategoryName, impactAreaElements, ref messages);
                if (stageDamageCurve != null)
                {
                    curves.Add(stageDamageCurve);
                }
            }

            //todo what if the count is zero
            string name = funcs[0].PlanName.Trim() + " - " + funcs[0].YearName.Trim();
            messages += "Group: " + name;
            messages += "\n\tNumber of curves: " + curves.Count + "\n\n";
            if (curves.Count > 0)
            {
                return new AggregatedStageDamageElement(name, funcs[0].CalculationDate, funcs[0].Description, -1, -1, curves, true);
            }
            else
            {
                return null;
            }
        }

        private static StageDamageCurve CreateStageDamageCurve(SingleDamageFunction sdf, string damageReachName, string damCat, 
            List<ImpactAreaElement> impactAreaElements, ref string messages)
        {
            damageReachName = damageReachName.Trim();
            damCat = damCat.Trim();

            StageDamageCurve curve = null;
            double[] depths = sdf.Depth;
            double[] damages = sdf.Damage;

            List<double> depthsList = new List<double>();
            List<double> damagesList = new List<double>();
            for (int i = 0; i < sdf.GetNumRows(); i++)
            {
                depthsList.Add(depths[i]);
                damagesList.Add(damages[i]);
            }
            //always use linear. This is the only option in Old Fda.
            //ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(depthsList, damagesList, InterpolationEnum.Linear);
            //IFdaFunction stageDamage = IFdaFunctionFactory.Factory( IParameterEnum.InteriorStageDamage, (IFunction)func);
            UncertainPairedData stageDamagePairedData = DefaultPairedData.CreateDefaultDeterminateUncertainPairedData(depthsList, damagesList, "Stage", "Damage", "Stage-Damage");

            //there should only ever be 0 or 1 impact area elements
            if (impactAreaElements.Count > 0)
            {
                ObservableCollection<ImpactAreaRowItem> impactAreaRows = ((ImpactAreaElement)impactAreaElements[0]).ImpactAreaRows;
                ImpactAreaRowItem selectedRow = null;

                //does this curve's damage reach equal an existing impact area?
                bool impactAreaMatches = false;
                foreach (ImpactAreaRowItem row in impactAreaRows)
                {
                    //the damage reach name needs to match an existing impact area to be included.
                    //message user if it does not.
                    if (row.Name.Equals(damageReachName))
                    {
                        impactAreaMatches = true;
                        curve = new StageDamageCurve(row, damCat, stageDamagePairedData);
                        break;
                    }
                    else
                    {
                        messages += Environment.NewLine + "The stage damage curve with damage reach of '" + damageReachName + "' could not be imported because it does not match any existing impact area names.";

                    }
                }
                //if (!impactAreaMatches)
                //{
                //    string msg = "The stage damage curve with damage reach of '" + damageReachName + "' could not be imported because it does not match any existing impact area names.";
                //    MessageBox.Show(msg, "Could Not Import", MessageBoxButton.OK, MessageBoxImage.Information);
                //}
            }

            return curve;
        }

        #endregion

        #region LP3
        public static List<AnalyticalFrequencyElement> CreateFlowFrequencyElements(ProbabilityFunctionList probFuncs)
        {
            List<AnalyticalFrequencyElement> elems = new List<AnalyticalFrequencyElement>();
            foreach (KeyValuePair<string, ProbabilityFunction> kvp in probFuncs.ProbabilityFunctions)
            {
                ProbabilityFunction pf = kvp.Value;
                FrequencyFunctionType typeID = pf.ProbabilityFunctionTypeId;
                if (typeID == FrequencyFunctionType.ANALYTICAL || typeID == FrequencyFunctionType.GRAPHICAL)
                {
                    AnalyticalFrequencyElement freqElem = CreateFrequencyElement(pf);
                    if(freqElem != null)
                    {
                        elems.Add(freqElem);
                    }
                }
            }
            return elems;
        }

        private static string CreatePYSRDescription(ProbabilityFunction pf)
        {
            string pysr = "(" + pf.PlanName.Trim() + " " + pf.YearName.Trim() + " " + pf.StreamName.Trim() + " " + pf.DamageReachName.Trim() + ") ";
            return pysr + pf.Description;
        }

        private static AnalyticalFrequencyElement CreateManualAnalyticalElement(ProbabilityFunction pf)
        {
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            double mean = pf.MomentsLp3[0];
            double stDev = pf.MomentsLp3[1];
            double skew = pf.MomentsLp3[2];

            int por = pf.EquivalentLengthOfRecord;

            bool isAnalytical = true;
            bool isStandard = true;//This boolean says whether it is "fit to params" or "fit to flows". True = "fit to params"
            bool isLogFlow = false;

            //there will be no analytical flows. We just need 
            List<double> analyticalFlows = new List<double>();
            List<double> graphicalFlows = new List<double>();

            
            return new AnalyticalFrequencyElement(pf.Name, editDate, CreatePYSRDescription(pf), por, isAnalytical, isStandard, mean, stDev, skew,
                isLogFlow, analyticalFlows, graphicalFlows, null);
        }

        private static AnalyticalFrequencyElement CreateFrequencyElement(ProbabilityFunction pf)
        {
            AnalyticalFrequencyElement elem = null;
            if (pf.ProbabilityFunctionTypeId == FrequencyFunctionType.ANALYTICAL)
            {
                if (pf.SourceOfStatisticsId == SourceOfStatistics.ENTERED)
                {
                    elem = CreateManualAnalyticalElement(pf);
                }
            }
            else if (pf.ProbabilityFunctionTypeId == FrequencyFunctionType.GRAPHICAL)
            {
                //todo: graphical will be addressed in a future task.

                ////get probabilities
                //List<double> probabilities = new List<double>();
                //for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
                //{
                //    probabilities.Add(pf.ExceedanceProbability[i]);
                //}

                //if (pf.ProbabilityDataTypeId == ProbabilityDataType.DISCHARGE_FREQUENCY)
                //{
                //    Write("\t\tDischarge: ");
                //    for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
                //        Write($"\t{pf.Discharge[i]}");
                //}
                //else if (pf.ProbabilityDataTypeId == ProbabilityDataType.STAGE_FREQUENCY)
                //{
                //    Write("\t\tStage: ");
                //    for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
                //        Write($"\t{pf.Stage[i]}");
                //}
                ////User Defined Uncertainty
                //if (pf.UncertTypeSpecification == UncertaintyTypeSpecification.NORMAL)
                //{
                //    Write("\t\tNormal: ");
                //    for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
                //        Write($"\t{pf._StdDevNormalUserDef[i]}");
                //    Write("\n");
                //}
                //else if (pf.UncertTypeSpecification == UncertaintyTypeSpecification.LOG_NORMAL)
                //{
                //    Write("\t\tLog Normal: ");
                //    for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
                //        Write($"\t{pf._StdDevLogUserDef[i]}");
                //    Write("\n");
                //}
                //else if (pf.UncertTypeSpecification == UncertaintyTypeSpecification.TRIANGULAR)
                //{
                //    Write("\t\tTriangular High: ");
                //    for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
                //        Write($"\t{pf._StdDevUpperUserDef[i]}");
                //    Write("\n");
                //    Write("\t\tTriangular Low: ");
                //    for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
                //        Write($"\t{pf._StdDevLowerUserDef[i]}");
                //}
            }

            return elem;
        }

        #endregion

        #region inflow outflow

        public static List<InflowOutflowElement> CreateInflowOutflowElements(ProbabilityFunctionList probFuncs)
        {
            List<InflowOutflowElement> elems = new List<InflowOutflowElement>();
            foreach (KeyValuePair<string, ProbabilityFunction> kvp in probFuncs.ProbabilityFunctions)
            {
                ProbabilityFunction pf = kvp.Value;                
                if (pf.NumberOfTransFlowPoints > 0)
                {
                    InflowOutflowElement elem = CreateInflowOutflow(pf);
                    if (elem != null)
                    {
                        elems.Add(elem);
                    }
                }
            }
            return elems;
        }

        private static InflowOutflowElement CreateInflowOutflow(ProbabilityFunction probFunction)
        {
            List<IDistribution> distributedOrdinates = GetUncertaintyValues(probFunction);

            UncertainPairedData func = new UncertainPairedData(probFunction.TransFlowInflow, distributedOrdinates.ToArray(), "Inflow", "Outflow", "Inflow-Outflow", "", -1);
            return new InflowOutflowElement(probFunction.Name, probFunction.CalculationDate, CreatePYSRDescription(probFunction), func);
        }

        private static List<IDistribution> GetUncertaintyValues(ProbabilityFunction probFunction)
        {
            List<IDistribution> ords = new List<IDistribution>();
            if (probFunction.ErrorTypeTransformFlow == ErrorType.NORMAL)
            {
                for (int i = 0; i < probFunction.NumberOfTransFlowPoints; i++)
                {
                    //todo: what is the mean here? i am using the outflow for now.
                    ords.Add(new Normal(probFunction.TransFlowOutflow[i], probFunction.TransFlowStdDev[i]));
                }
            }
            else if (probFunction.ErrorTypeTransformFlow == ErrorType.LOGNORMAL)
            {
                for (int i = 0; i < probFunction.NumberOfTransFlowPoints; i++)
                {
                    //todo: need a log normal
                    ords.Add(new LogNormal(probFunction.TransFlowOutflow[i], probFunction.TransFlowStdDev[i]));
                }
            }
            else if (probFunction.ErrorTypeTransformFlow == ErrorType.TRIANGULAR)
            {
                for (int i = 0; i < probFunction.NumberOfTransFlowPoints; i++)
                {
                    ords.Add(new Triangular(probFunction.TransFlowOutflow[i], probFunction.TransFlowLower[i], probFunction.TransFlowUpper[i]));
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
            List<IOccupancyType> fda2Occtypes = new List<IOccupancyType>();
            foreach (Importer.OccupancyType ot in ots.Occtypes)
            {
                try
                {
                    fda2Occtypes.Add(GetFDA2OccupancyType(ot));
                }
                catch(Exception e)
                {
                    string errorMsg = Environment.NewLine + "Failed to import occupancy type '" + ot.Name + "' because of the following exception:";
                    messages += errorMsg + Environment.NewLine + e.Message + Environment.NewLine;
                }
            }
            int newGroupID = Saving.PersistenceFactory.GetOccTypeManager().GetUnusedId();

            OccupancyTypesElement elem = new OccupancyTypesElement(groupName, newGroupID, fda2Occtypes);
            return elem;
        }

        private static IOccupancyType GetFDA2OccupancyType(Importer.OccupancyType ot1)
        {
            List<string> errorMessages = new List<string>();
            //translate from old occtype to new occtype
            IOccupancyType ot = OccupancyTypeFactory.Factory();

            //what do i need for a new ot
            ot.Name = ot1.Name;
            ot.Description = ot1.Description;
            ot.DamageCategory = DamageCategoryFactory.Factory(ot1.CategoryName);

            //the single damage functions will always be in this order
            //public enum StructureValueType { STRUCTURE, CONTENT, OTHER, CAR, TOTAL };
            //this list is in the order of the enum
            List<UncertainPairedData> coordFunctions = TranslateSingleDamageFunctionToCoordinatesFunctions(ot1, errorMessages);
            ot.StructureDepthDamageFunction = coordFunctions[(int)StructureValueType.STRUCTURE];
            ot.ContentDepthDamageFunction = coordFunctions[(int)StructureValueType.CONTENT];
            ot.VehicleDepthDamageFunction = coordFunctions[(int)StructureValueType.CAR];
            ot.OtherDepthDamageFunction = coordFunctions[(int)StructureValueType.OTHER];

            //the error distributions are in the following order:
            //public enum OccTypeStrucComponent { FFLOOR, STRUCTURE, CONTENT, OTHER, AUTO};

            //ffloor and structure 
            //* normal: make mean = 100

            List<IDistribution> uncertainties = TranslateErrorDistributionsToIOrdinates(ot1._ErrorDistribution);
            ot.StructureValueUncertainty = uncertainties[(int)OccTypeStrucComponent.STRUCTURE];
            ot.ContentValueUncertainty = uncertainties[(int)OccTypeStrucComponent.CONTENT];
            ot.VehicleValueUncertainty = uncertainties[(int)OccTypeStrucComponent.AUTO];
            ot.OtherValueUncertainty = uncertainties[(int)OccTypeStrucComponent.OTHER];
            ot.FoundationHeightUncertainty = uncertainties[(int)OccTypeStrucComponent.FFLOOR];

            //there is no concept of a value uncertainty type in old FDA, so default to percent of mean
            ot.StructureUncertaintyType = ValueUncertaintyType.PercentOfMean;
            ot.ContentUncertaintyType = ValueUncertaintyType.PercentOfMean;
            ot.VehicleUncertaintyType = ValueUncertaintyType.PercentOfMean;
            ot.OtherUncertaintyType = ValueUncertaintyType.PercentOfMean;

            ot.CalculateStructureDamage = true;
            ot.CalculateContentDamage = true;
            ot.CalculateVehicleDamage = true;
            ot.CalculateOtherDamage = false;

            return ot;
        }
        private static IDistribution TranslateErrorDistToOrdinate(ErrorDistribution errorDist)
        {
            double mean = errorDist.GetCentralValue();
            //st dev gets reused as min
            double stDev = errorDist.GetStdDev();
            double max = errorDist.GetUpper();
            ErrorType type = errorDist.GetErrorType();
            switch (type)
            {
                case ErrorType.NONE:
                        return new Deterministic(mean);
                case ErrorType.NORMAL:
                        return new Normal(mean, stDev);
                case ErrorType.TRIANGULAR:
                        //The mean is always 100. The importer code has the value at -901 so we hardcode it here.
                        return new Triangular(100, stDev, max); 
                case ErrorType.UNIFORM:
                        //todo: there is no lower. What to do?
                        return new Uniform(mean, max);
                case ErrorType.LOGNORMAL:
                        throw new NotImplementedException();
                default:
                        //todo: do what
                        //something went wrong, lets just make it a constant?
                        return new Deterministic(mean);
            }
        }
        private static List<IDistribution> TranslateErrorDistributionsToIOrdinates(ErrorDistribution[] errorDists)
        {
            List<IDistribution> ordinates = new List<IDistribution>();
            foreach (ErrorDistribution errDist in errorDists)
            {
                ordinates.Add(TranslateErrorDistToOrdinate(errDist));
            }
            return ordinates;
        }

        private static List<UncertainPairedData> TranslateSingleDamageFunctionToCoordinatesFunctions(Importer.OccupancyType ot, List<string> errorMessages)
        {
            
            SingleDamageFunction[] singleDamageFunctions = ot._SingleDamageFunction;
            //the single damage functions will always be in this order
            //public enum StructureValueType { STRUCTURE, CONTENT, OTHER, CAR, TOTAL };
            List<UncertainPairedData> coordinatesFunctions = new List<UncertainPairedData>();
            //if an occtype fails to read in properly we will make a message and keep trying
            //to import other occtypes.
            for (int i = 0; i < singleDamageFunctions.Length; i++)
            {
                SingleDamageFunction func = singleDamageFunctions[i];
                StructureValueType type = (StructureValueType)i;
                UncertainPairedData function = null;

                if (IsEmptyFunction(func))
                {
                    //create a function with just (0,0)
                    function = CreateEmptyFunction();
                }
                else
                {
                    function = CreateCoordinatesFunction(ot.Name, func, type, errorMessages);

                }

                //the coordinates function will be null if it was not able to be created
                if (function == null)
                {
                    //create an empty coord function?
                    function = CreateEmptyFunction();
                }

                coordinatesFunctions.Add(function);
            }
            return coordinatesFunctions;
        }

        private static UncertainPairedData CreateEmptyFunction()
        {
            List<double> xs = new List<double>() { 1, 2, 3 };
            List<Deterministic> ys = new List<Deterministic>() { new Deterministic(1),new Deterministic(2),new Deterministic(3) };
            //todo: not sure these labels are correct.
            return new UncertainPairedData(xs.ToArray(), ys.ToArray(), "Stage", "Damage", "Stage Damage", "", -1);
        }

        private static bool IsEmptyFunction(SingleDamageFunction function)
        {
            List<double> depths = function.Depth.ToList<double>();
            List<double> damages = function.Damage.ToList<double>();

            for (int i = 0; i < depths.Count; i++)
            {
                double depth = depths[i];
                double damage = damages[i];
                if (depth != 0 || damage != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private static UncertainPairedData CreateCoordinatesFunction(string name, SingleDamageFunction function, StructureValueType structureValueType, List<string> errors)
        {
            List<double> depths = function.Depth.ToList<double>();
            List<double> damages = function.Damage.ToList<double>();
            //stDevs get reused as min values if the type is triangular
            List<double> stDevs = function.StdDev.ToList<double>();
            List<double> maxValues = function.ErrHi.ToList<double>();
            //todo: should i check the lists are the same size?
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
                return DefaultPairedData.CreateDefaultDeterminateUncertainPairedData(xs, ys, "", "", "");
            }
            catch (ArgumentException e)
            {
                errors.Add(CreateFailedCoordFunctionErrorMsg(structureValueType, name, e.Message));
                return null;
            }
        }

        private static UncertainPairedData CreateNormalFunction(string name, List<double> xs, List<double> ys, List<double> stDevs, StructureValueType structureValueType, List<string> errors)
        {
            List<Normal> yVals = new List<Normal>();
            for (int i = 0; i < xs.Count; i++)
            {
                yVals.Add(new Normal(ys[i], stDevs[i]));
            }
            return new UncertainPairedData(xs.ToArray(), yVals.ToArray(), "", "", "", "", -1);

            //    List<IDistributedOrdinate> ordinates = new List<IDistributedOrdinate>();
            //for (int i = 0; i < xs.Count; i++)
            //{
            //    ordinates.Add(IDistributedOrdinateFactory.FactoryNormal(ys[i], stDevs[i]));
            //}
            //try
            //{
            //    return ICoordinatesFunctionsFactory.Factory(xs, ordinates, InterpolationEnum.Linear);
            //}
            //catch (ArgumentException e)
            //{
            //    errors.Add(CreateFailedCoordFunctionErrorMsg(structureValueType, name, e.Message));
            //    return null;
            //}
        }

        private static UncertainPairedData CreateTriangularFunction(string name, List<double> xs, List<double> ys, List<double> mins, List<double> maxs, StructureValueType structureValueType, List<string> errors)
        {
            List<Triangular> yVals = new List<Triangular>();
            for (int i = 0; i < xs.Count; i++)
            {
                yVals.Add(new Triangular(mins[i], ys[i], maxs[i]));
            }

            return new UncertainPairedData(xs.ToArray(), yVals.ToArray(), "", "", "", "", -1);


            //List<IDistributedOrdinate> ordinates = new List<IDistributedOrdinate>();
            //for (int i = 0; i < xs.Count; i++)
            //{
            //    ordinates.Add(IDistributedOrdinateFactory.FactoryTriangular(ys[i], mins[i], maxs[i]));
            //}
            //try
            //{
            //    return ICoordinatesFunctionsFactory.Factory(xs, ordinates, InterpolationEnum.Linear);
            //}
            //catch (ArgumentException e)
            //{
            //    errors.Add(CreateFailedCoordFunctionErrorMsg(structureValueType, name, e.Message));
            //    return null;
            //}
        }

        private static UncertainPairedData CreateUniformFunction(string name, List<double> xs, List<double> mins, List<double> maxs, StructureValueType structureValueType, List<string> errors)
        {
            List<Uniform> yVals = new List<Uniform>();
            for (int i = 0; i < xs.Count; i++)
            {
                yVals.Add(new Uniform(mins[i], maxs[i]));
            }

            return new UncertainPairedData(xs.ToArray(), yVals.ToArray(), "", "", "", "", -1);

            //List<IDistributedOrdinate> ordinates = new List<IDistributedOrdinate>();
            //for (int i = 0; i < xs.Count; i++)
            //{
            //    ordinates.Add(IDistributedOrdinateFactory.FactoryUniform(mins[i], maxs[i]));
            //}
            //try
            //{
            //    return ICoordinatesFunctionsFactory.Factory(xs, ordinates, InterpolationEnum.Linear);
            //}
            //catch (ArgumentException e)
            //{
            //    errors.Add(CreateFailedCoordFunctionErrorMsg(structureValueType, name, e.Message));
            //    return null;
            //}
        }

        private static string CreateFailedCoordFunctionErrorMsg(StructureValueType type, string occtypeName, string exceptionMsg)
        {
            return "Could not create a '" + type + "' coordinates function for the occupancy type: " + occtypeName + ". " + exceptionMsg;
        }

        private static void FindConstantValuesForNormal(List<double> xs, List<double> ys, List<double> stDevs)
        {
            List<List<int>> masterListOfConstantIndexes = new List<List<int>>();
            List<int> indexOfConstants = GetIndexesWithConstantValuesForNormal(ys, stDevs);

            //so now we have all the indexes of constant values.
            //if there are any then we know we need to do a linked function
            //add the first point to a list
            List<int> currentConstantFunction = new List<int>();
            currentConstantFunction.Add(indexOfConstants[0]);
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
                    currentConstantFunction = new List<int>();
                    currentConstantFunction.Add(nextIndex);
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
            List<int> indexOfConstants = new List<int>();
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
            List<ChildElement> elems = new List<ChildElement>();
            foreach (KeyValuePair<string, Levee> kvp in leveeList.Levees)
            {
                Levee lev = kvp.Value;

                //if (lev.FailureFunctionPairs.Count > 0)
                {
                    elems.Add(CreateLeveeElement(lev, ref message));
                }
                //if (lev.ExteriorInteriorPairs.Count > 0)
                //{
                //    Saving.PersistenceFactory.GetExteriorInteriorManager().SaveFDA1Element(lev);
                //}

            }
            return elems;
        }
        private static string CreatePYSRDescription(Levee lev)
        {
            string pysr = "(" + lev.PlanName.Trim() + " " + lev.YearName.Trim() + " " + lev.StreamName.Trim() + " " + lev.DamageReachName.Trim() + ") ";
            return pysr + lev.Description;
        }

        private static ChildElement CreateLeveeElement(Levee lev, ref string message)
        {
            //List<ICoordinate> failureCoords = new List<ICoordinate>();
            List<double> xs = new List<double>();
            List<double> ys = new List<double>();

            foreach (Pair_xy xy in lev.FailureFunctionPairs)
            {
                xs.Add( xy.GetX());
                ys.Add( xy.GetY());
            }

            //ICoordinatesFunction coordsFunction = null;
            //todo: what if no coords here.
            //in this case then we create a special default coordinates function
            bool isDefault = true;
            UncertainPairedData func = new UncertainPairedData();
            if (xs.Count == 0)
            {
                //create default curve
                List<double> defaultXs = new List<double>() {lev.ElevationTopOfLevee, lev.ElevationTopOfLevee + .000000000000001 };
                List<Deterministic> defaultYs = new List<Deterministic>() { new Deterministic(0), new Deterministic(1) };                

                func = new UncertainPairedData(defaultXs.ToArray(), defaultYs.ToArray(), "Elevation", "Probability", "Failure Function", "", -1);
                message += "No failure function was detected.\nCreating default failure function at top of levee.";
            }
            else
            {
                List<Deterministic> yVals = new List<Deterministic>();
                foreach (double d in ys)
                {
                    yVals.Add(new Deterministic(d));
                }
                func = new UncertainPairedData(xs.ToArray(), yVals.ToArray(), "Elevation", "Probability", "Failure Function", "", -1);
                isDefault = false;
            }

            LeveeFeatureElement leveeFeatureElement = new LeveeFeatureElement(lev.Name, lev.CalculationDate, CreatePYSRDescription(lev), lev.ElevationTopOfLevee, isDefault, func);
            return leveeFeatureElement;
        }

        #endregion

        #region Exterior Interior
        public static List<ChildElement> CreateExteriorInteriors(LeveeList leveeList)
        {
            List<ChildElement> elems = new List<ChildElement>();
            foreach (KeyValuePair<string, Levee> kvp in leveeList.Levees)
            {
                Levee lev = kvp.Value;
                if (lev.ExteriorInteriorPairs.Count > 0)
                {
                    elems.Add(CreateExteriorInterior(lev));
                }
            }
            return elems;
        }

        private static ChildElement CreateExteriorInterior(Levee lev)
        {
            List<double> xs = new List<double>();
            List<Deterministic> ys = new List<Deterministic>();
            foreach (Pair_xy xy in lev.ExteriorInteriorPairs)
            {
                xs.Add( xy.GetX());
                ys.Add(new Deterministic(xy.GetY()));
            }
            UncertainPairedData func = new UncertainPairedData(xs.ToArray(), ys.ToArray(), "Exterior Stage", "Interior Stage", "Exterior-Interior", "", -1);
            ExteriorInteriorElement elem = new ExteriorInteriorElement(lev.Name, lev.CalculationDate, CreatePYSRDescription(lev), func);
            return elem;
        }

        #endregion

    }
}
