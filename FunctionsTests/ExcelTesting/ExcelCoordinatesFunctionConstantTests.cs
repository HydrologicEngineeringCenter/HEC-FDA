
using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FunctionsTests.ExcelTesting
{
    [ExcludeFromCodeCoverage]

    public class ExcelCoordinatesFunctionConstantTests
    {
        private readonly ITestOutputHelper output;

        private const string _TestDataRelativePath = "ExcelTesting\\ExcelData\\CoordinatesFunctionsConstants.xlsx";
        private const int ORDER_WORKSHEET = 1;
        private const int F_OF_X_WORKSHEET = 2;
        private const int INVERSE_F_OF_X_WORKSHEET = 3;
        private const int F_OF_X_NONE = 4;
        private const int F_OF_X_PIECEWISE = 5;
        private const int F_OF_X_LINEAR = 6;
        private const int F_OF_X_CUBIC_SPLINE = 7;

        private const int INVERSE_F_OF_X_NONE = 8;
        private const int INVERSE_F_OF_X_PIECEWISE = 9;
        private const int INVERSE_F_OF_X_LINEAR = 10;
        private const int INVERSE_F_OF_X_CUBIC_SPLINE = 11;

        private const double INVALID_OPERATION_EXCEPTION = -9999;
        private const double ARGUMENT_OUT_OF_RANGE_EXCEPTION = -8888;

        public ExcelCoordinatesFunctionConstantTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        #region Worksheet 1: Order

        #endregion
       

        #region Worksheet 7: cubic spline
       
        #endregion       


        #region Multiple Worksheet Tests
        
        #endregion

        private bool DidTestPass(List<object> actualResults, List<double> expectedResults)
        {
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

        private DataTable CreateDataTable(List<double> actualYs)
        {
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("Actual");
            for (int i = 0; i < actualYs.Count; i++)
            {
                dt.Rows.Add(actualYs[i]);
            }

            return dt;
        }
        private DataTable CreateDataTable(List<object> actualYs)
        {
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("Actual");
            for (int i = 0; i < actualYs.Count; i++)
            {
                dt.Rows.Add(actualYs[i]);
            }

            return dt;
        }

        private DataTable CreateDataTable(string result)
        {
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("Actual");
            dt.Rows.Add(result);
            return dt;
        }

        private bool HasMinimalDifference(double value1, double value2)
        {
            double diff = Math.Abs(value1 - value2);
            return diff < .0001;
        }
        private bool HasMinimalDifference(double value1, double value2, int units)
        {
            long lValue1 = BitConverter.DoubleToInt64Bits(value1);
            long lValue2 = BitConverter.DoubleToInt64Bits(value2);

            // If the signs are different, return false except for +0 and -0.
            if ((lValue1 >> 63) != (lValue2 >> 63))
            {
                if (value1 == value2)
                    return true;

                return false;
            }

            long diff = Math.Abs(lValue1 - lValue2);

            if (diff <= (long)units)
                return true;

            return false;
        }

        private void WriteFunctionToOutput(ICoordinatesFunction function, string name)
        {
            output.WriteLine("");
            output.WriteLine("Function: " + name);
            foreach (ICoordinate coord in function.Coordinates)
            {
                output.WriteLine(coord.X.Value() + ", " + coord.Y.Value());
            }

        }
    }

}
