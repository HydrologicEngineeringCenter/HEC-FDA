using System;
using System.Collections.Generic;

namespace FdaTester.ModelTester.Utilities
{
    public static class DataImporter
    {
        #region Functions
        public static List<Object> ReadSingleDelimitedColumn(string fullFilePath, int importColumn, char delimiter = '\t')
        {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(new System.IO.FileStream(fullFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read)))
            {
                List<Object> importedData = new List<Object>();

                string line = reader.ReadLine();
                while (line != null)
                {
                    string[] entry = line.Split(delimiter);
                    for (int j = 0; j < entry.Length; j++)
                    {
                        if (j == importColumn)
                        {
                            importedData.Add(entry[j]);
                        }
                    }
                    line = reader.ReadLine();
                }
                return importedData;
            }   
        }

        public static List<List<object>> ReadMultipleDelimitedColumns(string fullFilePath, int[] importColumns, char delimiter = '\t')
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(new System.IO.FileStream(fullFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read));

            //1. Loop across number of columns to import.
            List<List<object>> importedData = new List<List<object>>();
            for (int i = 0; i < importColumns.Length; i++)
            {
                //2. Loop across lines in file
                List<object> columnData = new List<object>();
                string line = reader.ReadLine();
                while (line != null)
                {
                    //3. Loop across delimited elements in each line.
                    string[] entry = line.Split(delimiter);
                    for (int j = 0; j < entry.Length; j++)
                    {
                        //4. If the element is from the column to import then store the data.
                        if (i == j)
                        {
                            columnData.Add(entry[j]);
                        }
                    }
                    line = reader.ReadLine();
                }
                //5. Add the imported columnt to the stored data.
                importedData.Add(columnData);
            }
            return importedData;
        }
        #endregion
    }
}
