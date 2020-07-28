using ClosedXML.Excel;
using Functions;
using Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Sdk;

namespace ModelTests.ExcelTesting
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]

    public class ResultDataAttribute : ExcelDataAttributeBase
    {
        //name, func
        private Dictionary<string, IFunction> _FunctionsDictionary = new Dictionary<string, IFunction>();

        protected override List<Type> ColumnTypes { get; set; }
        protected override List<DataLength> ColumnDataLengths { get; set; }
        protected override List<int> ColumnIndices { get; set; }
        protected override int ColumnToWriteResults { get; set; }


        public ResultDataAttribute(string fileName, int[] worksheetNumbers) : base(fileName, worksheetNumbers)
        {
            ColumnTypes = new List<Type>()
            {
                 typeof(double), typeof(string), typeof(double), typeof(string),
                typeof(double),typeof(string),typeof(double),typeof(string),typeof(double),
                typeof(string),typeof(double),typeof(string),typeof(string), typeof(double),
                typeof(double),typeof(double),typeof(double)
            };
            ColumnDataLengths = new List<DataLength>
            {
                 DataLength.VariableLength, DataLength.FirstLineOnly, DataLength.VariableLength, DataLength.FirstLineOnly,
                 DataLength.VariableLength,DataLength.FirstLineOnly, DataLength.VariableLength,DataLength.FirstLineOnly,
                 DataLength.VariableLength,DataLength.FirstLineOnly, DataLength.VariableLength,DataLength.FirstLineOnly,
                 DataLength.VariableLength, DataLength.VariableLength,
                DataLength.VariableLength,DataLength.VariableLength,DataLength.VariableLength
            };
            ColumnIndices = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,16,17 };
            ColumnToWriteResults = 18;
        }

        public override IEnumerable<object[]> DataSource(string fileName)
        {
            string filePath = GetApplicationRoot() + "\\" + fileName;
            var workbook = new XLWorkbook(filePath);

            //load all the functions
            // i guess i will just assume that the first worksheet is the tests and
            //the others are the functions?
            LoadFunctionsFromWorksheet(workbook, 2);
            LoadFunctionsFromWorksheet(workbook, 3);
            LoadFunctionsFromWorksheet(workbook, 4);
            LoadFunctionsFromWorksheet(workbook, 5);
            LoadFunctionsFromWorksheet(workbook, 6);
            LoadFunctionsFromWorksheet(workbook, 7);




            List<object[]> tests = new List<object[]>();
            if (MultipleWorksheets)
            {
                foreach (int wsNumber in Worksheets)
                {
                    List<object[]> testData = GetTestsForWorksheet(workbook, wsNumber);

                    foreach (object[] test in testData)
                    {
                        object[] individualTest = ConvertFunctionDataAndTestDataIntoUnitTestParams(test);
                        tests.Add(individualTest);
                    }

                }
            }
            else
            {
                tests.AddRange(GetTestsForWorksheet(workbook, WorksheetNumber));
            }
            return tests;
        }

        /// <summary>
        /// This is the test data from the excel document, and the list of all the functions from the excel document. 
        /// I return an object array that matches the parameter types in the unit test.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="testData"></param>
        /// <returns></returns>
        private object[] ConvertFunctionDataAndTestDataIntoUnitTestParams( object[] testData)
        {
            object[] retval = new object[20];

            List<double> flowFreqProbs = (List<double>)testData[0];
            string flowFreqName = (string)testData[1];

            List<double> inOutProbs = (List<double>)testData[2];
            string inOutName = (string)testData[3];

            List<double> ratingProbs = (List<double>)testData[4];
            string ratingName = (string)testData[5];

            List<double> extIntProbs = (List<double>)testData[6];
            string extIntName = (string)testData[7];

            List<double> failureProbs = (List<double>)testData[8];
            string failureName = (string)testData[9];

            List<double> stageDamageProbs = (List<double>)testData[10];
            string stageDamageName = (string)testData[11];

            //assign the functions. They will either be there or be null
            IFunction flowFreq = getFunctionFromName(flowFreqName);
            IFdaFunction ffFunc = IFdaFunctionFactory.Factory(flowFreq, IParameterEnum.InflowFrequency);

            IFunction inflowOutflow = getFunctionFromName(inOutName);
            IFdaFunction iOFunc = IFdaFunctionFactory.Factory(inflowOutflow, IParameterEnum.InflowOutflow);

            IFunction rating = getFunctionFromName(ratingName);
            IFdaFunction ratFunc = IFdaFunctionFactory.Factory(rating, IParameterEnum.Rating);

            IFunction extInt = getFunctionFromName(extIntName);
            IFdaFunction eIFunc = IFdaFunctionFactory.Factory(extInt, IParameterEnum.ExteriorInteriorStage);

            IFunction failure = getFunctionFromName(failureName);
            IFdaFunction failureFunc = IFdaFunctionFactory.Factory(failure, IParameterEnum.LateralStructureFailure);

            IFunction stageDamage = getFunctionFromName(stageDamageName);
            IFdaFunction sDFunction = IFdaFunctionFactory.Factory(stageDamage, IParameterEnum.InteriorStageDamage);


            retval[0] = flowFreqProbs;
            retval[1] = ffFunc;

            retval[2] = inOutProbs;
            retval[3] = iOFunc;

            retval[4] = ratingProbs;
            retval[5] = ratFunc;

            retval[6] = extIntProbs;
            retval[7] = eIFunc;

            retval[8] = failureProbs;
            retval[9] = failureFunc;

            retval[10] = stageDamageProbs;
            retval[11] = sDFunction;

            //add the threshold types and values
            retval[12] = testData[12];
            retval[13] = testData[13];

            //add the expected results
            retval[14] = testData[14];
            retval[15] = testData[15];
            retval[16] = testData[16];

            //now add the worksheet number, row and column to write to
            retval[17] = testData[17];
            retval[18] = testData[18];
            retval[19] = testData[19];

            return retval;
        }

        private IFunction getFunctionFromName(string name)
        {
            if(_FunctionsDictionary.ContainsKey(name))
            {
                return _FunctionsDictionary[name];
            }
            else
            {
                return null;
            }
        }

        private void LoadFunctionsFromWorksheet(XLWorkbook workbook, int wsNumber)
        {
            IXLWorksheet ws = workbook.Worksheet(wsNumber);
            bool moreFunctionsExist = true;

            //find the first function
            int functionStartRow = FindNextFunctionRowIndex(ws, 1);

            while(moreFunctionsExist)
            {
                int lastRowOfTest = -1;
                //this is the function in the first column
                LoadFunction(ws, functionStartRow, 1, out lastRowOfTest);
                //now that we found a function. Look horizontally for more
                //we already got the function starting in column 1, start looking for more in column 5
                LoadAnyHorizontalFunctions(ws, functionStartRow, 5);

                int nextVerticalFunctionIndex = FindNextFunctionRowIndex(ws, lastRowOfTest);
                if(nextVerticalFunctionIndex == -1)
                {
                    moreFunctionsExist = false;
                }
                else
                {
                    functionStartRow = nextVerticalFunctionIndex;
                }

            }



        }

        private void LoadAnyHorizontalFunctions(IXLWorksheet ws, int functionStartRow, int startLookingAtColumn)
        {
            bool moreHorizontalTests = true;
            while (moreHorizontalTests)
            {
                int nextHorizontalFunction = FindNextFunctionColumnIndex(ws, functionStartRow, startLookingAtColumn);
                if (nextHorizontalFunction != -1)
                {
                    //we don't actually care about the last row since we are going horizontally
                    //just move to the right a few columns and start searching from there.
                    int lastRowOfFunction = -1;
                    LoadFunction(ws, functionStartRow, nextHorizontalFunction,out lastRowOfFunction);
                    startLookingAtColumn = nextHorizontalFunction + 4;
                }
                else
                {
                    moreHorizontalTests = false;
                }
            }
        }

        private int FindNextFunctionColumnIndex(IXLWorksheet ws, int row, int startLookingAtColumn)
        {
            for (int i = startLookingAtColumn; i < startLookingAtColumn + 15; i++)
            {
                string value = (string)ws.Row(row).Cell(i).Value;
                //if (value.Length > 2 && value[2] == '-')
                if(value.Length >0 && value[0] == '#')
                {
                    return i;
                }
            }
            return -1;
        }

        protected static int FindNextFunctionRowIndex(IXLWorksheet ws, int startLookingAtRow)
        {
            for (int i = startLookingAtRow; i < startLookingAtRow + 40; i++)
            {
                object cellValue =  ws.Row(i).Cell(1).Value;
                string value = Convert.ToString(cellValue);

                //if(value.Length>2 && value[2] == '-')
                if(value.Length>0 && value[0] == '#')
                {
                    return i;
                }
            }
            return -1;
        }

        private void LoadFunction(IXLWorksheet ws, int row, int col, out int lastRowOfTest)
        {
            //the row should be the row that the name of the function is on.
            //the dist type will be next to it
            string name = (string)ws.Row(row).Cell(col).Value;
            string distType = (string)ws.Row(row).Cell(col+1).Value;
            string interpolator = (string)ws.Row(row).Cell(col + 2).Value;

            InterpolationEnum interp = ConvertToInterpolationEnum(interpolator);

            //todo: ignore dist type of truncated normal
            if(distType.ToUpper() == "TRUNCATED NORMAL")
            {
                lastRowOfTest = row + 1;
                return;
            }

            List<ICoordinate> coordinates = ReadCoordinates(ws, row + 2, col, distType);
           
            //todo: ignore cubic spline for now
            if(interp == InterpolationEnum.NaturalCubicSpline )
            {
                lastRowOfTest = row + coordinates.Count + 1;
                return;
            }

            IFunction func = IFunctionFactory.Factory(coordinates, interp);
            //IFdaFunction fdaFunc = IFdaFunctionFactory.Factory(func, IParameterEnum.)
            _FunctionsDictionary.Add(name, func);
            lastRowOfTest = row + coordinates.Count + 1;
            
        }

        private List<ICoordinate> ReadCoordinates(IXLWorksheet ws, int row, int col, string distType)
        {
            List<ICoordinate> coordinates = new List<ICoordinate>();
            List<double> xValues = GetDoubleValuesVariableLength(row, col, ws);
            switch(distType.ToUpper())
            {
                case "CONSTANT":
                    {
                        List<double> yValues = GetDoubleValuesVariableLength(row, col + 1, ws);
                        if(xValues.Count != yValues.Count)
                        {
                            throw new ArgumentException("X and Y values were different lengths on row: " + row + " col: " + col);
                        }
                        for(int i = 0;i<xValues.Count;i++)
                        {
                            coordinates.Add( ICoordinateFactory.Factory(xValues[i], yValues[i]));
                        }
                        break;
                    }
                case "NORMAL":
                    {
                        List<double> means = GetDoubleValuesVariableLength(row, col + 1, ws);
                        List<double> stDevs = GetDoubleValuesVariableLength(row, col + 2, ws);

                        if (xValues.Count != means.Count && means.Count != stDevs.Count)
                        {
                            throw new ArgumentException("X and Y values were different lengths on row: " + row + " col: " + col);
                        }
                        List<IDistributedOrdinate> ords = new List<IDistributedOrdinate>();
                        for(int i = 0;i<means.Count;i++)
                        {
                            IDistributedOrdinate dist = IDistributedOrdinateFactory.FactoryNormal(means[i], stDevs[i]);
                            coordinates.Add(ICoordinateFactory.Factory(xValues[i], dist));
                        }
                        break;
                    }
                case "TRIANGULAR":
                    {
                        List<double> min = GetDoubleValuesVariableLength(row, col + 1, ws);
                        List<double> mode = GetDoubleValuesVariableLength(row, col + 2, ws);
                        List<double> max = GetDoubleValuesVariableLength(row, col + 3, ws);

                        if (xValues.Count != min.Count && min.Count != mode.Count && min.Count != max.Count)
                        {
                            throw new ArgumentException("X and Y values were different lengths on row: " + row + " col: " + col);
                        }
                        List<IDistributedOrdinate> ords = new List<IDistributedOrdinate>();
                        for (int i = 0; i < min.Count; i++)
                        {
                            IDistributedOrdinate dist = IDistributedOrdinateFactory.FactoryTriangular(mode[i], min[i], max[i]);
                            coordinates.Add(ICoordinateFactory.Factory(xValues[i], dist));
                        }
                        break;
                    }
                case "UNIFORM":
                    {
                        List<double> min = GetDoubleValuesVariableLength(row, col + 1, ws);
                        List<double> max = GetDoubleValuesVariableLength(row, col + 2, ws);

                        if (xValues.Count != min.Count && min.Count != max.Count)
                        {
                            throw new ArgumentException("X and Y values were different lengths on row: " + row + " col: " + col);
                        }
                        List<IDistributedOrdinate> ords = new List<IDistributedOrdinate>();
                        for (int i = 0; i < min.Count; i++)
                        {
                            IDistributedOrdinate dist = IDistributedOrdinateFactory.FactoryUniform(min[i], max[i]);
                            coordinates.Add(ICoordinateFactory.Factory(xValues[i], dist));
                        }
                        break;
                    }
                case "TRUNCATED NORMAL":
                    {
                        List<double> means = GetDoubleValuesVariableLength(row, col + 1, ws);
                        List<double> stDevs = GetDoubleValuesVariableLength(row, col + 2, ws);
                        List<double> min = GetDoubleValuesVariableLength(row, col + 3, ws);
                        List<double> max = GetDoubleValuesVariableLength(row, col + 4, ws);

                        if (xValues.Count != min.Count && min.Count != max.Count && means.Count != max.Count && means.Count != stDevs.Count)
                        {
                            throw new ArgumentException("X and Y values were different lengths on row: " + row + " col: " + col);
                        }
                        List<IDistributedOrdinate> ords = new List<IDistributedOrdinate>();
                        for (int i = 0; i < min.Count; i++)
                        {
                            IDistributedOrdinate dist = IDistributedOrdinateFactory.FactoryTruncatedNormal(means[i], stDevs[i], min[i], max[i]);
                            coordinates.Add(ICoordinateFactory.Factory(xValues[i], dist));
                        }
                        break;
                    }
                case "BETA":
                    {
                        List<double> alpha = GetDoubleValuesVariableLength(row, col + 1, ws);
                        List<double> beta = GetDoubleValuesVariableLength(row, col + 2, ws);
                        List<double> location = GetDoubleValuesVariableLength(row, col + 3, ws);
                        List<double> scale = GetDoubleValuesVariableLength(row, col + 4, ws);

                        if (xValues.Count != alpha.Count && alpha.Count != beta.Count && location.Count != beta.Count && location.Count != scale.Count)
                        {
                            throw new ArgumentException("X and Y values were different lengths on row: " + row + " col: " + col);
                        }
                        List<IDistributedOrdinate> ords = new List<IDistributedOrdinate>();
                        for (int i = 0; i < alpha.Count; i++)
                        {
                            IDistributedOrdinate dist = IDistributedOrdinateFactory.FactoryBeta(alpha[i], beta[i], location[i], scale[i]);
                            coordinates.Add(ICoordinateFactory.Factory(xValues[i], dist));
                        }
                        break;
                    }
                default:
                    {
                        throw new Exception("Could not read coordinates because I could not match distribution type: " + distType);
                    }

            }
            return coordinates;
        }

        private InterpolationEnum ConvertToInterpolationEnum(string interp)
        {

            if (interp.ToUpper().Equals("LINEAR"))
            {
                return InterpolationEnum.Linear;
            }
            else if (interp.ToUpper().Equals("PIECEWISE"))
            {
                return InterpolationEnum.Piecewise;
            }
            else if (interp.ToUpper().Equals("NONE"))
            {
                return InterpolationEnum.None;
            }
            else if (interp.ToUpper().Equals("NATURALCUBICSPLINE") || interp.ToUpper().Equals("CUBICSPLINE"))
            {
                return InterpolationEnum.NaturalCubicSpline;
            }
            else throw new ArgumentException("could not convert '" + interp + "'.");
        }


    }
}
