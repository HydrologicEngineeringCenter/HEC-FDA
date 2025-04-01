using Geospatial.Features;
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
    public static bool RowHasValuesForColumns(PointFeatureCollection pointShapefile, int rowIndex, List<string> requiredFields, out List<string> missingValues)
    {
        bool valid = true;
        missingValues = new();
        TableRow row = pointShapefile.AttributeTable.Rows[rowIndex];

        for(int i = 0; i < requiredFields.Count; i++)
        {
            string fieldName = requiredFields[i];
            object val = row.Value(fieldName);
            Type type = row.Table.GetColumn(fieldName).Type;
            if (CellIsEmpty(val, type))
            {
                valid = false;
                missingValues.Add(fieldName);
            }
        }
        return valid;
    }

    /// <summary>
    /// Checks that all rows have the specified field. out param returns index of rows without. 
    /// </summary>
    public static bool RowsHaveValueForColumn(PointFeatureCollection pointShapefile, string field, out List<int> rowsWithMissingData)
    {
        bool valid = true;
        rowsWithMissingData = [];

        var rows = pointShapefile.AttributeTable.Rows;
        for (int i = 0; i < rows.Count; i++)
        {
            object val = rows[i].Value(field);
            Type type = rows[i].Table.GetColumn(field).Type;
            if (CellIsEmpty(val, type))
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
    public static bool AllRowsHaveUniqueValueForColumn<T>(PointFeatureCollection pointShapefile, string columnName, out List<int> rowsWithDuplicatedData)
    {
        bool valid = true;
        var rows = pointShapefile.AttributeTable.Rows;
        rowsWithDuplicatedData = [];
        List<object> previousVals = [];

        for (int i = 0; i < rows.Count; i++)
        {
            object val = rows[i].Value(columnName);
            Type type = rows[i].Table.GetColumn(columnName).Type;
            if (CellIsEmpty(val, type))
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
    

    private static bool CellIsEmpty(object cellValue, Type type)
    {
        ///ints have to have some value. Can't be null. Geospatial will return -1 for missing values in an int column. 
        if(type == typeof(int))
        {
            return ((int)cellValue == -1);
        }
        return (cellValue == DBNull.Value || cellValue == null);

    }

}
