using System;
using System.Collections.Generic;
using Utility.Memory;


namespace HEC.FDA.Model.Spatial;
/// <summary>
/// Facilitates validating shapefile is readable and all required fields are present. 
/// </summary>
public static class StructureDataValidator
{
    /// <summary>
    /// Check that the given row has all the specified fields. out param returns the whole row as objects, and list of the column headers with missing data.
    /// </summary>
    public static bool RowHasValuesForColumns(PointShapefile pointShapefile, int rowIndex, List<string> requiredFields, out object[] rowValues, out List<string> missingValues)
    {
        bool valid = true;
        missingValues = new List<string>();

        TableRow row = pointShapefile.Rows[rowIndex];
        rowValues = new object[requiredFields.Count];

        for(int i = 0; i < requiredFields.Count; i++)
        {
            string fieldName = requiredFields[i];
            object val = row.Value(fieldName);
            if(CellHasData(val))
            {
                valid = false;
                missingValues.Add(fieldName);
            }
            rowValues[i] = val;
        }
        return valid;
    }

    /// <summary>
    /// Checks that all rows have the specified field. out param returns index of rows without. 
    /// </summary>
    public static bool RowsHaveValueForColumn(PointShapefile pointShapefile, string field, out List<int> rowsWithMissingData)
    {
        bool valid = true;
        rowsWithMissingData = [];

        var rows = pointShapefile.Rows;
        for (int i = 0; i < rows.Count; i++)
        {
            object val = rows[i];
            if (!CellHasData(val))
            {
                rowsWithMissingData.Add(i);
                valid = false;
            }
        }
        return valid;
    }



    /// <summary>
    /// Checks that all rows have unique vlaues for the specified column. out param returns index of rows with duplicated data. Ignores rows with no data. 
    /// </summary>
    public static bool AllRowsHaveUniqueValueForColumn<T>(PointShapefile pointShapefile, string columnName, out List<int> rowsWithDuplicatedData)
    {
        bool valid = true;
        var rows = pointShapefile.Rows;
        rowsWithDuplicatedData = [];
        List<object> previousVals = [];

        for (int i = 0; i < rows.Count; i++)
        {
            object val = rows[i];
            if (!CellHasData(val))
            {
                continue;
            }
            if(previousVals.Contains(val))
            {
                rowsWithDuplicatedData.Add(i);
                valid = false;
            }
            else
            {
                previousVals.Add(val);
            }
        }
        return valid;
    }
    

    private static bool CellHasData(object cellValue)
    {
        return (cellValue == DBNull.Value || cellValue == null || string.IsNullOrWhiteSpace(cellValue as string));
    }

}
