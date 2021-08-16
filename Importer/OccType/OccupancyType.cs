using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;

using Functions;
using ViewModel.Inventory.OccupancyTypes;
using ViewModel.Inventory.DamageCategory;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using ViewModel.Saving.PersistenceManagers;

namespace Importer
{
    [Serializable]
    public class OccupancyType : FdObjectData
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        public ErrorDistribution[] _ErrorDistribution;
        public SingleDamageFunction[] _SingleDamageFunction;
        #endregion
        #region Properties
        public int CategoryId
        { get; set; }
        public string CategoryName
        { get; set; }

        public bool UsesDollar
        { get; set; }

        public double RatioContent
        { get; set; }
        public double RatioOther
        { get; set; }
        public double RatioCar
        { get; set; }
        #endregion
        #region Constructors
        public OccupancyType()
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
            UsesDollar = false;
            RatioContent = 0.0;
            RatioOther = 0.0;
            RatioCar = 0.0;
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
        public void Print()
        {
            WriteLine($"\nOccupancy Name: {this.Name}");
            WriteLine($"\tOccupancy ID: {Id}");
            WriteLine($"\tDescription: {this.Description}");
            WriteLine($"\tCategory Name: {this.CategoryName}");
            WriteLine($"\tCategory ID: {CategoryId}");
            WriteLine($"\tUses Direct Dollar: {this.UsesDollar}");
            WriteLine($"\tContent-To-Structure Value Ratio: {this._ErrorDistribution[2].GetCentralValue()}");
            WriteLine($"\tOther-To-Structure Value Ratio: {this._ErrorDistribution[3].GetCentralValue()}");
            WriteLine($"\tAutomobile-To-Structure Value Ratio: {this._ErrorDistribution[4].GetCentralValue()}");

            //Depth-Damage Functions
            for (int itype = 0; itype < 4; itype++)
            {
                SingleDamageFunction sdf = _SingleDamageFunction[itype];
                int numRows = sdf.GetNumRows();
                if (numRows > 0)
                {
                    double[] depth = sdf.Depth;
                    double[] damage = sdf.Damage;
                    double[] err = null;
                    double[] errLo = null;

                    Write($"\n\n\tType of Function: {(StructureValueType)itype}");

                    Write($"\n\tStage:");
                    for (int i = 0; i < numRows; i++) Write($"\t{depth[i]}");
                    Write($"\n\tDamage:");
                    for (int i = 0; i < numRows; i++) Write($"\t{damage[i]}");
                    switch (sdf.GetTypeError())
                    {
                        case ErrorType.NONE:
                            break;
                        case ErrorType.NORMAL:
                            err = sdf.StdDev;
                            Write($"\n\tNormal Error: ");
                            for (int i = 0; i < numRows; i++) Write($"\t{err[i]}");
                            break;
                        case ErrorType.LOGNORMAL:
                            err = sdf.StdDev;
                            Write($"\n\tLog Normal Error: ");
                            for (int i = 0; i < numRows; i++) Write($"\t{err[i]}");
                            break;
                        case ErrorType.TRIANGULAR:
                            err = sdf.ErrHi;
                            //errLo = sdf.GetTriangularLower();
                            errLo = sdf.StdDev;
                            Write($"\n\tTriangular Upper Error: ");
                            for (int i = 0; i < numRows; i++) Write($"\t{err[i]}");
                            Write($"\n\tTriangular Lower Error: ");
                            for (int i = 0; i < numRows; i++) Write($"\t{errLo[i]}");
                            break;
                        default:
                            break;
                    }
                }
            }
            Write("\n");

            //Structure Information
            //!TODO; Need to export the structure information
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\nOccupancy Name: {this.Name}");
            wr.WriteLine($"\tOccupancy ID: {Id}");
            wr.WriteLine($"\tDescription: {this.Description}");
            wr.WriteLine($"\tCategory Name: {this.CategoryName}");
            wr.WriteLine($"\tCategory ID: {CategoryId}");
            wr.WriteLine($"\tUses Direct Dollar: {this.UsesDollar}");
            wr.WriteLine($"\tContent-To-Structure Value Ratio: {this._ErrorDistribution[2].GetCentralValue()}");
            wr.WriteLine($"\tOther-To-Structure Value Ratio: {this._ErrorDistribution[3].GetCentralValue()}");
            wr.WriteLine($"\tAutomobile-To-Structure Value Ratio: {this._ErrorDistribution[4].GetCentralValue()}");

            //Depth-Damage Functions
            for (int itype = 0; itype < 4; itype++)
            {
                SingleDamageFunction sdf = _SingleDamageFunction[itype];
                int numRows = sdf.GetNumRows();
                if (numRows > 0)
                {
                    double[] depth = sdf.Depth;
                    double[] damage = sdf.Damage;
                    double[] err = null;
                    double[] errLo = null;

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
                            err = sdf.StdDev;
                            wr.Write($"\n\tNormal Error: ");
                            for (int i = 0; i < numRows; i++) wr.Write($"\t{err[i]}");
                            break;
                        case ErrorType.LOGNORMAL:
                            err = sdf.StdDev;
                            wr.Write($"\n\tLog Normal Error: ");
                            for (int i = 0; i < numRows; i++) wr.Write($"\t{err[i]}");
                            break;
                        case ErrorType.TRIANGULAR:
                            err = sdf.ErrHi;
                            //errLo = sdf.GetTriangularLower();
                            errLo = sdf.StdDev;
                            wr.Write($"\n\tTriangular Upper Error: ");
                            for (int i = 0; i < numRows; i++) wr.Write($"\t{err[i]}");
                            wr.Write($"\n\tTriangular Lower Error: ");
                            for (int i = 0; i < numRows; i++) wr.Write($"\t{errLo[i]}");
                            break;
                        default:
                            break;
                    }
                }
            }
            wr.Write("\n");

            //Structure Information
        }
        public void ExportHeader(StreamWriter wr, char delimt)
        {
            //fieldsOcctype = { "OCC_NAME", "OCC_DESCRIPTION", "CAT_NAME", "PARAMETER", "START_DATA" };

            for (int i = 0; i < AsciiImportExport.FieldsOcctype.Length; i++)
            {
                wr.Write($"{ AsciiImportExport.FieldsOcctype[i]}{ delimt}");
            }
            wr.Write("\n");
        }
        public void Export(StreamWriter wr, char delimt)
        {
            //StructureValueType { STRUCTURE, CONTENT, OTHER, CAR, TOTAL };
            //ErrorTypes { NONE, NORMAL, TRIANGULAR, LOGNORMAL, UNIFORM };

            int i = 0;
            string theCode = "";
            double[] depth = null;
            double[] damage = null;
            double[] stdDev = null;
            double[] errHi = null;

            bool fullOutput = true;

            //Process Each Damage Function (Structure, Content, Other, Car
            for (int ixType = 0; ixType < 4; ixType++)
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
                    errHi = sdf.ErrHi;

                    //Process each parameter (stage, damage, stdDev, Triangular Error
                    for (int iparam = 0; iparam < 4; iparam++)
                    {
                        if (errorType == ErrorType.NONE && iparam > 1)
                            continue;
                        if (errorType != ErrorType.TRIANGULAR && iparam > 2)
                            continue;
                        if (fullOutput)
                        {
                            ExportName(fullOutput, wr, delimt);
                            fullOutput = false;
                        }
                        else
                        {
                            ExportName(false, wr, delimt);
                        }
                        switch (iparam)
                        {
                            case 0: //stage
                                wr.Write($"Stage{delimt}");
                                for (i = 0; i < numRows; i++)
                                { wr.Write($"{depth[i]}{delimt}"); }
                                break;
                            case 1: //damage
                                    // theCode = exportGetParamCode(valueType, errorType, iparam, sdf.DirectDollar);
                                theCode = ExportGetParamCode(valueType, errorType, iparam, UsesDollar);
                                wr.Write($"{theCode}{delimt}");
                                for (i = 0; i < numRows; i++)
                                { wr.Write($"{damage[i]}{delimt}"); }
                                break;
                            case 2: //Std Dev
                                theCode = ExportGetParamCode(valueType, errorType, iparam, sdf.DirectDollar);
                                wr.Write($"{theCode}{delimt}");
                                for (i = 0; i < numRows; i++)
                                { wr.Write($"{stdDev[i]}{delimt}"); }
                                break;
                            case 3: //Triangular
                                theCode = ExportGetParamCode(valueType, errorType, iparam, sdf.DirectDollar);
                                wr.Write($"{theCode}{delimt}");
                                for (i = 0; i < numRows; i++)
                                { wr.Write($"{errHi[i]}{delimt}"); }
                                break;
                            default:
                                break;
                        }
                        wr.Write("\n");
                    }
                }
            }
            ExportStrucParameters(wr, delimt);
            return;
        }

        public void ExportTest(StreamWriter wr, char delimt)
        {
            //StructureValueType { STRUCTURE, CONTENT, OTHER, CAR, TOTAL };
            //ErrorTypes { NONE, NORMAL, TRIANGULAR, LOGNORMAL, UNIFORM };

            int i = 0;
            string theCode = "";
            double[] depth = null;
            double[] damage = null;
            double[] stdDev = null;
            double[] errHi = null;

            bool fullOutput = true;

            //Process Each Damage Function (Structure, Content, Other, Car
            for (int ixType = 0; ixType < 4; ixType++)
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
                    errHi = sdf.ErrHi;

                    //Process each parameter (stage, damage, stdDev, Triangular Error
                    for (int iparam = 0; iparam < 4; iparam++)
                    {
                        if (errorType == ErrorType.NONE && iparam > 1)
                            continue;
                        if (errorType != ErrorType.TRIANGULAR && iparam > 2)
                            continue;
                        if (fullOutput)
                        {
                            ExportName(fullOutput, wr, delimt);
                            fullOutput = false;
                        }
                        else
                        {
                            ExportName(false, wr, delimt);
                        }
                        switch (iparam)
                        {
                            case 0: //stage
                                wr.Write($"Stage{delimt}");
                                for (i = 0; i < numRows; i++)
                                { wr.Write($"{depth[i]}{delimt}"); }
                                break;
                            case 1: //damage
                                    // theCode = exportGetParamCode(valueType, errorType, iparam, sdf.DirectDollar);
                                theCode = ExportGetParamCode(valueType, errorType, iparam, UsesDollar);
                                wr.Write($"{theCode}{delimt}");
                                for (i = 0; i < numRows; i++)
                                { wr.Write($"{damage[i]}{delimt}"); }
                                break;
                            case 2: //Std Dev
                                theCode = ExportGetParamCode(valueType, errorType, iparam, sdf.DirectDollar);
                                wr.Write($"{theCode}{delimt}");
                                for (i = 0; i < numRows; i++)
                                { wr.Write($"{stdDev[i]}{delimt}"); }
                                break;
                            case 3: //Triangular
                                theCode = ExportGetParamCode(valueType, errorType, iparam, sdf.DirectDollar);
                                wr.Write($"{theCode}{delimt}");
                                for (i = 0; i < numRows; i++)
                                { wr.Write($"{errHi[i]}{delimt}"); }
                                break;
                            default:
                                break;
                        }
                        wr.Write("\n");
                    }
                    //=======================================================================================
                    //rdc temp;20Aug2018;Testing various code
                    //IFunction<double> theFunc = sdf.getFunctionV2();
                    //int itmp = 1;

                    //IOrdinate<string> testObj = OrdinateFactory.Factory(1, DistributedValueFactory.Factory<string>(new Statistics.Normal(0, 1), DistributionPrinterEnum.String));
                    ////OrdinateDistributedY<string> comparisonObj = new OrdinateDistributedY<string>(2, DistributedValueFactory.Factory<string>(new Statistics.Normal(0, 1), DistributionPrinterEnum.String));
                    //IOrdinate<string> comparisonObj = OrdinateFactory.Factory(2, DistributedValueFactory.Factory<string>(new Statistics.Normal(0,1), DistributionPrinterEnum.String));
                    //Assert.False(testObj.Equals(comparisonObj));

                    //IOrdinate<double> test01 = OrdinateFactory.Factory(1, 2);
                    //IOrdinate<double> test02 = OrdinateFactory.Factory(2, 2);
                    //Assert.False(test01.Equals(test02));

                    //IOrdinate<string> test03 = OrdinateFactory.Factory(0.0, DistributedValueFactory.Factory<string>(new Statistics.Normal(55.0, .4), DistributionPrinterEnum.String));

                    //ContinuousDistribution contDistTri01 = new Statistics.Normal(55.0, 0.4);
                    //IOrdinate<string> test03a = OrdinateFactory.Factory(30.0, DistributedValueFactory.Factory<string>(contDistTri01, DistributionPrinterEnum.String));

                    //double theMin = 50;
                    //double theMax = 70;
                    //double theMostLikely = 55;
                    //double theStdDev = 0.3;

                    //ContinuousDistribution continDist = new Statistics.Normal(theMostLikely, theStdDev);
                    //double thePdf = 0.0;
                    //double theResult = continDist.GetPDF(thePdf);       //Get zero
                    //Type type = continDist.GetType();       //Get zero

                    //String typeAsString = continDist.GetType().Name;

                    //double theMeanOut = ((Normal)continDist).GetMean;
                    //double theStdDevOut = ((Normal)continDist).GetStDev;
                    ////((Normal)continDist).SetParameters(double[] theArrayOfData);
                    //continDist.GetType();
                    //string typeAsString01 = continDist.GetType().Name;
                    //itmp = 2;

                    ////rdc temp;21Aug2018;rdc critical;
                    //ContinuousDistribution contDistTri = new Statistics.Triangular(theMin, theMax, theMostLikely);
                    //String typeContDistTri = contDistTri.GetType().Name;
                    //double mlContDistTri = ((Triangular)contDistTri).GetCentralTendency;
                    //double minContDistTri = ((Triangular)contDistTri).getMin;
                    //double maxContDistTri = ((Triangular)contDistTri).getMax;

                    //IDistribution<string> strTemp01 = DistributedValueFactory.Factory<string>(new Statistics.Triangular(theMin, theMax, theMostLikely), DistributionPrinterEnum.String);
                    //IOrdinate<string> test04 = OrdinateFactory.Factory(3, strTemp01);
                    //string typeTest04 = test04.GetType().Name;

                    ////Function with List of IOrdinate s
                    //IFunction<double> testFuncDouble = FunctionFactory.CreateFunction(new List<IOrdinate<double>>() { OrdinateFactory.Factory(0, 1), OrdinateFactory.Factory(1, 2) });

                    //List < IOrdinate < double >> testFuncIordArray = new List<IOrdinate<double>>();
                    //testFuncIordArray.Add(OrdinateFactory.Factory(0,1));
                    //testFuncIordArray.Add(OrdinateFactory.Factory(1, 2));
                    //int numTestFuncIordArray = testFuncIordArray.Count();

                    //IOrdinate<double>theIOrdReturned = testFuncIordArray.ElementAt<IOrdinate<double>>(0);
                    //Type theTypeOfReturnedObj = theIOrdReturned.GetType();
                    //double theReturnedX = theIOrdReturned.x;
                    //double theReturnedY = theIOrdReturned.y;

                    //testFuncIordArray.Clear();

                    //List<IOrdinate<string>> testFuncIordArray01 = new List<IOrdinate<string>>();
                    ////testFuncIordArray01.Add(OrdinateFactory.Factory(0, 1));

                    //IDistribution<string> test07a = DistributedValueFactory.Factory<string>(new Statistics.Normal(60.0, 0.6), DistributionPrinterEnum.String);
                    //IOrdinate<string> test07 = OrdinateFactory.Factory(1, test07a);
                    //testFuncIordArray01.Add(test07);

                    //testFuncIordArray01.Add(OrdinateFactory.Factory(2, DistributedValueFactory.Factory<string>(new Statistics.Normal(61.0, .61), DistributionPrinterEnum.String)));

                    //IOrdinate<string> theOrdReturned01 = testFuncIordArray01.ElementAt<IOrdinate<string>>(1);
                    //Type theTypeOfReturnedObj01 = theOrdReturned01.GetType();
                    //double theReturnedX01 = theOrdReturned01.x;
                    //string theReturnedY01 = theOrdReturned01.y;




                    //List<IOrdinate<ContinuousDistribution>> testFuncIordArray02 = new List<IOrdinate<ContinuousDistribution>>();
                    //IDistribution<ContinuousDistribution> test08a = DistributedValueFactory.Factory<ContinuousDistribution>(new Statistics.Normal(60.0, 0.6), DistributionPrinterEnum.ContinousDistribution);
                    //IOrdinate<ContinuousDistribution> test08 = OrdinateFactory.Factory(1, test08a);
                    //testFuncIordArray02.Add(test08);

                    //testFuncIordArray02.Add(OrdinateFactory.Factory(2, DistributedValueFactory.Factory<ContinuousDistribution>(new Statistics.Normal(61.0, .61), DistributionPrinterEnum.ContinousDistribution)));

                    //IOrdinate<ContinuousDistribution> theOrdReturned02 = testFuncIordArray02.ElementAt<IOrdinate<ContinuousDistribution>>(0);
                    //string theTypeOfReturnedObj02 = theOrdReturned02.GetType().Name;
                    //double theReturnedX02 = theOrdReturned02.x;
                    //ContinuousDistribution theReturnedY02 = theOrdReturned02.y;

                    //string theTypeOfDist02 = theReturnedY02.GetType().Name;
                    //double meanContDistNorm = ((Normal)theReturnedY02).GetCentralTendency;
                    //double stdDevContDistNorm = ((Normal)theReturnedY02).GetStDev;




                    ////the following does not work
                    //IDistribution<double> dblTemp01 = DistributedValueFactory.Factory<double>(new Statistics.Triangular(theMin, theMax, theMostLikely), DistributionPrinterEnum.ContinousDistribution);
                    ////ContinuousDistribution dblTemp02 = DistributedValueFactory.Factory<double>(new Statistics.Triangular(theMin, theMax, theMostLikely), DistributionPrinterEnum.ContinousDistribution);
                    //IOrdinate<double> test05 = OrdinateFactory.Factory(5, dblTemp01);

                    ////the following does not work
                    //IOrdinate<double> test06 = OrdinateFactory.Factory(2, DistributedValueFactory.Factory<double>(new Statistics.Triangular(theMin, theMax, theMostLikely), DistributionPrinterEnum.String));
                    //itmp = 3;
                    ////End of test section
                    ////=======================================================================================
                }
            }
            ExportStrucParameters(wr, delimt);
            return;
        }
        protected void ExportName(bool fullOutput, StreamWriter wr, char delimt)
        {
            if (fullOutput)
            {
                wr.Write($"{this.Name}{delimt}");
                wr.Write($"{this.Description}{delimt}");
                wr.Write($"{this.CategoryName}{delimt}");
            }
            else
            {
                wr.Write($"{this.Name}{delimt}");
                wr.Write($"{delimt}");
                wr.Write($"{delimt}");
            }
            return;
        }
        protected void ExportStrucParameters(StreamWriter wr, char delimt)
        {
            //Output the error data associated with the structure (first floor, structure, etc.
            ExportName(false, wr, delimt);

            wr.Write("Struct");
            for (int i = 0; i < 5; i++)
            {
                double centralValue = _ErrorDistribution[i].GetCentralValue();
                double stdDev = _ErrorDistribution[i].GetStdDev();
                double upper = _ErrorDistribution[i].GetUpper();

                wr.Write($"{delimt}{_ErrorDistribution[i].GetErrorTypeCode()}");
                if (i > 1 && !Study.IsBadNumber(centralValue))
                    wr.Write($"{delimt}{_ErrorDistribution[i].GetCentralValue()}");
                else
                    wr.Write($"{delimt}");
                if (!Study.IsBadNumber(stdDev))
                    wr.Write($"{delimt}{_ErrorDistribution[i].GetStdDev()}");
                else
                    wr.Write($"{delimt}");
                if (!Study.IsBadNumber(upper))
                    wr.Write($"{delimt}{_ErrorDistribution[i].GetUpper()}");
                else
                    wr.Write($"{delimt}");
            }
            wr.Write("\n");
        }
        #endregion
        #region Functions
        public ErrorDistribution GetErrorDistribution(OccTypeStrucComponent occType)
        {
            if ((int)occType < 0 || (int)occType > 4)
            {
                //error
                return _ErrorDistribution[1];
            }
            else
            {
                return _ErrorDistribution[(int)occType];
            }
        }
        public ErrorDistribution SetErrorDistribution(ErrorDistribution errorDistribution, OccTypeStrucComponent occType)
        {
            if (occType < 0 || (int)occType > 4)
            {
                //error
                return this._ErrorDistribution[1];
            }
            else
            {
                this._ErrorDistribution[(int)occType] = ObjectCopier.Clone(errorDistribution);
                return this._ErrorDistribution[(int)occType];
            }
        }
        public SingleDamageFunction GetSingleDamageFunction(StructureValueType typeValue, SingleDamageFunction singleDamageFunction)
        {
            int itype = (int)typeValue;
            singleDamageFunction = new SingleDamageFunction();
            singleDamageFunction = ObjectCopier.Clone(_SingleDamageFunction[itype]);
            return singleDamageFunction;
        }
        protected string ExportGetParamCode(StructureValueType typeVal, ErrorType typeError, int iparam, bool usesDollar)
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
                default:
                    break;
            }
            switch (iparam)
            {
                case 0: //stage covered elsewhere
                    break;
                case 1: // damage
                    if (usesDollar) theCode += "$";
                    break;
                case 2: // stdDev
                    switch (typeError)
                    {
                        case ErrorType.NONE:
                            break;
                        case ErrorType.NORMAL:
                            theCode += "N";
                            break;
                        case ErrorType.LOGNORMAL:
                            theCode += "L";
                            break;
                        case ErrorType.TRIANGULAR:
                            theCode += "TL";
                            break;
                    }
                    break;
                case 3: // error Triangular
                    theCode += "TU";
                    break;
                default:
                    break;
            }
            return theCode;
        }

        public IOccupancyType GetFDA2OccupancyType()
        {
            List<string> errorMessages = new List<string>();
            //translate from old occtype to new occtype
            IOccupancyType ot = OccupancyTypeFactory.Factory();

            //what do i need for a new ot
            ot.Name = Name;
            ot.Description = Description;
            ot.DamageCategory = DamageCategoryFactory.Factory(CategoryName);

            //the single damage functions will always be in this order
            //public enum StructureValueType { STRUCTURE, CONTENT, OTHER, CAR, TOTAL };
            //this list is in the order of the enum
            List<ICoordinatesFunction> coordFunctions = TranslateSingleDamageFunctionToCoordinatesFunctions(_SingleDamageFunction, errorMessages);
            ot.StructureDepthDamageFunction = coordFunctions[(int)StructureValueType.STRUCTURE];
            ot.ContentDepthDamageFunction = coordFunctions[(int)StructureValueType.CONTENT];
            ot.VehicleDepthDamageFunction = coordFunctions[(int)StructureValueType.CAR];
            ot.OtherDepthDamageFunction = coordFunctions[(int)StructureValueType.OTHER];

            //the error distributions are in the following order:
            //public enum OccTypeStrucComponent { FFLOOR, STRUCTURE, CONTENT, OTHER, AUTO};
            List<IOrdinate> uncertainties = TranslateErrorDistributionsToIOrdinates(_ErrorDistribution);
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


        private List<IOrdinate> TranslateErrorDistributionsToIOrdinates(ErrorDistribution[] errorDists)
        {
            List<IOrdinate> ordinates = new List<IOrdinate>();
            foreach(ErrorDistribution errDist in errorDists)
            {
                ordinates.Add(TranslateErrorDistToOrdinate(errDist));
            }
            return ordinates;
        }

        private IOrdinate TranslateErrorDistToOrdinate(ErrorDistribution errorDist)
        {
            double mean = errorDist.GetCentralValue();
            //st dev gets reused as min
            double stDev = errorDist.GetStdDev();
            double max = errorDist.GetUpper();
            ErrorType type = errorDist.GetErrorType();
            switch(type)
            {
                case ErrorType.NONE:
                    {
                        return new Constant(mean);
                    }
                case ErrorType.NORMAL:
                    {
                        return IDistributedOrdinateFactory.FactoryNormal(mean, stDev);
                    }
                case ErrorType.TRIANGULAR:
                    {
                        return IDistributedOrdinateFactory.FactoryTriangular(mean, stDev, max);
                    }
                case ErrorType.UNIFORM:
                    {
                        return IDistributedOrdinateFactory.FactoryUniform(stDev, max);
                    }
                case ErrorType.LOGNORMAL:
                    {
                        throw new NotImplementedException();
                    }
                default:
                    {
                        //todo: do what
                        //something went wrong, lets just make it a constant?
                        return new Constant(mean);
                    }
            }
        }

        private List<ICoordinatesFunction> TranslateSingleDamageFunctionToCoordinatesFunctions(SingleDamageFunction[] singleDamageFunctions, List<string> errorMessages)
        {
            //the single damage functions will always be in this order
            //public enum StructureValueType { STRUCTURE, CONTENT, OTHER, CAR, TOTAL };
            List<ICoordinatesFunction> coordinatesFunctions = new List<ICoordinatesFunction>();
            //if an occtype fails to read in properly we will make a message and keep trying
            //to import other occtypes.
            for (int i = 0; i < _SingleDamageFunction.Length; i++)
            {
                SingleDamageFunction func = _SingleDamageFunction[i];
                StructureValueType type = (StructureValueType)i;
                ICoordinatesFunction function = null;

                if(IsEmptyFunction(func))
                {
                    //create a function with just (0,0)
                    function = CreateEmptyFunction();
                }
                else
                {
                    function = CreateCoordinatesFunction(func, type, errorMessages);

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

        private ICoordinatesFunction CreateEmptyFunction()
        {
            List<double> xs = new List<double>() { 1 ,2,3};
            List<double> ys = new List<double>() { 1,2,3 };
            return ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
        }

        private bool IsEmptyFunction(SingleDamageFunction function)
        {
            List<double> depths = function.Depth.ToList<double>();
            List<double> damages = function.Damage.ToList<double>();

            for(int i = 0; i<depths.Count;i++)
            {
                double depth = depths[i];
                double damage = damages[i];
                if(depth != 0 || damage != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private ICoordinatesFunction CreateCoordinatesFunction(SingleDamageFunction function, StructureValueType structureValueType, List<string> errors)
        {
            List<double> depths = function.Depth.ToList<double>();
            List<double> damages = function.Damage.ToList<double>();
            //stDevs get reused as min values if the type is triangular
            List<double> stDevs = function.StdDev.ToList<double>();
            List<double> maxValues = function.ErrHi.ToList<double>();
            //todo: should i check the lists are the same size?
            switch(function.GetTypeError())
            {
                case ErrorType.NONE:
                    {
                        return CreateNoneFunction(depths, damages, structureValueType, errors);
                    }
                case ErrorType.NORMAL:
                    {
                        //search for constant values up front (ie: 0,0)
                        FindConstantValuesForNormal(depths, damages, stDevs);
                        return CreateNormalFunction(depths, damages, stDevs, structureValueType, errors);
                    }
                case ErrorType.TRIANGULAR:
                    {
                        return CreateTriangularFunction(depths, damages, stDevs, maxValues, structureValueType, errors);
                    }
                case ErrorType.UNIFORM:
                    {
                        return CreateUniformFunction(depths, stDevs, maxValues, structureValueType, errors);
                    }
                case ErrorType.LOGNORMAL:
                    {
                        throw new NotImplementedException("");
                    }
                default:
                    {
                        errors.Add("Could not create a '" + structureValueType + "' coordinates function for the occupancy type: " + Name + " because '" + structureValueType + "' is" +
                            " not defined in FDA.");
                        return null;
                    }
            }
        }

        /// <summary>
        /// Gets all the indexes from the lists that have constant values. (ie: y = stdev)
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="stDevs"></param>
        /// <returns></returns>
        private List<int> GetIndexesWithConstantValuesForNormal( List<double> ys, List<double> stDevs)
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

        private void FindConstantValuesForNormal(List<double> xs, List<double> ys, List<double> stDevs)
        {
            List<List<int>> masterListOfConstantIndexes = new List<List<int>>();
            List<int> indexOfConstants = GetIndexesWithConstantValuesForNormal( ys, stDevs);

            //so now we have all the indexes of constant values.
            //if there are any then we know we need to do a linked function
            //add the first point to a list
            List<int> currentConstantFunction = new List<int>();
            currentConstantFunction.Add(indexOfConstants[0]);
            for(int i = 0;i<indexOfConstants.Count-1;i++)
            {
                int currentIndex = indexOfConstants[i];
                int nextIndex = indexOfConstants[i + 1];
                if(currentIndex+1 == nextIndex)
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

        private string CreateFailedCoordFunctionErrorMsg(StructureValueType type, string occtypeName, string exceptionMsg)
        {
            return "Could not create a '" + type + "' coordinates function for the occupancy type: " + Name + ". " + exceptionMsg;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="structureValueType">This is just to add more info to the error msg if there is one</param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private ICoordinatesFunction CreateNoneFunction(List<double> xs, List<double> ys, StructureValueType structureValueType, List<string> errors)
        {           
            try
            {
                return ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
            }
            catch(ArgumentException e)
            {
                errors.Add(CreateFailedCoordFunctionErrorMsg(structureValueType, Name, e.Message));
                return null;
            }
        }


        private ICoordinatesFunction CreateNormalFunction(List<double> xs, List<double> ys, List<double> stDevs, StructureValueType structureValueType, List<string> errors)
        {
            List<IDistributedOrdinate> ordinates = new List<IDistributedOrdinate>();
            for (int i = 0; i < xs.Count; i++)
            {
                ordinates.Add( IDistributedOrdinateFactory.FactoryNormal(ys[i], stDevs[i]));
            }
            try
            {
                return ICoordinatesFunctionsFactory.Factory(xs, ordinates, InterpolationEnum.Linear);
            }
            catch(ArgumentException e)
            {
                errors.Add(CreateFailedCoordFunctionErrorMsg(structureValueType, Name, e.Message));
                return null;
            }
        }

        private ICoordinatesFunction CreateTriangularFunction(List<double> xs, List<double> ys, List<double> mins,List<double> maxs, StructureValueType structureValueType, List<string> errors)
        {
            List<IDistributedOrdinate> ordinates = new List<IDistributedOrdinate>();
            for (int i = 0; i < xs.Count; i++)
            {
                ordinates.Add(IDistributedOrdinateFactory.FactoryTriangular(ys[i], mins[i], maxs[i]));
            }
            try
            {
                return ICoordinatesFunctionsFactory.Factory(xs, ordinates, InterpolationEnum.Linear);
            }
            catch (ArgumentException e)
            {
                errors.Add(CreateFailedCoordFunctionErrorMsg(structureValueType, Name, e.Message));
                return null;
            }
        }

        private ICoordinatesFunction CreateUniformFunction(List<double> xs, List<double> mins, List<double> maxs, StructureValueType structureValueType, List<string> errors)
        {
            List<IDistributedOrdinate> ordinates = new List<IDistributedOrdinate>();
            for (int i = 0; i < xs.Count; i++)
            {
                ordinates.Add(IDistributedOrdinateFactory.FactoryUniform(mins[i], maxs[i]));
            }
            try
            {
                return ICoordinatesFunctionsFactory.Factory(xs, ordinates, InterpolationEnum.Linear);
            }
            catch (ArgumentException e)
            {
                errors.Add(CreateFailedCoordFunctionErrorMsg(structureValueType, Name, e.Message));
                return null;
            }
        }

        #endregion
    }
}
