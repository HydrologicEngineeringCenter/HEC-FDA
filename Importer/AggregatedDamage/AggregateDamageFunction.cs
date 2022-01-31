using System;
using System.IO;

namespace Importer
{
    [Serializable]
    public class AggregateDamageFunction : FdObjectDataLook
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

    }
}
