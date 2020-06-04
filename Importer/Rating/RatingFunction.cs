using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;
using FdaViewModel.StageTransforms;
using Functions.CoordinatesFunctions;
using Functions;
using Model;
using Model.Inputs.Functions.ImpactAreaFunctions;
using FdaViewModel.Saving.PersistenceManagers;

namespace Importer
{
    [Serializable]
    public class RatingFunction : FdObjectData, ISaveToSqlite
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private double[] _Discharge = null;
        private double[] _Stage = null;

        //Error by Ordinate - not typical
        private double[] _StdDev = null;
        private double[] _StdDevLog = null;
        private double[] _StdDevHigh = null;
        private double[] _StdDevLow = null;

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
        public ErrorType ErrorTypesId
        { get; set; }
        public bool UsesGlobalError
        { get; set; }
        public int NumberOfPoints
        { get; set; }
        public int NumberOfPointsAllocated
        { get; set; }

        //Global Error - Typical Case
        public double BaseStage
        { get; set; }
        public double GlobalStdDev
        { get; set; }
        public double GlobalStdDevLog
        { get; set; }
        public double GlobalStdDevHigh
        { get; set; }
        public double GlobalStdDevLow
        { get; set; }
        #endregion
        #region Constructors
        public RatingFunction()
        {
            NumberOfPointsAllocated = 20;
            _Discharge = new double[NumberOfPointsAllocated];
            _Stage = new double[NumberOfPointsAllocated];
            _StdDev = new double[NumberOfPointsAllocated];
            _StdDevLog = new double[NumberOfPointsAllocated];
            _StdDevHigh = new double[NumberOfPointsAllocated];
            _StdDevLow = new double[NumberOfPointsAllocated];

            Reset();
        }
        #endregion
        #region Voids
        public new void Reset()
        {
            base.Reset();
            NumberOfPoints = 0;
            UsesGlobalError = true;
            ErrorTypesId = ErrorType.NONE;
            for (int i = 0; i < NumberOfPointsAllocated; i++)
                _Discharge[i] = _Stage[i] = _StdDev[i] = _StdDevLog[i] = _StdDevHigh[i] = _StdDevLow[i] = Study.badNumber;
            BaseStage = GlobalStdDev = GlobalStdDevLog = GlobalStdDevHigh = GlobalStdDevLow = Study.badNumber;

            return;
        }
        public void ReallocateRatingWithCheckAndSave(int numOrds)
        {
            if (numOrds > NumberOfPointsAllocated)
            {
                double[] discharge = new double[numOrds];
                double[] stage = new double[numOrds];
                double[] stdDev = new double[numOrds];
                double[] stdDevLog = new double[numOrds];
                double[] stdDevHigh = new double[numOrds];
                double[] stdDevLow = new double[numOrds];
                for (int i = 0; i < numOrds; i++)
                    discharge[i] = stage[i] = stdDev[i] = stdDevLog[i] = stdDevHigh[i] = stdDevLow[i] = Study.badNumber;
                for (int i = 0; i < NumberOfPoints; i++)
                {
                    discharge[i] = this._Discharge[i];
                    stage[i] = this._Stage[i];
                    stdDev[i] = this._StdDev[i];
                    stdDevLog[i] = this._StdDevLog[i];
                    stdDevHigh[i] = this._StdDevHigh[i];
                    stdDevLow[i] = this._StdDevLow[i];
                }
                NumberOfPointsAllocated = numOrds;
                this._Discharge = discharge;
                this._Stage = stage;
                this._StdDev = stdDev;
                this._StdDevLog = stdDevLog;
                this._StdDevHigh = stdDevHigh;
                this._StdDevLow = stdDevLow;
            }
            return;
        }
        public void Print()
        {
            //Basic Information
            WriteLine($"\n\nStage-Discharge Function Name: {Name}");
            WriteLine($"\tDescription: {Description}");
            WriteLine($"\tPlan: {PlanName}");
            WriteLine($"\tYear: {YearName}");
            WriteLine($"\tStream: {StreamName}");
            WriteLine($"\tReach: {DamageReachName}");
            WriteLine($"\tError Type: {ErrorTypesId}");
            WriteLine($"\tUses Global Error: {UsesGlobalError}");

            if (UsesGlobalError)
            {
                WriteLine($"\tBase Stage: {BaseStage}");
                if (ErrorTypesId == ErrorType.NORMAL)
                    WriteLine($"\tStd Dev: {GlobalStdDev}");
                else if (ErrorTypesId == ErrorType.LOGNORMAL)
                    WriteLine($"\tLog Std Dev: {GlobalStdDevLog}");
                else if (ErrorTypesId == ErrorType.TRIANGULAR)
                    WriteLine($"\tUpper Error: {GlobalStdDevHigh}\n\tLower Error: {GlobalStdDevLow}");
            }
            //Points
            WriteLine($"\n\tRating Curve, Number of Points {NumberOfPoints}");
            Write("\t\tDischarge: ");
            for (int i = 0; i < NumberOfPoints; i++)
                Write($"\t{_Discharge[i]}");
            Write("\n\t\tStage: ");
            for (int i = 0; i < NumberOfPoints; i++)
                Write($"\t{_Stage[i]}");
            Write("\n");

            //Ordinate by Ordinate errors
            if (!UsesGlobalError)
            {
                if (ErrorTypesId == ErrorType.NORMAL)
                {
                    Write("\t\tStd Dev:");
                    for (int i = 0; i < NumberOfPoints; i++)
                        Write($"\t{_StdDev[i]}");
                    Write("\n");
                }
                else if (ErrorTypesId == ErrorType.LOGNORMAL)
                {
                    Write("\t\tLog Std Dev: ");
                    for (int i = 0; i < NumberOfPoints; i++)
                        WriteLine($"\t{_StdDevLog[i]}");
                    Write("\n");
                }
                else if (ErrorTypesId == ErrorType.TRIANGULAR)
                {
                    Write("\t\tUpper Error:");
                    for (int i = 0; i < NumberOfPoints; i++)
                        Write($"\t{_StdDevHigh[i]}");
                    Write("\n\t\tLower Error:");
                    for (int i = 0; i < NumberOfPoints; i++)
                        Write($"\t{_StdDevLow[i]}");
                    Write("\n");
                }
            }
            return;
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            //Basic Information
            wr.WriteLine($"\n\nStage-Discharge Function Name: {Name}");
            wr.WriteLine($"\tDescription: {Description}");
            wr.WriteLine($"\tPlan: {PlanName}");
            wr.WriteLine($"\tYear: {YearName}");
            wr.WriteLine($"\tStream: {StreamName}");
            wr.WriteLine($"\tReach: {DamageReachName}");
            wr.WriteLine($"\tError Type: {ErrorTypesId}");
            wr.WriteLine($"\tUses Global Error: {UsesGlobalError}");

            if (UsesGlobalError)
            {
                wr.WriteLine($"\tBase Stage: {BaseStage}");
                if (ErrorTypesId == ErrorType.NORMAL)
                    wr.WriteLine($"\tStd Dev: {GlobalStdDev}");
                else if (ErrorTypesId == ErrorType.LOGNORMAL)
                    wr.WriteLine($"\tLog Std Dev: {GlobalStdDevLog}");
                else if (ErrorTypesId == ErrorType.TRIANGULAR)
                    wr.WriteLine($"\tUpper Error: {GlobalStdDevHigh}\n\tLower Error: {GlobalStdDevLow}");
            }
            //Points
            wr.WriteLine($"\n\tRating Curve, Number of Points {NumberOfPoints}");
            wr.Write("\t\tDischarge: ");
            for (int i = 0; i < NumberOfPoints; i++)
                wr.Write($"\t{_Discharge[i]}");
            wr.Write("\n\t\tStage: ");
            for (int i = 0; i < NumberOfPoints; i++)
                wr.Write($"\t{_Stage[i]}");
            wr.Write("\n");

            //Ordinate by Ordinate errors
            if (!UsesGlobalError)
            {
                if (ErrorTypesId == ErrorType.NORMAL)
                {
                    wr.Write("\t\tStd Dev:");
                    for (int i = 0; i < NumberOfPoints; i++)
                        wr.Write($"\t{_StdDev[i]}");
                    wr.Write("\n");
                }
                else if (ErrorTypesId == ErrorType.LOGNORMAL)
                {
                    wr.Write("\t\tLog Std Dev: ");
                    for (int i = 0; i < NumberOfPoints; i++)
                        wr.WriteLine($"\t{_StdDevLog[i]}");
                    wr.Write("\n");
                }
                else if (ErrorTypesId == ErrorType.TRIANGULAR)
                {
                    wr.Write("\t\tUpper Error:");
                    for (int i = 0; i < NumberOfPoints; i++)
                        wr.Write($"\t{_StdDevHigh[i]}");
                    wr.Write("\n\t\tLower Error:");
                    for (int i = 0; i < NumberOfPoints; i++)
                        wr.Write($"\t{_StdDevLow[i]}");
                    wr.Write("\n");
                }
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
            for (int i = 0; i < AsciiImportExport.FieldsRatingFunc.Length; i++)
                wr.Write($"{AsciiImportExport.FieldsRatingFunc[i]}{delimt}{delimt}");
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

            char[] errCode = { ' ', 'N', 'L', 'T', 'U' };

            wr.Write($"{errCode[(int)this.ErrorTypesId]}{delimt}{delimt}");

            if (UsesGlobalError)
            {
                wr.Write($"{this.BaseStage}{delimt}{delimt}");
                if (ErrorTypesId == ErrorType.NORMAL)
                    wr.Write($"{this.GlobalStdDev}{delimt}{delimt}");
                else
                    wr.Write($"{delimt}{delimt}");
                if (ErrorTypesId == ErrorType.LOGNORMAL)
                    wr.Write($"{this.GlobalStdDevLog}{delimt}{delimt}");
                else
                    wr.Write($"{delimt}{delimt}");

                if (ErrorTypesId == ErrorType.TRIANGULAR)
                {
                    wr.Write($"{this.GlobalStdDevLow}{delimt}{delimt}");
                    wr.Write($"{this.GlobalStdDevHigh}{delimt}{delimt}");
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                        wr.Write($"{delimt}{delimt}");
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                    wr.Write($"{delimt}{delimt}");
            }
            wr.WriteLine($"{this.Description}");

            return;
        }
        protected void ExportData(StreamWriter wr, char delimt)
        {
            //char* codeData[] = { "Q", "S", "SN", "SL", "STL", "STU" };

            wr.Write($"{delimt}{delimt}Q");
            for (int i = 0; i < NumberOfPoints; i++)
                wr.Write($"{delimt}{this._Discharge[i]}");
            wr.Write($"\n{delimt}{delimt}S");
            for (int i = 0; i < NumberOfPoints; i++)
                wr.Write($"{delimt}{_Stage[i]}");
            wr.Write("\n");
            //Errors by ordinate
            if (!UsesGlobalError)
            {
                switch (ErrorTypesId)
                {
                    case ErrorType.NONE:
                        //None, no output
                        break;
                    case ErrorType.NORMAL:
                        //Normal
                        wr.Write($"{delimt}{delimt}SN");
                        for (int i = 0; i < NumberOfPoints; i++)
                            wr.Write($"{delimt}{this._StdDev[i]}");
                        wr.Write("\n");
                        break;
                    case ErrorType.LOGNORMAL:
                        //Log Normal
                        wr.Write($"{delimt}{delimt}SL");
                        for (int i = 0; i < NumberOfPoints; i++)
                            wr.Write($"{delimt}{this._StdDevLog[i]}");
                        wr.Write("\n");
                        break;
                    case ErrorType.TRIANGULAR:
                        //Triangular
                        wr.Write($"{delimt}{delimt}STL");
                        for (int i = 0; i < NumberOfPoints; i++)
                            wr.Write($"{delimt}{this._StdDevLow[i]}");
                        wr.Write("\n");
                        wr.Write($"{delimt}{delimt}STU");
                        for (int i = 0; i < NumberOfPoints; i++)
                            wr.Write($"{delimt}{this._StdDevHigh[i]}");
                        wr.Write("\n");
                        break;
                    default:
                        break;
                }
            }
            return;
        }
        public void SetDischarge(int numPoints, double[] discharge)
        {
            this.ReallocateRatingWithCheckAndSave(numPoints);
            this.NumberOfPoints = numPoints;
            for (int i = 0; i < numPoints; i++) this._Discharge[i] = discharge[i];
        }
        public void SetStage(int numPoints, double[] stage)
        {
            this.ReallocateRatingWithCheckAndSave(numPoints);
            this.NumberOfPoints = numPoints;
            for (int i = 0; i < numPoints; i++) this._Stage[i] = stage[i];
        }
        public void SetStdDev(int numPoints, double[] stdDev)
        {
            ReallocateRatingWithCheckAndSave(numPoints);
            this.NumberOfPoints = numPoints;
            for (int i = 0; i < numPoints; i++) this._StdDev[i] = stdDev[i];
        }
        public void SetStdDevLog(int numPoints, double[] stdDevLog)
        {
            ReallocateRatingWithCheckAndSave(numPoints);
            this.NumberOfPoints = numPoints;
            for (int i = 0; i < numPoints; i++) this._StdDevLog[i] = stdDevLog[i];
        }
        public void SetStdDevHigh(int numPoints, double[] stdDevHigh)
        {
            ReallocateRatingWithCheckAndSave(numPoints);
            this.NumberOfPoints = numPoints;
            for (int i = 0; i < numPoints; i++) this._StdDevHigh[i] = stdDevHigh[i];
        }
        public void SetStdDevLow(int numPoints, double[] stdDevLow)
        {
            ReallocateRatingWithCheckAndSave(numPoints);
            this.NumberOfPoints = numPoints;
            for (int i = 0; i < numPoints; i++) this._StdDevLow[i] = stdDevLow[i];
        }
        #endregion
        #region Functions
        public double[] GetDischarge()
        {
            return _Discharge;
        }
        public double[] GetStage()
        {
            return _Stage;
        }
        #endregion

        public void SaveToSqlite()
        {
            double[] stages = GetStage();
            double[] flows = GetDischarge();
            //these arrays might have a bunch of "Study.badNumber" (-901). I need to get rid of them by only grabbing the correct number of points.
            List<double> stagesList = new List<double>();
            List<double> flowsList = new List<double>();
            for(int i = 0;i<NumberOfPoints;i++)
            {
                stagesList.Add(stages[i]);
                flowsList.Add(flows[i]);
            }
            //always use linear. This is the only option in Old Fda.
            ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(stagesList, flowsList, InterpolationEnum.Linear);
            IFdaFunction rating = ImpactAreaFunctionFactory.Factory(func, IFdaFunctionEnum.Rating);
            RatingCurveElement elem = new RatingCurveElement(Name,CalculationDate,Description,rating);
            RatingElementPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetRatingManager();
            manager.SaveNew(elem);
        }

    }
}
