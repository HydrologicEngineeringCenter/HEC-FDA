using Functions;
using FunctionsView.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsView.View
{
    //Distribution types and number of y columns:
    //None: 1
    //Normal: 2
    //uniform: 2
    //triangular:3
    //Scaled Beta: 4
    //truncated Normal: 4
    public static class ColumnWidths
    {
        public static int COL_X_WIDTH = 80;
        public static int COL_DIST_WIDTH = 100;
        public static int COL_INTERP_WIDTH = 100;

        private static readonly Dictionary<DistributionType, int[]> DYNAMIC_COL_WIDTHS = new Dictionary<DistributionType, int[]>()
        {
            { DistributionType.Constant, new int[]{100 } },
            {DistributionType.Normal, new int[]{100,50} },
            { DistributionType.Uniform, new int[]{100,100 } },
            { DistributionType.Triangular, new int[]{100,100,100 } },
            { DistributionType.Beta4Parameters, new int[]{50,50,100,100 } },
            { DistributionType.TruncatedNormal, new int[]{100,50,100,100 } },

        };

        /// <summary>
        /// Gets the column widths for the "y" columns for a beta distribution. A table type is passed in
        /// that represents the widest table type in the editors list of tables. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int[] GetComputedBetaWidths(int[] dynamicColumnWidths)
        {
            ////Beta columns: alpha, beta, Min, Max
            return TransformColumnWidthArrayForTablesWithFewerColumns(4, dynamicColumnWidths); 
        }

        /// <summary>
        /// Gets the column widths for the "y" columns for a truncated normal distribution. A table type is passed in
        /// that represents the widest table type in the editors list of tables. If the table type is truncated normal
        /// or a narrower type then this will return the dictionary values for truncated normal
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int[] GetComputedTrancatedNormalWidths(int[] dynamicColumnWidths)
        {
            ////truncated normal columns: Mean, St dev, Min, Max
            return TransformColumnWidthArrayForTablesWithFewerColumns(4, dynamicColumnWidths);  
        }

        /// <summary>
        /// Gets the column widths for the "y" columns for a uniform distribution. A table type is passed in
        /// that represents the widest table type in the editors list of tables. If the table type is uniform
        /// or a narrower type then this will return the dictionary values for uniform
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int[] GetComputedUniformWidths(int[] dynamicColumnWidths)
        {
            return TransformColumnWidthArrayForTablesWithFewerColumns(2, dynamicColumnWidths);
        }

        private static int[] TransformColumnWidthArrayForTablesWithFewerColumns(int tableColumnCount, int[] colWidthArray)
        {
            int[] tableWidths = new int[tableColumnCount];
            //load the values up to the last spot
            for(int i = 0;i<tableColumnCount-1;i++)
            {
                tableWidths[i] = colWidthArray[i];
            }
            //figure out the last value
            if (colWidthArray.Length > tableColumnCount)
            {
                int sum = GetSumFromIndexToLast(tableColumnCount-1, colWidthArray);
                tableWidths[tableColumnCount-1] = sum;
            }
            else
            {
                tableWidths[tableColumnCount-1] = colWidthArray[tableColumnCount-1];
            }
            return tableWidths;
        }
        /// <summary>
        /// Gets the column widths for the "y" columns for a triangular distribution. A table type is passed in
        /// that represents the widest table type in the editors list of tables. If the table type is triangular
        /// or a narrower type then this will return the dictionary values for Triangular
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int[] GetComputedTriangularWidths(int[] dynamicColumnWidths)
        {
            //triangular columns: Most Likely, Min, Max
            //we need to return an array with 3 values. If the dynamic col widths
            //are more than 3, then we sum the remainder.
            return TransformColumnWidthArrayForTablesWithFewerColumns(3, dynamicColumnWidths);         
        }

        private static int GetSumFromIndexToLast(int index, int[] array)
        {
            int sum = 0;
            for(int i = index;i<array.Length;i++)
            {
                sum += array[i];
            }
            return sum;
        }
        public static int[] GetComputedNormalWidths(int[] dynamicColumnWidths)
        {
            return TransformColumnWidthArrayForTablesWithFewerColumns(2, dynamicColumnWidths);          
        }
        
        /// <summary>
        /// The total width of all columns in the table
        /// </summary>
        /// <param name="dynamicColumnWidths">The array of integer widths for the dynamically changeable columns (the 'Y' columns other than distribution type)</param>
        /// <returns></returns>
        public static int TotalColumnWidths(int[] dynamicColumnWidths)
        {
            int totalDynamicWidths = dynamicColumnWidths.Sum();
            return (COL_X_WIDTH + totalDynamicWidths + COL_DIST_WIDTH + COL_INTERP_WIDTH);
        }

        private static List<DistributionType> GetUniqueListOfDistributionsFromRows(List<CoordinatesFunctionRowItem> rows)
        {
            List<DistributionType> retval = new List<DistributionType>();
            foreach(CoordinatesFunctionRowItem row in rows)
            {
                if(!retval.Contains(row.SelectedDistributionType))
                {
                    retval.Add(row.SelectedDistributionType);
                }
            }
            return retval;
        }


        private static int GetLargestNumberOfColumns(List<DistributionType> distributions)
        {
            int max = 0;
            foreach(DistributionType dist in distributions)
            {
                if(DYNAMIC_COL_WIDTHS[dist].Length > max)
                {
                    max = DYNAMIC_COL_WIDTHS[dist].Length;
                }
            }
            return max;
        }

        private static int[] GetColumnWidths(List<DistributionType> distributions)
        {
            //I potentially have rows of several different distribution types
            //Find the type with the most number of columns. Then loop over all the dist types
            //and find the widest value for that column. 
            //The return value will be the widest column from each type
            int numColumns = GetLargestNumberOfColumns(distributions);
            int[] retval = new int[numColumns];
            for (int i = 0; i < numColumns; i++)
            {
                int maxWidth = 0;
                foreach (DistributionType dist in distributions)
                {
                    if (i < DYNAMIC_COL_WIDTHS[dist].Length)
                    {
                        int width = DYNAMIC_COL_WIDTHS[dist][i];
                        if (width > maxWidth)
                        {
                            maxWidth = width;
                        }
                    }
                }
                retval[i] = maxWidth;
            }
            return retval;

        }

        /// <summary>
        /// Returnes the table type of the widest table from the rows passed in.
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static int[] GetComputedColumnWidths(List<CoordinatesFunctionRowItem> rows)
        {
            List<DistributionType> uniqueDistributionTypes = GetUniqueListOfDistributionsFromRows(rows);
            return GetColumnWidths(uniqueDistributionTypes);
        }   

    }
}
