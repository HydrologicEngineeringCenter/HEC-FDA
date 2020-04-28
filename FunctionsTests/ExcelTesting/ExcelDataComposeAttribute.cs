using ClosedXML.Excel;
using Functions;
using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xunit.Sdk;

namespace FunctionsTests.ExcelTesting
{
    [ExcludeFromCodeCoverage]
    //Note that these class names must end in "Attribute"
    //also note that the last two parameters of your unit test should be: int rowToWriteResultsTo, int columnToWriteResultsTo
    //You should not include these two in any of the properties below, just the hard data.
    public sealed class ExcelDataComposeAttribute: ExcelDataAttributeBase
    {
        protected override List<Type> ColumnTypes { get; set; }
        protected override List<DataLength> ColumnDataLengths { get; set; }
        protected override List<int> ColumnIndices { get; set; }
        protected override int ColumnToWriteResults { get; set; }

        public ExcelDataComposeAttribute(string fileName, int worksheetNumber):base(fileName, worksheetNumber)
        {
            ColumnTypes = new List<Type>() { typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double) };
            ColumnDataLengths = new List<DataLength> { DataLength.VariableLength, DataLength.VariableLength, DataLength.VariableLength, DataLength.VariableLength, DataLength.VariableLength, DataLength.VariableLength };
            ColumnIndices = new List<int>() { 1, 2, 4, 5, 7, 8 };
            ColumnToWriteResults = 10;
        }

       
      
    }
}
