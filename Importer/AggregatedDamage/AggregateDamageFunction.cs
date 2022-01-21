using System;
using System.Collections.Generic;
using System.IO;
using static System.Console;

namespace Importer
{
    [Serializable]
    public class AggregateDamageFunction : FdObjectDataLook, ISaveToSqlite
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private ErrorDistribution[] _ErrorDistribution;
        private SingleDamageFunction[] _SingleDamageFunction;
        #endregion
        #region Properties
        public SingleDamageFunction[] DamageFunctions
        {
            get { return _SingleDamageFunction; }
            set { _SingleDamageFunction = value; }
        }
        public int CategoryId
        { get; set; }
        public string CategoryName
        { get; set; }
        #endregion
        #region Constructors
        public AggregateDamageFunction()
        {
            _ErrorDistribution = new ErrorDistribution[5];
            for (int i = 0; i < 5; i++) _ErrorDistribution[i] = new ErrorDistribution();

            _SingleDamageFunction = new SingleDamageFunction[5];
            for (int i = 0; i < 5; i++) _SingleDamageFunction[i] = new SingleDamageFunction();
        }
        #endregion
        #region Voids
        public new void Reset()
        {
            base.Reset();
            CategoryId = -1;
            CategoryName = "";
            for (int i = 0; i < 5; i++)
            {
                _ErrorDistribution[i].Reset();
                _SingleDamageFunction[i].Reset();
            }
        }
        public void SetSingleDamageFunction(StructureValueType typeValue, SingleDamageFunction singleDamageFunction)
        {
            int itype = (int)typeValue;
            _SingleDamageFunction[itype] = ObjectCopier.Clone(singleDamageFunction);
        }
        public void Print(AsyncLogger logger)
        {
            logger.Log($"\nAggregatted Damage Function Name: {this.Name}");
            logger.Log($"\tDescription: {this.Description}");
            logger.Log($"\tCategory Name: {this.CategoryName}");

            //Depth-Damage Functions
            //for (int itype = (int)StructureValueType.STRUCTURE; itype <= (int)StructureValueType.TOTAL; itype++)
            //{
            //    SingleDamageFunction sdf = _SingleDamageFunction[itype];
            //    int numRows = sdf.GetNumRows();
            //    if (numRows > 0)
            //    {
            //        double[] depth = sdf.Depth;
            //        double[] damage = sdf.Damage;
            //        double[] stdDev = null;

            //        logger.Log($"\n\n\tType of Function: {(StructureValueType)itype}");

            //        logger.Log($"\n\tStage:");
            //        for (int i = 0; i < numRows; i++) logger.Append($"{depth[i]}, ");
            //        logger.Log($"\n\tDamage:");
            //        for (int i = 0; i < numRows; i++) logger.Append($"\t{damage[i]}, ");
            //        switch (sdf.GetTypeError())
            //        {
            //            case ErrorType.NONE:
            //                break;
            //            case ErrorType.NORMAL:
            //                stdDev = sdf.StdDev;
            //                logger.Log($"\n\tNormal Error: ");
            //                for (int i = 0; i < numRows; i++) logger.Append($"{stdDev[i]}, ");
            //                break;
            //            default:
            //                break;
            //        }
            //    }
            //}
            //logger.Log("\n");
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\nAggregatted Damage Function Name: {this.Name}");
            wr.WriteLine($"\tDescription: {this.Description}");
            wr.WriteLine($"\tCategory Name: {this.CategoryName}");

            //Depth-Damage Functions
            for (int itype = (int)StructureValueType.STRUCTURE; itype <= (int)StructureValueType.TOTAL; itype++)
            {
                SingleDamageFunction sdf = _SingleDamageFunction[itype];
                int numRows = sdf.GetNumRows();
                if (numRows > 0)
                {
                    double[] depth = sdf.Depth;
                    double[] damage = sdf.Damage;
                    double[] stdDev = null;

                    wr.Write($"\n\n\tType of Function: {(StructureValueType)itype}");

                    wr.Write($"\n\tStage:");
                    for (int i = 0; i < numRows; i++) wr.Write($"\t{depth[i]}");
                    wr.Write($"\n\tDamage:");
                    for (int i = 0; i < numRows; i++) wr.Write($"\t{damage[i]}");
                    switch (sdf.GetTypeError())
                    {
                        case ErrorType.NONE:
                            break;
                        case ErrorType.NORMAL:
                            stdDev = sdf.StdDev;
                            wr.Write($"\n\tNormal Error: ");
                            for (int i = 0; i < numRows; i++) wr.Write($"\t{stdDev[i]}");
                            break;
                        default:
                            break;
                    }
                }
            }
            wr.Write("\n");
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
            for (int i = 0; i < AsciiImportExport.FieldsAggDamgFunc.Length; i++)
                wr.Write($"{AsciiImportExport.FieldsAggDamgFunc[i]}{delimt}{delimt}");
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
            wr.Write($"{this.CategoryName}{delimt}{delimt}");
            wr.WriteLine($"{this.Description}");
            return;
        }
        public void ExportData(StreamWriter wr, char delimt)
        {
            //StructureValueType { STRUCTURE, CONTENT, OTHER, CAR, TOTAL };
            //ErrorTypes { NONE, NORMAL, TRIANGULAR, LOGNORMAL, UNIFORM };

            int i = 0;
            string theCode = "";
            double[] depth = null;
            double[] damage = null;
            double[] stdDev = null;

            bool fullOutput = true;

            //Process Each Damage Function (Structure, Content, Other, Car, Total
            for (int ixType = (int)StructureValueType.STRUCTURE; ixType <= (int)StructureValueType.TOTAL; ixType++)
            {
                StructureValueType valueType = (StructureValueType)ixType;
                SingleDamageFunction sdf = new SingleDamageFunction();

                sdf = ObjectCopier.Clone(_SingleDamageFunction[ixType]);

                //this.getSingleDamageFunction((StructureValueType)ixType, sdf);//Doesn't work

                if (sdf == null)
                {
                    continue;
                }
                else if (sdf.GetNumRows() < 1)
                {
                    continue;
                }
                else
                {
                    ErrorType errorType = sdf.GetTypeError();
                    int numRows = sdf.GetNumRows();
                    depth = sdf.Depth;
                    damage = sdf.Damage;
                    stdDev = sdf.StdDev;

                    //Process each parameter (stage, damage, stdDev for this type of damage
                    for (int iparam = 0; iparam < 3; iparam++)
                    {
                        if (errorType == ErrorType.NONE && iparam > 1)
                            continue;
                        if (fullOutput)
                        {
                            //exportName(fullOutput, wr, delimt);
                            fullOutput = false;
                        }
                        else
                        {
                            //exportName(false, wr, delimt);
                        }
                        switch (iparam)
                        {
                            case 0: //stage
                                theCode = ExportGetParamCode(valueType, errorType, iparam);
                                wr.Write($"{theCode}{delimt}");
                                for (i = 0; i < numRows; i++)
                                { wr.Write($"{depth[i]}{delimt}"); }
                                break;
                            case 1: //damage
                                theCode = ExportGetParamCode(valueType, errorType, iparam);
                                wr.Write($"{theCode}{delimt}");
                                for (i = 0; i < numRows; i++)
                                { wr.Write($"{damage[i]}{delimt}"); }
                                break;
                            case 2: //Std Dev
                                theCode = ExportGetParamCode(valueType, errorType, iparam);
                                wr.Write($"{theCode}{delimt}");
                                for (i = 0; i < numRows; i++)
                                { wr.Write($"{stdDev[i]}{delimt}"); }
                                break;
                            default:
                                break;
                        }
                        wr.Write("\n");
                    }
                }
            }
            return;
        }
        #endregion
        #region Functions
        public ErrorDistribution GetErrorDistribution(StructureValueType funcType)
        {
            if (funcType < StructureValueType.STRUCTURE || funcType > StructureValueType.TOTAL)
            {
                //error
                return _ErrorDistribution[1];
            }
            else
            {
                return _ErrorDistribution[(int)funcType];
            }
        }
        public ErrorDistribution SetErrorDistribution(ErrorDistribution errorDistribution, StructureValueType funcType)
        {
            if (funcType < StructureValueType.STRUCTURE || funcType > StructureValueType.TOTAL)
            {
                //error
                return this._ErrorDistribution[1];
            }
            else
            {
                this._ErrorDistribution[(int)funcType] = ObjectCopier.Clone(errorDistribution);
                return this._ErrorDistribution[(int)funcType];
            }
        }
        public SingleDamageFunction GetSingleDamageFunction(StructureValueType typeValue, SingleDamageFunction singleDamageFunction)
        {
            int itype = (int)typeValue;
            singleDamageFunction = ObjectCopier.Clone(_SingleDamageFunction[itype]);
            return singleDamageFunction;
        }
        public SingleDamageFunction GetSingleDamageFunction(StructureValueType typeValue)
        {
            int itype = (int)typeValue;
            return this._SingleDamageFunction[itype];
        }
        protected string ExportGetParamCode(StructureValueType typeVal, ErrorType typeError, int iparam)
        {
            string theCode = "";
            switch (typeVal)
            {
                case StructureValueType.STRUCTURE:
                    theCode = "S";
                    //if (iparam == 1 && usesDollar) theCode += "$";
                    break;
                case StructureValueType.CONTENT:
                    theCode = "C";
                    break;
                case StructureValueType.OTHER:
                    theCode = "O";
                    break;
                case StructureValueType.CAR:
                    theCode = "A";
                    break;
                case StructureValueType.TOTAL:
                    theCode = "T";
                    break;
                default:
                    break;
            }
            switch (iparam)
            {
                case 0: //stage 
                    theCode += "STAGE";
                    break;
                case 1: // damage
                    break;
                case 2: // stdDev
                    switch (typeError)
                    {
                        case ErrorType.NONE:
                            break;
                        case ErrorType.NORMAL:
                            theCode += "N";
                            break;
                    }
                    break;
                default:
                    break;
            }
            return theCode;
        }
        #endregion


        public void SaveToSqlite()
        {
            ////todo: can we be guaranteed that there will always be 5 curves here?
            ////and will they always be in this order?
            ////StructureValueType { STRUCTURE, CONTENT, OTHER, CAR, TOTAL };

            //bool hasAssetCar = false;
            //bool hasAssetOther = false;
            //bool hasAssetContent = false;
            //bool hasAssetDamage = false;

            //SingleDamageFunction totalDamageFunc = _SingleDamageFunction[(int)StructureValueType.TOTAL];
            //SingleDamageFunction carDamageFunc = _SingleDamageFunction[(int)StructureValueType.CAR];
            //SingleDamageFunction otherDamageFunc = _SingleDamageFunction[(int)StructureValueType.OTHER];
            //SingleDamageFunction contentDamageFunc = _SingleDamageFunction[(int)StructureValueType.CONTENT];
            //SingleDamageFunction structDamageFunc = _SingleDamageFunction[(int)StructureValueType.STRUCTURE];


            ////right now we will only use the total in FDA 2.0 but will write the other curves
            ////out so that the user can view them down the road.
            //StageDamagePersistenceManager manager = ViewModel.Saving.PersistenceFactory.GetStageDamageManager();
            //SaveTotalFunction(totalDamageFunc,manager);
            ////todo: i am using Name here is that correct, or do i need to add the PYSR stuff to the front of the name?
            //if (carDamageFunc.GetNumRows() > 0)
            //{
            //    SaveAssetFunction(carDamageFunc, Name, GetAssetTypeFromStructureValueType(StructureValueType.CAR), manager);
            //}
            //if (otherDamageFunc.GetNumRows() > 0)
            //{
            //    SaveAssetFunction(otherDamageFunc, Name, GetAssetTypeFromStructureValueType(StructureValueType.OTHER), manager);
            //}
            //if (contentDamageFunc.GetNumRows() > 0)
            //{
            //    SaveAssetFunction(contentDamageFunc, Name, GetAssetTypeFromStructureValueType(StructureValueType.CONTENT), manager);
            //}
            //if (structDamageFunc.GetNumRows() > 0)
            //{
            //    SaveAssetFunction(structDamageFunc, Name, GetAssetTypeFromStructureValueType(StructureValueType.STRUCTURE), manager);
            //}

        }

        //private StageDamageAssetType GetAssetTypeFromStructureValueType(StructureValueType valueType)
        //    {
        //        switch(valueType)
        //        {
        //            case StructureValueType.CAR:
        //                {
        //                    return StageDamageAssetType.CAR;
        //                }
        //            case StructureValueType.CONTENT:
        //                {
        //                    return StageDamageAssetType.CONTENT;
        //                }
        //            case StructureValueType.OTHER:
        //                {
        //                    return StageDamageAssetType.OTHER;
        //                }
        //            case StructureValueType.STRUCTURE:
        //                {
        //                    return StageDamageAssetType.STRUCTURE;
        //                }
        //            case StructureValueType.TOTAL:
        //                {
        //                    return StageDamageAssetType.TOTAL;
        //                }
        //            default:
        //                {
        //                    throw new ArgumentException("Could not translate the StructureValueType to a StageDamageAssetType.");
        //                }

        //        }
        //    }

        //private void SaveTotalFunction(SingleDamageFunction sdf, StageDamagePersistenceManager manager)
        //{
        //    AggregatedStageDamageElement elem = CreateStageDamageElement(sdf);
        //    manager.SaveNewElement(elem);
        //}

        //private void SaveAssetFunction(SingleDamageFunction sdf, string nameOfTotalFunc, StageDamageAssetType type, StageDamagePersistenceManager manager)
        //{
        //    AggregatedStageDamageElement elem = CreateStageDamageElement(sdf);
        //    manager.SaveAssetCurve(elem, type, nameOfTotalFunc);
        //}

        //private AggregatedStageDamageElement CreateStageDamageElement(SingleDamageFunction sdf)
        //{
        //    double[] depths = sdf.Depth;
        //    double[] damages = sdf.Damage;

        //    //these arrays might have a bunch of "Study.badNumber" (-901). I need to get rid of them by only grabbing the correct number of points.
        //    List<double> depthsList = new List<double>();
        //    List<double> damagesList = new List<double>();
        //    for (int i = 0; i < sdf.GetNumRows(); i++)
        //    {
        //        depthsList.Add(depths[i]);
        //        damagesList.Add(damages[i]);
        //    }
        //    //always use linear. This is the only option in Old Fda.
        //    ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(depthsList, damagesList, InterpolationEnum.Linear);
        //    //IFdaFunction stageDamage = IFdaFunctionFactory.Factory( IParameterEnum.InteriorStageDamage, (IFunction)func);
        //    List<StageDamageCurve> curves = new List<StageDamageCurve>();
        //    ImpactAreaRowItem ri = new ImpactAreaRowItem(-1, "testImpactArea");
        //    StageDamageCurve curve = new StageDamageCurve(ri, "testDamCat", func);
        //    curves.Add(curve);
        //    return new AggregatedStageDamageElement(Name, CalculationDate, Description, -1,-1, curves, true);
        //}

    }
}
