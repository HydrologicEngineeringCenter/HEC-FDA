using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;


namespace Importer
{
    public class MemoDataField
    {
        static public void ProcessMemoDataField(string theMemoField,
                         ref int numValRows,
                         ref int numColsOfData,
                         ref double[,] _valuesInMemoField)
        {
            //  Processes one Memo Field. Assumes it contains double values in a
            //      square matrix.
            char delimiterTabChar = '\t';
            char delimiterCrChar = '\r';

            //Break string into an array of strings, Each string a row of memo field
            //It leaves the new line (\n) in place which must be stripped out
            string[] rowsOfData;
            string[] rowOfDataParsed;
            int numRowsOfData = 0;
            numColsOfData = 0;
            rowsOfData = theMemoField.Split(delimiterCrChar);
            numRowsOfData = rowsOfData.Length;
            if (rowsOfData[rowsOfData.Length - 1].Length < 1)
                numRowsOfData = numRowsOfData - 1;

            //Count the maximum number of columns based on first row
            rowOfDataParsed = rowsOfData[0].Split(delimiterTabChar);
            numColsOfData = rowOfDataParsed.Length;
            if (rowOfDataParsed[rowOfDataParsed.Length - 1].Length < 1)
                numColsOfData = numColsOfData - 1;

            _valuesInMemoField = new double[numRowsOfData, numColsOfData];

            //Process Each Row of Data in Memo Field
            numValRows = 0;
            for (int irow = 0; irow < numRowsOfData; irow++)
            {
                //Strip the new line (\n) from each column of data
                char[] theRowChar = rowsOfData[irow].ToCharArray();
                try
                {
                    if (theRowChar[0] == '\n')
                        theRowChar[0] = ' ';
                    if (theRowChar[theRowChar.Length - 1] == '\t')
                        theRowChar[theRowChar.Length - 1] = ' ';
                    rowsOfData[irow] = new string(theRowChar);
                    rowsOfData[irow] = rowsOfData[irow].Trim();
                }
                catch
                {
                    rowsOfData[irow] = String.Empty;
                }
                if (rowsOfData[irow].Length > 0)
                {
                    /*rdc temp;rdc critical;05Dec2018;Temporary removal for TestDbtReader program.
                    if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 49)
                        Write($"\nProcessMemoDataField Row: {numValRows + 1}\t");
                        */

                    //Process each column of data
                    rowOfDataParsed = rowsOfData[irow].Split(delimiterTabChar);
                    for (int icol = 0; icol < rowOfDataParsed.Length && icol < numColsOfData; icol++)
                    {
                        string theCellContent = "";
                        try
                        {
                            theCellContent = rowOfDataParsed[icol].ToString();
                            double theDoubleValue = Convert.ToDouble(rowOfDataParsed[icol]);
                            _valuesInMemoField[numValRows, icol] = theDoubleValue;
                            //values[numValRows][icol] = new Double(Convert.ToDouble(rowOfDataParsed[icol]));

                            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 49)
                                Write($"{ _valuesInMemoField[numValRows, icol]}\t");
                        }
                        catch
                        {
                            //_valuesInMemoField[numValRows, icol] = theCellContent;
                            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 49)
                                Write($"{theCellContent}\t");
                        }
                    }
                    numValRows++;

                }
            }
            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 49)
                WriteLine("");
        }
        static public string ProcessDoubleToMemoString(
            int numValRows,
            int numColsOfData,
            double[,] valuesInMemoField,
            ref string theMemoField)
        {
            //  Processes one Memo Field. Assumes it contains double values in a
            //      square matrix. It converts a double array[,] to a tab delimited
            //      string in the form of a square matrix. Columns come first, then
            //      next row
            char delimiterTabChar = '\t';
            char delimiterCrChar = '\r';

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                for (int irow = 0; irow < numValRows; irow++)
                {
                    for (int icol = 0; icol < numColsOfData; icol++)
                    {
                        sw.Write(valuesInMemoField[irow,icol]);
                        sw.Write(delimiterTabChar);
                    }
                    sw.Write(delimiterCrChar);
                }
                theMemoField = sw.ToString();
            }
            return theMemoField;
        }
    }
}
