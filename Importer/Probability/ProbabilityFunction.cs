using Functions;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Console;

namespace Importer
{
    [Serializable]

    public class ProbabilityFunction : FdObjectData, ISaveToSqlite
    {
        #region enums
        public enum FrequencyFunctionType { FF_UNKNOWN, ANALYTICAL, GRAPHICAL };
        public enum SourceOfStatistics { FF_STATISTICS_NOT_SET, ENTERED, CALCULATED };
        public enum ProbabilityDataType { STAGE_FREQUENCY, DISCHARGE_FREQUENCY };
        public enum UncertaintyTypeSpecification { EQUIV_REC_LENGTH = -1, NONE, NORMAL, LOG_NORMAL, TRIANGULAR };
        #endregion
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private FrequencyFunctionType _ProbabilityFunctionTypeId = FrequencyFunctionType.FF_UNKNOWN;
        private ProbabilityDataType _ProbabilityDataTypeId = ProbabilityDataType.DISCHARGE_FREQUENCY;

        //Analytical
        private double[] _MomentsLp3 = new double[3];
        private double[] _PointsSynthetic = new double[3];

        //Graphical
        private double[] _ExceedanceProbability = null;
        private double[] _Discharge = null;
        private double[] _Stage = null;

        //User Defined Uncertainty
        private double[] _StdDevNormalUserDef = null;
        private double[] _StdDevLogUserDef = null;
        private double[] _StdDevUpperUserDef = null;
        private double[] _StdDevLowerUserDef = null;

        //Transform Flow
        private double[] _TransFlowInflow = null;
        private double[] _TransFlowOutflow = null;
        private double[] _TransFlowStdDev = null;
        private double[] _TransFlowLogStdDev = null;
        private double[] _TransFlowUpper = null;
        private double[] _TransFlowLower = null;

        //Calculation Ordinates
        public int _NumCalcPoints = 0;
        public int _NumCalcPointsAllocated = 0;
        private double[] _Calc50 = null;
        private double[] _Calc95 = null;
        private double[] _Calc75 = null;
        private double[] _Calc25 = null;
        private double[] _Calc05 = null;

        #endregion
        #region Properties
        public string PlanName
        { get; set; }
        public string YearName
        { get; set; }
        public string StreamName
        { get; set; }
        public string DamageReachName
        { get; set; }
        public int EquivalentLengthOfRecord
        { get; set; }

        //Analytical
        public SourceOfStatistics SourceOfStatisticsId
        { get; set; }

        //Graphical
        public UncertaintyTypeSpecification UncertTypeSpecification
        { get; set; }
        public int NumberOfGraphicalPoints
        { get; set; }
        public int NumberOfGraphicalPointsAllocated
        { get; set; }

        //Transform Flow
        public ErrorType ErrorTypeTransformFlow
        { get; set; }
        public int NumberOfTransFlowPoints
        { get; set; }
        public int NumberOfTransFlowPointsAllocated
        { get; set; }
        public FrequencyFunctionType ProbabilityFunctionTypeId { get => _ProbabilityFunctionTypeId; set => _ProbabilityFunctionTypeId = value; }
        public ProbabilityDataType ProbabilityDataTypeId { get => _ProbabilityDataTypeId; set => _ProbabilityDataTypeId = value; }
        public double[] MomentsLp3 { get => _MomentsLp3; set => _MomentsLp3 = value; }
        public double[] PointsSynthetic { get => _PointsSynthetic; set => _PointsSynthetic = value; }
        public double[] ExceedanceProbability { get => _ExceedanceProbability; set => _ExceedanceProbability = value; }
        public double[] Discharge { get => _Discharge; set => _Discharge = value; }
        public double[] Stage { get => _Stage; set => _Stage = value; }
        public double[] StdDevNormalUserDef { get => _StdDevNormalUserDef; set => _StdDevNormalUserDef = value; }
        public double[] StdDevLogUserDef { get => _StdDevLogUserDef; set => _StdDevLogUserDef = value; }
        public double[] StdDevUpperUserDef { get => _StdDevUpperUserDef; set => _StdDevUpperUserDef = value; }
        public double[] StdDevLowerUserDef { get => _StdDevLowerUserDef; set => _StdDevLowerUserDef = value; }
        public double[] TransFlowInflow { get => _TransFlowInflow; set => _TransFlowInflow = value; }
        public double[] TransFlowOutflow { get => _TransFlowOutflow; set => _TransFlowOutflow = value; }
        public double[] TransFlowStdDev { get => _TransFlowStdDev; set => _TransFlowStdDev = value; }
        public double[] TransFlowLogStdDev { get => _TransFlowLogStdDev; set => _TransFlowLogStdDev = value; }
        public double[] TransFlowUpper { get => _TransFlowUpper; set => _TransFlowUpper = value; }
        public double[] TransFlowLower { get => _TransFlowLower; set => _TransFlowLower = value; }

        public double[] Calc50 { get => _Calc50; set => _Calc50 = value; }
        public double[] Calc95 { get => _Calc95; set => _Calc95 = value; }
        public double[] Calc75 { get => _Calc75; set => _Calc75 = value; }
        public double[] Calc25 { get => _Calc25; set => _Calc25 = value; }
        public double[] Calc05 { get => _Calc05; set => _Calc05 = value; }

        #endregion
        #region Constructors
        public ProbabilityFunction()
        {
            //Graphical
            NumberOfGraphicalPointsAllocated = 0;
            NumberOfGraphicalPoints = 0;
            _ExceedanceProbability = null;
            _Discharge = null;
            _Stage = null;
            _StdDevNormalUserDef = null;
            _StdDevLogUserDef = null;
            _StdDevLowerUserDef = null;
            _StdDevUpperUserDef = null;
            /*
            ExceedanceProbability = new double[NumberOfGraphicalPointsAllocated];
            Discharge = new double[NumberOfGraphicalPointsAllocated];
            Stage = new double[NumberOfGraphicalPointsAllocated];
            for (int i = 0; i < NumberOfGraphicalPointsAllocated; i++)
            {
                ExceedanceProbability[i] = Study.badNumber;
                Discharge[i] = Study.badNumber;
                Stage[i] = Study.badNumber;
            }
            //User Defined Uncertainty
            _StdDevNormalUserDef = new double[NumberOfGraphicalPointsAllocated];
            _StdDevLogUserDef = new double[NumberOfGraphicalPointsAllocated];
            _StdDevUpperUserDef = new double[NumberOfGraphicalPointsAllocated];
            _StdDevLowerUserDef = new double[NumberOfGraphicalPointsAllocated];
            for (int i = 0; i < NumberOfGraphicalPointsAllocated; i++)
            {
                _StdDevNormalUserDef[i] = Study.badNumber;
                _StdDevLogUserDef[i] = Study.badNumber;
                _StdDevUpperUserDef[i] = Study.badNumber;
                _StdDevLowerUserDef[i] = Study.badNumber;
            }
            */
            //Transform Flow
            ErrorTypeTransformFlow = ErrorType.NONE;
            NumberOfTransFlowPointsAllocated = 0;
            NumberOfTransFlowPoints = 0;
            TransFlowInflow = null;
            TransFlowOutflow = null;
            TransFlowStdDev = null;
            TransFlowLogStdDev = null;
            TransFlowUpper = null;
            TransFlowLower = null;

            //Calculation Points
            _NumCalcPoints = 0;
            _NumCalcPointsAllocated = 0;
            _Calc50 = null;
            _Calc95 = null;
            _Calc75 = null;
            _Calc25 = null;
            _Calc05 = null;

            Reset();
        }
        #endregion
        #region Voids
        public void Deallocate()
        {
            NumberOfGraphicalPoints = 0;
            NumberOfGraphicalPointsAllocated = 0;
            _ExceedanceProbability = null;
            _Discharge = null;
            _Stage = null;
            _StdDevNormalUserDef = null;
            _StdDevLogUserDef = null;
            _StdDevUpperUserDef = null;
            _StdDevLowerUserDef = null;

            NumberOfTransFlowPoints = 0;
            NumberOfTransFlowPointsAllocated = 0;
            _TransFlowInflow = null;
            _TransFlowOutflow = null;
            _TransFlowStdDev = null;
            _TransFlowLogStdDev = null;
            _TransFlowLower = null;
            _TransFlowUpper = null;

            _NumCalcPoints = 0;
            _NumCalcPointsAllocated = 0;
            _Calc50 = null;
            _Calc95 = null;
            _Calc75 = null;
            _Calc25 = null;
            _Calc05 = null;

        }
        public new void Reset()
        {
            base.Reset();
            ProbabilityFunctionTypeId = FrequencyFunctionType.FF_UNKNOWN;
            ProbabilityDataTypeId = ProbabilityDataType.DISCHARGE_FREQUENCY;
            EquivalentLengthOfRecord = 50;
            SourceOfStatisticsId = SourceOfStatistics.FF_STATISTICS_NOT_SET;
            for (int i = 0; i < 3; i++) MomentsLp3[i] = Study.badNumber;
            for (int i = 0; i < 3; i++) PointsSynthetic[i] = Study.badNumber;
            UncertTypeSpecification = UncertaintyTypeSpecification.EQUIV_REC_LENGTH;

            ResetGraphical();
            ResetUserDefined();
            ResetTransformFlow();
        }
        protected void ResetGraphical()
        {
            //Graphical
            NumberOfGraphicalPoints = 0;
            for (int i = 0; i < NumberOfGraphicalPointsAllocated; i++)
            {
                ExceedanceProbability[i] = Study.badNumber;
                Discharge[i] = Study.badNumber;
                Stage[i] = Study.badNumber;
            }
            return;
        }
        protected void ResetUserDefined()
        {
            NumberOfGraphicalPoints = 0;
            //User Defined
            for (int i = 0; i < NumberOfGraphicalPointsAllocated; i++)
            {
                _StdDevNormalUserDef[i] = Study.badNumber;
                _StdDevLogUserDef[i] = Study.badNumber;
                _StdDevUpperUserDef[i] = Study.badNumber;
                _StdDevLowerUserDef[i] = Study.badNumber;
            }
            return;
        }
        protected void ResetTransformFlow()
        {
            //Transform Flow
            NumberOfTransFlowPoints = 0;
            for (int i = 0; i < NumberOfTransFlowPointsAllocated; i++)
            {
                TransFlowInflow[i] = Study.badNumber;
                TransFlowOutflow[i] = Study.badNumber;
                TransFlowStdDev[i] = Study.badNumber;
                TransFlowLogStdDev[i] = Study.badNumber;
                TransFlowUpper[i] = Study.badNumber;
                TransFlowLower[i] = Study.badNumber;
            }
            return;
        }
        protected void ResetCalculationPoints()
        {
            _NumCalcPoints = 0;
            for(int i = 0; i < _NumCalcPointsAllocated; i++)
            {
                Calc05[i] = Study.badNumber;
                Calc25[i] = Study.badNumber;
                Calc50[i] = Study.badNumber;
                Calc75[i] = Study.badNumber;
                Calc95[i] = Study.badNumber;
            }
            return;
        }
        public void ReallocateGraphicalWithCheckAndSave(int numOrdsAlloc)
        {
            if (numOrdsAlloc > NumberOfGraphicalPointsAllocated)
            {
                double[] prob = new double[numOrdsAlloc];
                double[] flow = new double[numOrdsAlloc];
                double[] stage = new double[numOrdsAlloc];
                double[] errNormal = new double[numOrdsAlloc];
                double[] errLogNormal = new double[numOrdsAlloc];
                double[] errUpper = new double[numOrdsAlloc];
                double[] errLower = new double[numOrdsAlloc];

                for (int i = 0; i < numOrdsAlloc; i++)
                {
                    prob[i] = flow[i] = stage[i] = errNormal[i] = errLogNormal[i] = errUpper[i] = errLower[i] = Study.badNumber;
                }

                for (int i = 0; i < NumberOfGraphicalPointsAllocated; i++)
                {
                    prob[i] = ExceedanceProbability[i];
                    flow[i] = Discharge[i];
                    stage[i] = this.Stage[i];
                    errNormal[i] = _StdDevNormalUserDef[i];
                    errLogNormal[i] = _StdDevLogUserDef[i];
                    errUpper[i] = _StdDevUpperUserDef[i];
                    errLower[i] = _StdDevLowerUserDef[i];
                }
                NumberOfGraphicalPointsAllocated = numOrdsAlloc;
                ExceedanceProbability = prob;
                Discharge = flow;
                this.Stage = stage;
                _StdDevNormalUserDef = errNormal;
                _StdDevLogUserDef = errLogNormal;
                _StdDevUpperUserDef = errUpper;
                _StdDevLowerUserDef = errLower;
            }
            return;
        }
        public void ReallocateTransformFlowWithCheckAndSave(int numOrdsAlloc)
        {
            if (numOrdsAlloc > NumberOfTransFlowPointsAllocated)
            {
                double[] inflow = new double[numOrdsAlloc];
                double[] outflow = new double[numOrdsAlloc];
                double[] errNormal = new double[numOrdsAlloc];
                double[] errLogNormal = new double[numOrdsAlloc];
                double[] errUpper = new double[numOrdsAlloc];
                double[] errLower = new double[numOrdsAlloc];
                for (int i = 0; i < numOrdsAlloc; i++)
                {
                    inflow[i] = outflow[i] = errNormal[i] = errLogNormal[i] = errUpper[i] = errLower[i] = Study.badNumber;
                }
                for (int i = 0; i < NumberOfTransFlowPointsAllocated; i++)
                {
                    inflow[i] = TransFlowInflow[i];
                    outflow[i] = TransFlowOutflow[i];
                    errNormal[i] = TransFlowStdDev[i];
                    errLogNormal[i] = TransFlowLogStdDev[i];
                    errUpper[i] = TransFlowUpper[i];
                    errLower[i] = TransFlowLower[i];
                }
                NumberOfTransFlowPointsAllocated = numOrdsAlloc;
                TransFlowInflow = inflow;
                TransFlowOutflow = outflow;
                TransFlowStdDev = errNormal;
                TransFlowLogStdDev = errLogNormal;
                TransFlowUpper = errUpper;
                TransFlowLower = errLower;
            }
            return;
        }
        public void ReallocateCalculationPointsWithCheckAndSave(int numOrdsAlloc)
        {
            if (numOrdsAlloc > _NumCalcPointsAllocated)
            {
                double[] q05 = new double[numOrdsAlloc];
                double[] q25 = new double[numOrdsAlloc];
                double[] q50 = new double[numOrdsAlloc];
                double[] q75 = new double[numOrdsAlloc];
                double[] q95 = new double[numOrdsAlloc];
                for (int i = 0; i < _NumCalcPointsAllocated; i++)
                {
                    q05[i] = q25[i] = q50[i] = q75[i] = q95[i] = Study.badNumber;
                }
                for (int i = 0; i < _NumCalcPointsAllocated; i++)
                {
                    q05[i] = Calc05[i];
                    q25[i] = Calc25[i];
                    q50[i] = Calc50[i];
                    q75[i] = Calc75[i];
                    q95[i] = Calc95[i];
                }
                _NumCalcPointsAllocated = numOrdsAlloc;
                Calc05 = q05;
                Calc25 = q25;
                Calc50 = q50;
                Calc75 = q75;
                Calc95 = q95;
            }
            return;
        }
        public void Print()
        {
            //Basic Information
            WriteLine($"\n\nProbability Function Name: {Name}");
            WriteLine($"\tDescription: {this.Description}");
            WriteLine($"\tPlan: {PlanName}");
            WriteLine($"\tYear: {YearName}");
            WriteLine($"\tStream: {StreamName}");
            WriteLine($"\tReach: {DamageReachName}");
            WriteLine($"\tEquiv. Length of Record: {EquivalentLengthOfRecord}");
            WriteLine($"\tProbability Function Type: {ProbabilityFunctionTypeId}");
            WriteLine($"\tData Type: {(ProbabilityDataTypeId)}");
            WriteLine($"\tUncertainty Type: {UncertTypeSpecification}");

            //Paired Data
            if (ProbabilityFunctionTypeId == FrequencyFunctionType.ANALYTICAL)
            {
                if (SourceOfStatisticsId == SourceOfStatistics.ENTERED)
                {
                    WriteLine($"\tLog Pearson Type III moments:");
                    WriteLine($"\t\tMean: {MomentsLp3[0]}");
                    WriteLine($"\t\tStd. Dev.: {MomentsLp3[1]}");
                    WriteLine($"\t\tSkew: {MomentsLp3[2]}");
                }
                else if (SourceOfStatisticsId == SourceOfStatistics.CALCULATED)
                {
                    WriteLine($"\tAnalytical Synthetic Points:");
                    WriteLine($"\t\tFlow @ 0.50: {PointsSynthetic[0]}");
                    WriteLine($"\t\tFlow @ 0.10: {PointsSynthetic[1]}");
                    WriteLine($"\t\tFlow @ 0.01: {PointsSynthetic[2]}");
                }
            }
            else if (ProbabilityFunctionTypeId == FrequencyFunctionType.GRAPHICAL)
            {
                WriteLine($"\tGraphical Points:");
                Write("\t\tProbability: ");
                for (int i = 0; i < NumberOfGraphicalPoints; i++)
                    Write($"\t{ExceedanceProbability[i]}");
                Write("\n");
                if (ProbabilityDataTypeId == ProbabilityDataType.DISCHARGE_FREQUENCY)
                {
                    Write("\t\tDischarge: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        Write($"\t{Discharge[i]}");
                }
                else if (ProbabilityDataTypeId == ProbabilityDataType.STAGE_FREQUENCY)
                {
                    Write("\t\tStage: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        Write($"\t{Stage[i]}");
                }
                Write("\n");

                //User Defined Uncertainty
                if (UncertTypeSpecification == UncertaintyTypeSpecification.NORMAL)
                {
                    Write("\t\tNormal: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        Write($"\t{_StdDevNormalUserDef[i]}");
                    Write("\n");
                }
                else if (UncertTypeSpecification == UncertaintyTypeSpecification.LOG_NORMAL)
                {
                    Write("\t\tLog Normal: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        Write($"\t{_StdDevLogUserDef[i]}");
                    Write("\n");
                }
                else if (UncertTypeSpecification == UncertaintyTypeSpecification.TRIANGULAR)
                {
                    Write("\t\tTriangular High: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        Write($"\t{_StdDevUpperUserDef[i]}");
                    Write("\n");
                    Write("\t\tTriangular Low: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        Write($"\t{_StdDevLowerUserDef[i]}");
                }
            }
            //Transform Flow Function
            if (NumberOfTransFlowPoints > 0)
            {
                WriteLine("\n\tTransform Flow Function");
                Write("\t\tInflow: ");
                for (int i = 0; i < NumberOfTransFlowPoints; i++)
                    Write($"\t{TransFlowInflow[i]}");
                Write("\n");
                Write("\t\tOutflow: ");
                for (int i = 0; i < NumberOfTransFlowPoints; i++)
                    Write($"\t{TransFlowOutflow[i]}");
                Write("\n");
                if (ErrorTypeTransformFlow == ErrorType.NORMAL)
                {
                    Write("\t\tNormal: ");
                    for (int i = 0; i < NumberOfTransFlowPoints; i++)
                        Write($"\t{TransFlowStdDev[i]}");
                    Write("\n");
                }
                else if (ErrorTypeTransformFlow == ErrorType.LOGNORMAL)
                {
                    Write("\t\tLog Normal: ");
                    for (int i = 0; i < NumberOfTransFlowPoints; i++)
                        Write($"\t{TransFlowLogStdDev[i]}");
                    Write("\n");
                }
                else if (ErrorTypeTransformFlow == ErrorType.TRIANGULAR)
                {
                    Write("\t\tTriangular High: ");
                    for (int i = 0; i < NumberOfTransFlowPoints; i++)
                        Write($"\t{ TransFlowUpper[i]}");
                    Write("\n");
                    Write("\t\tTriangular Low: ");
                    for (int i = 0; i < NumberOfTransFlowPoints; i++)
                        Write($"\t{TransFlowLower[i]}");
                    Write("\n");
                }
            }
            //Calculation Points
            if(_NumCalcPoints > 0)
            {
                WriteLine("\n\tCalculation Points");
                Write("\t\tQ05: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    Write($"\t{Calc05[i]}");
                Write("\n");

                Write("\t\tQ25: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    Write($"\t{Calc25[i]}");
                Write("\n");

                Write("\t\tQ50: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    Write($"\t{Calc50[i]}");
                Write("\n");

                Write("\t\tQ75: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    Write($"\t{Calc75[i]}");
                Write("\n");

                Write("\t\tQ95: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    Write($"\t{Calc95[i]}");
                Write("\n");
            }
            return;
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            //Basic Information
            wr.WriteLine($"\n\nProbability Function Name: {Name}");
            wr.WriteLine($"\tDescription: {this.Description}");
            wr.WriteLine($"\tPlan: {PlanName}");
            wr.WriteLine($"\tYear: {YearName}");
            wr.WriteLine($"\tStream: {StreamName}");
            wr.WriteLine($"\tReach: {DamageReachName}");
            wr.WriteLine($"\tEquiv. Length of Record: {EquivalentLengthOfRecord}");
            wr.WriteLine($"\tProbability Function Type: {ProbabilityFunctionTypeId}");
            wr.WriteLine($"\tData Type: {(ProbabilityDataTypeId)}");
            wr.WriteLine($"\tUncertainty Type: {UncertTypeSpecification}");

            //Paired Data
            if (ProbabilityFunctionTypeId == FrequencyFunctionType.ANALYTICAL)
            {
                if (SourceOfStatisticsId == SourceOfStatistics.ENTERED)
                {
                    wr.WriteLine($"\tLog Pearson Type III moments:");
                    wr.WriteLine($"\t\tMean: {MomentsLp3[0]}");
                    wr.WriteLine($"\t\tStd. Dev.: {MomentsLp3[1]}");
                    wr.WriteLine($"\t\tSkew: {MomentsLp3[2]}");
                }
                else if (SourceOfStatisticsId == SourceOfStatistics.CALCULATED)
                {
                    wr.WriteLine($"\tAnalytical Synthetic Points:");
                    wr.WriteLine($"\t\tFlow @ 0.50: {PointsSynthetic[0]}");
                    wr.WriteLine($"\t\tFlow @ 0.10: {PointsSynthetic[1]}");
                    wr.WriteLine($"\t\tFlow @ 0.01: {PointsSynthetic[2]}");
                }
            }
            else if (ProbabilityFunctionTypeId == FrequencyFunctionType.GRAPHICAL)
            {
                wr.WriteLine($"\tGraphical Points:");
                wr.Write("\t\tProbability: ");
                for (int i = 0; i < NumberOfGraphicalPoints; i++)
                    wr.Write($"\t{ExceedanceProbability[i]}");
                wr.Write("\n");
                if (ProbabilityDataTypeId == ProbabilityDataType.DISCHARGE_FREQUENCY)
                {
                    wr.Write("\t\tDischarge: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        wr.Write($"\t{Discharge[i]}");
                }
                else if (ProbabilityDataTypeId == ProbabilityDataType.STAGE_FREQUENCY)
                {
                    wr.Write("\t\tStage: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        wr.Write($"\t{Stage[i]}");
                }
                wr.Write("\n");

                //User Defined Uncertainty
                if (UncertTypeSpecification == UncertaintyTypeSpecification.NORMAL)
                {
                    wr.Write("\t\tNormal: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        wr.Write($"\t{_StdDevNormalUserDef[i]}");
                    wr.Write("\n");
                }
                else if (UncertTypeSpecification == UncertaintyTypeSpecification.LOG_NORMAL)
                {
                    wr.Write("\t\tLog Normal: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        wr.Write($"\t{_StdDevLogUserDef[i]}");
                    wr.Write("\n");
                }
                else if (UncertTypeSpecification == UncertaintyTypeSpecification.TRIANGULAR)
                {
                    wr.Write("\t\tTriangular High: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        wr.Write($"\t{_StdDevUpperUserDef[i]}");
                    wr.Write("\n");
                    wr.Write("\t\tTriangular Low: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        wr.Write($"\t{_StdDevLowerUserDef[i]}");
                }
            }
            //Transform Flow Function
            if (NumberOfTransFlowPoints > 0)
            {
                wr.WriteLine("\n\tTransform Flow Function");
                wr.Write("\t\tInflow: ");
                for (int i = 0; i < NumberOfTransFlowPoints; i++)
                    wr.Write($"\t{TransFlowInflow[i]}");
                wr.Write("\n");
                wr.Write("\t\tOutflow: ");
                for (int i = 0; i < NumberOfTransFlowPoints; i++)
                    wr.Write($"\t{TransFlowOutflow[i]}");
                wr.Write("\n");
                if (ErrorTypeTransformFlow == ErrorType.NORMAL)
                {
                    wr.Write("\t\tNormal: ");
                    for (int i = 0; i < NumberOfTransFlowPoints; i++)
                        wr.Write($"\t{TransFlowStdDev[i]}");
                    wr.Write("\n");
                }
                else if (ErrorTypeTransformFlow == ErrorType.LOGNORMAL)
                {
                    wr.Write("\t\tLog Normal: ");
                    for (int i = 0; i < NumberOfTransFlowPoints; i++)
                        wr.Write($"\t{TransFlowLogStdDev[i]}");
                    wr.Write("\n");
                }
                else if (ErrorTypeTransformFlow == ErrorType.TRIANGULAR)
                {
                    wr.Write("\t\tTriangular High: ");
                    for (int i = 0; i < NumberOfTransFlowPoints; i++)
                        wr.Write($"\t{ TransFlowUpper[i]}");
                    wr.Write("\n");
                    wr.Write("\t\tTriangular Low: ");
                    for (int i = 0; i < NumberOfTransFlowPoints; i++)
                        wr.Write($"\t{TransFlowLower[i]}");
                    wr.Write("\n");
                }
            }
            //Calculation Points
            if (_NumCalcPoints > 0)
            {
                wr.WriteLine("\n\tCalculation Points");
                wr.Write("\t\tQ05: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    wr.Write($"\t{Calc05[i]}");
                wr.Write("\n");

                wr.Write("\t\tQ25: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    wr.Write($"\t{Calc25[i]}");
                wr.Write("\n");

                wr.Write("\t\tQ50: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    wr.Write($"\t{Calc50[i]}");
                wr.Write("\n");

                wr.Write("\t\tQ75: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    wr.Write($"\t{Calc75[i]}");
                wr.Write("\n");

                wr.Write("\t\tQ95: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    wr.Write($"\t{Calc95[i]}");
                wr.Write("\n");
            }
            return;
        }
        public void Export(StreamWriter wr, char delimt)
        {
            ExportHeader(wr, delimt);
            ExportName(wr, delimt);
            ExportData(wr, delimt);
            return;
        }
        protected void ExportHeader(StreamWriter wr, char delimt)
        {
            //        {   "FREQ_NAME",        // 1
            //            "PLAN",             // 2
            //            "YEAR",             // 3
            //            "STREAM",           // 4
            //            "REACH",            // 5
            //            "YRS",              // 6
            //            "FFTYPE",           // 7
            //            "DATATYPE",         // 8
            //             "UNCERTTYPE",       // 9
            //            "DESC"              //10


            for (int i = 0; i < AsciiImportExport.FieldsProbFunc.Length; i++)
            {
                wr.Write($"{ AsciiImportExport.FieldsProbFunc[i]}{ delimt}{delimt}");
            }
            wr.Write("\n");
            return;
        }
        protected void ExportName(StreamWriter wr, char delimt)
        {
            wr.Write($"{this.Name}{delimt}{delimt}");
            wr.Write($"{this.PlanName}{delimt}{delimt}");
            wr.Write($"{this.YearName}{delimt}{delimt}");
            wr.Write($"{this.StreamName}{delimt}{delimt}");
            wr.Write($"{this.DamageReachName}{delimt}{delimt}");
            wr.Write($"{this.EquivalentLengthOfRecord}{delimt}{delimt}");

            if (this.ProbabilityFunctionTypeId == FrequencyFunctionType.ANALYTICAL)
            {
                if (this.SourceOfStatisticsId == SourceOfStatistics.ENTERED)
                    wr.Write($"L{delimt}{delimt}");
                else if (this.SourceOfStatisticsId == SourceOfStatistics.CALCULATED)
                    wr.Write($"S{delimt}{delimt}");
                else
                    wr.Write($"Error{delimt}{delimt}");
                wr.Write($"Q{delimt}{delimt}");
            }
            else if (this.ProbabilityFunctionTypeId == FrequencyFunctionType.GRAPHICAL)
            {
                wr.Write($"G{delimt}{delimt}");
                if (this.ProbabilityDataTypeId == ProbabilityDataType.DISCHARGE_FREQUENCY)
                    wr.Write($"Q{delimt}{delimt}");
                else if (this.ProbabilityDataTypeId == ProbabilityDataType.STAGE_FREQUENCY)
                    wr.Write($"S{delimt}{delimt}");
                else
                    wr.Write($"Error{delimt}{delimt}");
            }
            else
            {
                wr.Write($"Error{delimt}{delimt}");
                wr.Write($"Error{delimt}{delimt}");
            }
            wr.Write($"{(int)this.UncertTypeSpecification}{delimt}{delimt}");
            wr.Write($"{this.Description}{delimt}{delimt}");
            wr.Write("\n");
            return;
        }
        protected void ExportData(StreamWriter wr, char delimt)
        {
            //Analytical
            if (this.ProbabilityFunctionTypeId == FrequencyFunctionType.ANALYTICAL)
            {
                wr.Write($"{delimt}{delimt}");
                if (this.SourceOfStatisticsId == SourceOfStatistics.ENTERED)
                {
                    wr.Write($"LP3");
                    for (int i = 0; i < 3; i++)
                        wr.Write($"{delimt}{this.MomentsLp3[i]}");
                }
                else if (this.SourceOfStatisticsId == SourceOfStatistics.CALCULATED)
                {
                    wr.Write($"SYN");
                    for (int i = 0; i < 3; i++)
                        wr.Write($"{delimt}{this.PointsSynthetic[i]}");
                }
                else
                {
                    wr.Write("Error");
                }
                wr.Write("\n");
            }
            //Graphical
            else if (this.ProbabilityFunctionTypeId == FrequencyFunctionType.GRAPHICAL)
            {
                wr.Write($"{delimt}{delimt}PROB");
                for (int i = 0; i < this.NumberOfGraphicalPoints; i++)
                    wr.Write($"{delimt}{this.ExceedanceProbability[i]}");
                wr.Write("\n");

                if (this.ProbabilityDataTypeId == ProbabilityDataType.DISCHARGE_FREQUENCY)
                {
                    wr.Write($"{delimt}{delimt}FLOW");
                    for (int i = 0; i < this.NumberOfGraphicalPoints; i++)
                        wr.Write($"{delimt}{Discharge[i]}");
                }
                else if (this.ProbabilityDataTypeId == ProbabilityDataType.STAGE_FREQUENCY)
                {
                    wr.Write($"{delimt}{delimt}STAGE");
                    for (int i = 0; i < this.NumberOfGraphicalPoints; i++)
                        wr.Write($"{delimt}{Stage[i]}");
                }
                wr.Write("\n");
                //User Defined Uncertainty
                if ((int)this.UncertTypeSpecification > 0)
                {
                    switch (this.UncertTypeSpecification)
                    {
                        case UncertaintyTypeSpecification.NORMAL:
                            wr.Write($"{delimt}{delimt}UDFN");
                            for (int i = 0; i < NumberOfGraphicalPoints; i++)
                                wr.Write($"{delimt}{_StdDevNormalUserDef[i]}");
                            break;
                        case UncertaintyTypeSpecification.LOG_NORMAL:
                            wr.Write($"{delimt}{delimt}UDFL");
                            for (int i = 0; i < NumberOfGraphicalPoints; i++)
                                wr.Write($"{delimt}{_StdDevLogUserDef[i]}");
                            break;
                        case UncertaintyTypeSpecification.TRIANGULAR:
                            wr.Write($"{delimt}{delimt}UDFTL");
                            for (int i = 0; i < NumberOfGraphicalPoints; i++)
                                wr.Write($"{delimt}{_StdDevLowerUserDef[i]}");
                            wr.Write($"\n{delimt}{delimt}UDFTU");
                            for (int i = 0; i < NumberOfGraphicalPoints; i++)
                                wr.Write($"{delimt}{_StdDevUpperUserDef[i]}");
                            break;
                        default:
                            break;
                    }
                    wr.Write("\n");
                }
            }
            //Unknown - an Error
            else
            {
            }
            //Transform Flow
            if (this.NumberOfTransFlowPoints > 0)
            {
                wr.Write($"{delimt}{delimt}Qin");
                for (int i = 0; i < NumberOfTransFlowPoints; i++)
                    wr.Write($"{delimt}{TransFlowInflow[i]}");
                wr.Write("\n");

                wr.Write($"{delimt}{delimt}Qout");
                for (int i = 0; i < NumberOfTransFlowPoints; i++)
                    wr.Write($"{delimt}{TransFlowOutflow[i]}");
                wr.Write("\n");

                switch (this.ErrorTypeTransformFlow)
                {
                    case ErrorType.NORMAL:
                        wr.Write($"{delimt}{delimt}QN");
                        for (int i = 0; i < NumberOfTransFlowPoints; i++)
                            wr.Write($"{delimt}{TransFlowStdDev[i]}");
                        break;
                    case ErrorType.LOGNORMAL:
                        wr.Write($"{delimt}{delimt}QL");
                        for (int i = 0; i < NumberOfTransFlowPoints; i++)
                            wr.Write($"{delimt}{TransFlowLogStdDev[i]}");
                        break;
                    case ErrorType.TRIANGULAR:
                        wr.Write($"{delimt}{delimt}QTU");
                        for (int i = 0; i < NumberOfTransFlowPoints; i++)
                            wr.Write($"{delimt}{TransFlowUpper[i]}");
                        wr.Write($"\n{delimt}{delimt}QTL");
                        for (int i = 0; i < NumberOfTransFlowPoints; i++)
                            wr.Write($"{delimt}{TransFlowLower[i]}");
                        break;
                    default:
                        break;
                }
                wr.Write("\n");
            }
            return;
        }

        public void SaveToSqlite()
        {
            if (ProbabilityFunctionTypeId == FrequencyFunctionType.ANALYTICAL)
            {
                if (SourceOfStatisticsId == SourceOfStatistics.ENTERED)
                {
                    //LP3 moments
                    double mean = MomentsLp3[0];
                    double stdDev = MomentsLp3[1];
                    double skew = MomentsLp3[2];
                
                }
                else if (SourceOfStatisticsId == SourceOfStatistics.CALCULATED)
                {
                    //analytical synthetic points
                    double flowPoint5 = PointsSynthetic[0];
                    double flowPoint1 = PointsSynthetic[1];
                    double flowPoint01 = PointsSynthetic[2];

                }
            }
            else if(ProbabilityFunctionTypeId == FrequencyFunctionType.GRAPHICAL)
            {
                //get probabilities
                List<double> probabilities = new List<double>();
                for (int i = 0; i < NumberOfGraphicalPoints; i++)
                {
                    probabilities.Add(ExceedanceProbability[i]);
                }

                if (ProbabilityDataTypeId == ProbabilityDataType.DISCHARGE_FREQUENCY)
                {
                    Write("\t\tDischarge: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        Write($"\t{Discharge[i]}");
                }
                else if (ProbabilityDataTypeId == ProbabilityDataType.STAGE_FREQUENCY)
                {
                    Write("\t\tStage: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        Write($"\t{Stage[i]}");
                }
                //User Defined Uncertainty
                if (UncertTypeSpecification == UncertaintyTypeSpecification.NORMAL)
                {
                    Write("\t\tNormal: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        Write($"\t{_StdDevNormalUserDef[i]}");
                    Write("\n");
                }
                else if (UncertTypeSpecification == UncertaintyTypeSpecification.LOG_NORMAL)
                {
                    Write("\t\tLog Normal: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        Write($"\t{_StdDevLogUserDef[i]}");
                    Write("\n");
                }
                else if (UncertTypeSpecification == UncertaintyTypeSpecification.TRIANGULAR)
                {
                    Write("\t\tTriangular High: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        Write($"\t{_StdDevUpperUserDef[i]}");
                    Write("\n");
                    Write("\t\tTriangular Low: ");
                    for (int i = 0; i < NumberOfGraphicalPoints; i++)
                        Write($"\t{_StdDevLowerUserDef[i]}");
                }
            }


            if (_ProbabilityDataTypeId == ProbabilityDataType.DISCHARGE_FREQUENCY)
            {
                //List<ICoordinate> flowFreqCoords = new List<ICoordinate>();
                //foreach (Pair_xy xy in )
                //{
                //    double x = xy.GetX();
                //    double y = xy.GetY();
                //    flowFreqCoords.Add(ICoordinateFactory.Factory(x, y));
                //}
                //ICoordinatesFunction coordsFunction = ICoordinatesFunctionsFactory.Factory(flowFreqCoords, InterpolationEnum.Linear);
                //ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory()
                //ImpactAreaFunctionFactory.FactoryFrequency(, ImpactAreaFunctionEnum.InflowFrequency);
            }
            


            //i think these are inflow outflow?
            //Transform Flow Function
            if (NumberOfTransFlowPoints > 0)
            {
                WriteLine("\n\tTransform Flow Function");
                Write("\t\tInflow: ");
                for (int i = 0; i < NumberOfTransFlowPoints; i++)
                    Write($"\t{TransFlowInflow[i]}");
                Write("\n");
                Write("\t\tOutflow: ");
                for (int i = 0; i < NumberOfTransFlowPoints; i++)
                    Write($"\t{TransFlowOutflow[i]}");
                Write("\n");
                if (ErrorTypeTransformFlow == ErrorType.NORMAL)
                {
                    Write("\t\tNormal: ");
                    for (int i = 0; i < NumberOfTransFlowPoints; i++)
                        Write($"\t{TransFlowStdDev[i]}");
                    Write("\n");
                }
                else if (ErrorTypeTransformFlow == ErrorType.LOGNORMAL)
                {
                    Write("\t\tLog Normal: ");
                    for (int i = 0; i < NumberOfTransFlowPoints; i++)
                        Write($"\t{TransFlowLogStdDev[i]}");
                    Write("\n");
                }
                else if (ErrorTypeTransformFlow == ErrorType.TRIANGULAR)
                {
                    Write("\t\tTriangular High: ");
                    for (int i = 0; i < NumberOfTransFlowPoints; i++)
                        Write($"\t{ TransFlowUpper[i]}");
                    Write("\n");
                    Write("\t\tTriangular Low: ");
                    for (int i = 0; i < NumberOfTransFlowPoints; i++)
                        Write($"\t{TransFlowLower[i]}");
                    Write("\n");
                }
            }


            //Calculation Points
            if (_NumCalcPoints > 0)
            {
                WriteLine("\n\tCalculation Points");
                Write("\t\tQ05: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    Write($"\t{Calc05[i]}");
                Write("\n");

                Write("\t\tQ25: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    Write($"\t{Calc25[i]}");
                Write("\n");

                Write("\t\tQ50: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    Write($"\t{Calc50[i]}");
                Write("\n");

                Write("\t\tQ75: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    Write($"\t{Calc75[i]}");
                Write("\n");

                Write("\t\tQ95: ");
                for (int i = 0; i < _NumCalcPoints; i++)
                    Write($"\t{Calc95[i]}");
                Write("\n");
            }

        }
        #endregion
        #region Functions
        #endregion
    }
}
