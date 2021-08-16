using Functions;
using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace FunctionsTests.ExcelTesting
{
    [ExcludeFromCodeCoverage]

    public class ExcelLinkedFunctionsTests
    {
        private const double INVALID_OPERATION_EXCEPTION = -9999;
        private const double ARGUMENT_OUT_OF_RANGE_EXCEPTION = -8888;
        private const string _TestDataRelativePath = "ExcelTesting\\ExcelData\\LinkedFunctionsTestData.xlsx";


        [Theory]
        [ExcelLinkedFunctionsOrderData(_TestDataRelativePath, 1)]
        public void ExcelOrderTests(List<double> xs1, List<double> ys1, string interpolation1,
         List<double> xs2, List<double> ys2, string interpolation2,
         List<double> xs3, List<double> ys3, string interpolation3, 
         string expectedOrder, int rowToWriteTo, int columnToWriteTo)
        {
            InterpolationEnum interp1 = ConvertToInterpolationEnum(interpolation1);
            IFunction func1 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs1, ys1, interp1);

            InterpolationEnum interp2 = ConvertToInterpolationEnum(interpolation2);
            IFunction func2 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs2, ys2, interp2);

            InterpolationEnum interp3 = ConvertToInterpolationEnum(interpolation3);
            IFunction func3 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs3, ys3, interp3);

            List<ICoordinatesFunction> funcs = new List<ICoordinatesFunction>() { func1, func2, func3 };

            CoordinatesFunctionLinked linkedFunc = new CoordinatesFunctionLinked(funcs);

            OrderedSetEnum order = linkedFunc.Order;
            bool passedTest = order.ToString().ToUpper().Equals(expectedOrder.ToUpper());

            ExcelDataAttributeBase.SaveData(_TestDataRelativePath, 1, rowToWriteTo, columnToWriteTo, CreateDataTable(order.ToString()), passedTest);
            Assert.True(passedTest);

        }


        [Theory]
        [ExcelLinkedFunctionFofXData(_TestDataRelativePath, new int[] { 2, 3 })]
        public void Excel_FofX_Tests(List<double> xs1, List<double> ys1, string interpolation1,
         List<double> xs2, List<double> ys2, string interpolation2,
         List<double> xs3, List<double> ys3, string interpolation3,
         List<double> knownXs, List<double> expectedYs, int worksheetNumber, int rowToWriteTo, int columnToWriteTo)
        {
           
                InterpolationEnum interp1 = ConvertToInterpolationEnum(interpolation1);
                IFunction func1 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs1, ys1, interp1);

                InterpolationEnum interp2 = ConvertToInterpolationEnum(interpolation2);
                IFunction func2 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs2, ys2, interp2);

                InterpolationEnum interp3 = ConvertToInterpolationEnum(interpolation3);
                IFunction func3 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs3, ys3, interp3);

                List<ICoordinatesFunction> funcs = new List<ICoordinatesFunction>() { func1, func2, func3 };
                CoordinatesFunctionLinked linkedFunc = new CoordinatesFunctionLinked(funcs);

                List<object> actualVals = new List<object>();

                foreach (double xVal in knownXs)
                {
                    try
                    {
                        double result = linkedFunc.F(IOrdinateFactory.Factory(xVal)).Value();
                        actualVals.Add(result);
                    }
                    catch (Exception ex)
                    {
                        if (ex is InvalidOperationException)
                        {
                            actualVals.Add(INVALID_OPERATION_EXCEPTION);
                        }
                        else if (ex is ArgumentOutOfRangeException)
                        {
                            actualVals.Add(ARGUMENT_OUT_OF_RANGE_EXCEPTION);
                        }
                        else
                        {
                            actualVals.Add(ex.GetType().ToString() + ": " + ex.Message);
                        }
                    }
                }

            bool passedTest = DidTestPass(actualVals, expectedYs);

            ExcelDataAttributeBase.SaveData(_TestDataRelativePath, worksheetNumber, rowToWriteTo, columnToWriteTo, CreateDataTable(actualVals, linkedFunc.Order), passedTest);
            Assert.True(passedTest);
        

        }

        [Theory]
        [ExcelLinkedFunctionFofXData(_TestDataRelativePath, new int[] { 4,5})]
        public void Excel_InverseFofX_Tests(List<double> xs1, List<double> ys1, string interpolation1,
        List<double> xs2, List<double> ys2, string interpolation2,
        List<double> xs3, List<double> ys3, string interpolation3,
        List<double> knownYs, List<double> expectedYs,int worksheetNumber, int rowToWriteTo, int columnToWriteTo)
        {

            InterpolationEnum interp1 = ConvertToInterpolationEnum(interpolation1);
            IFunction func1 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs1, ys1, interp1);

            InterpolationEnum interp2 = ConvertToInterpolationEnum(interpolation2);
            IFunction func2 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs2, ys2, interp2);

            InterpolationEnum interp3 = ConvertToInterpolationEnum(interpolation3);
            IFunction func3 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs3, ys3, interp3);

            List<ICoordinatesFunction> funcs = new List<ICoordinatesFunction>() { func1, func2, func3 };
            CoordinatesFunctionLinked linkedFunc = new CoordinatesFunctionLinked(funcs);

            List<object> actualVals = new List<object>();

            foreach (double yVal in knownYs)
            {
                try
                {
                    double result = linkedFunc.InverseF(IOrdinateFactory.Factory(yVal)).Value();
                    actualVals.Add(result);
                }
                catch (Exception ex)
                {
                    if (ex is InvalidOperationException)
                    {
                        actualVals.Add(INVALID_OPERATION_EXCEPTION);
                    }
                    else if (ex is ArgumentOutOfRangeException)
                    {
                        actualVals.Add(ARGUMENT_OUT_OF_RANGE_EXCEPTION);
                    }
                    else
                    {
                        actualVals.Add(ex.GetType().ToString() + ": " + ex.Message);
                    }
                }
            }

            bool passedTest = DidTestPass(actualVals, expectedYs);

            ExcelDataAttributeBase.SaveData(_TestDataRelativePath,worksheetNumber, rowToWriteTo, columnToWriteTo, CreateDataTable(actualVals, linkedFunc.Order), passedTest);
            Assert.True(passedTest);


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
            else if (interp.ToUpper().Equals("NATURALCUBICSPLINE"))
            {
                return InterpolationEnum.NaturalCubicSpline;
            }
            else throw new ArgumentException("could not convert '" + interp + "'.");
        }

        private DataTable CreateDataTable(string result)
        {
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("Actual");
            dt.Rows.Add(result);
            return dt;
        }

        private bool DidTestPass(List<object> actualResults, List<double> expectedResults)
        {
            if(actualResults.Count != expectedResults.Count)
            {
                return false;
            }
            bool passedTest = true;
            for (int i = 0; i < expectedResults.Count; i++)
            {
                object result = actualResults[i];
                if (result is double)
                {
                    if (!HasMinimalDifference((double)result, expectedResults[i]))//, 1))
                    {
                        passedTest = false;
                    }
                }
                else
                {
                    passedTest = false;
                }
            }
            return passedTest;
        }

        private DataTable CreateDataTable(List<object> actualYs, OrderedSetEnum linkedFunctionOrder)
        {
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("Actual");
            dt.Columns.Add("LinkedFunctionOrder");
            for (int i = 0; i < actualYs.Count; i++)
            {
                if (i == 0)
                {
                    dt.Rows.Add(actualYs[i], linkedFunctionOrder.ToString());
                }
                else
                {
                    dt.Rows.Add(actualYs[i]);
                }
            }

            return dt;
        }

        private bool HasMinimalDifference(double value1, double value2)
        {
            double diff = Math.Abs(value1 - value2);
            return diff < .0001;
        }

    }
}
