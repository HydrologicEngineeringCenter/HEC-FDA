using Functions;
using FunctionsView.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsView.View
{
    //types and number of y columns:
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

        private static readonly Dictionary<TableTypes, int[]> DYNAMIC_COL_WIDTHS = new Dictionary<TableTypes, int[]>()
        {
            { TableTypes.None, new int[]{100 } },
            {TableTypes.Normal, new int[]{50,50} },
            { TableTypes.Uniform, new int[]{100,100 } },
            { TableTypes.Triangular, new int[]{100,100,100 } },
            { TableTypes.Beta, new int[]{50,50,100,100 } },
            { TableTypes.TruncatedNormal, new int[]{50,50,100,100 } },

        };

        /// <summary>
        /// Gets the column widths for the "y" columns for a beta distribution. A table type is passed in
        /// that represents the widest table type in the editors list of tables. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int[] GetComputedBetaWidths(TableTypes type)
        {
            //Beta columns: alpha, beta, Min, Max
            int[] truncatedNormal = DYNAMIC_COL_WIDTHS[TableTypes.TruncatedNormal];
            int[] betaWidths = DYNAMIC_COL_WIDTHS[TableTypes.Beta];

            switch (type)
            {
                case TableTypes.TruncatedNormal:
                    {
                        //if we get here then the truncated Normal has wider columns
                        int alph = betaWidths[0];
                        int beta = betaWidths[1];
                        int min = betaWidths[2];
                        int max = betaWidths[3];

                        return new int[] { alph, beta, min, max };
                    }
                default:
                    {
                        return betaWidths;
                    }
            }
        }

        /// <summary>
        /// Gets the column widths for the "y" columns for a truncated normal distribution. A table type is passed in
        /// that represents the widest table type in the editors list of tables. If the table type is truncated normal
        /// or a narrower type then this will return the dictionary values for truncated normal
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int[] GetComputedTrancatedNormalWidths(TableTypes type)
        {
            //truncated normal columns: Mean, St dev, Min, Max

            //you only have to check for types that have more columns than yourself.
            int[] triangular = DYNAMIC_COL_WIDTHS[TableTypes.Triangular];
            int[] truncatedNormal = DYNAMIC_COL_WIDTHS[TableTypes.TruncatedNormal];
            int[] scaledBeta = DYNAMIC_COL_WIDTHS[TableTypes.Beta];

            switch (type)
            {
                case TableTypes.Beta:
                    {
                        //beta has 4 columns
                        int mean = scaledBeta[0];
                        int stDev = scaledBeta[1];
                        int min = scaledBeta[2];
                        int max = scaledBeta[3];

                        return new int[] { mean, stDev, min, max };
                    }
                default:
                    {
                        return truncatedNormal;
                    }
            }
        }

        /// <summary>
        /// Gets the column widths for the "y" columns for a uniform distribution. A table type is passed in
        /// that represents the widest table type in the editors list of tables. If the table type is uniform
        /// or a narrower type then this will return the dictionary values for uniform
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int[] GetComputedUniformWidths(TableTypes type)
        {
            //uniform columns: Min, Max

            //you only have to check for types that have more columns than yourself.
            int[] uniformWidths = DYNAMIC_COL_WIDTHS[TableTypes.Uniform];
            int[] normalWidths = DYNAMIC_COL_WIDTHS[TableTypes.Normal];
            int[] triangular = DYNAMIC_COL_WIDTHS[TableTypes.Triangular];
            int[] truncatedNormal = DYNAMIC_COL_WIDTHS[TableTypes.TruncatedNormal];
            int[] scaledBeta = DYNAMIC_COL_WIDTHS[TableTypes.Beta];

            switch (type)
            {
                case TableTypes.Beta:
                    {
                        //beta has 4 columns
                        int min = scaledBeta[0];
                        //add the second, third and fourth col widths together
                        int max = scaledBeta[1] + scaledBeta[2] + scaledBeta[3];
                        return new int[] { min, max };
                    }
                case TableTypes.TruncatedNormal:
                    {
                        //truncatedNormal has 4 columns
                        int min = truncatedNormal[0];
                        //add the second, third and fourth col widths together
                        int max = truncatedNormal[1] + truncatedNormal[2] + truncatedNormal[3];
                        return new int[] { min, max };
                    }
                case TableTypes.Triangular:
                    {
                        //triangular has 3 columns
                        int min = triangular[0];
                        //add the second and third col widths together
                        int max = triangular[1] + triangular[2];
                        return new int[] {min, max};
                    }
                case TableTypes.Normal:
                    {
                        //if we get here then the normal col widths were bigger than the uniform widths
                        int min = normalWidths[0];
                        int max = normalWidths[1];
                        return new int[] { min, max };
                    }
                default:
                    {
                        return uniformWidths;
                    }
            }
        }

        /// <summary>
        /// Gets the column widths for the "y" columns for a triangular distribution. A table type is passed in
        /// that represents the widest table type in the editors list of tables. If the table type is triangular
        /// or a narrower type then this will return the dictionary values for Triangular
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int[] GetComputedTriangularWidths(TableTypes type)
        {
            //triangular columns: Most Likely, Min, Max

            //you only have to check for types that have more columns than yourself.
            int[] triangularWidths = DYNAMIC_COL_WIDTHS[TableTypes.Triangular];
            int[] truncatedNormal = DYNAMIC_COL_WIDTHS[TableTypes.TruncatedNormal];
            int[] scaledBeta = DYNAMIC_COL_WIDTHS[TableTypes.Beta];

            switch (type)
            {
                case TableTypes.Beta:
                    {
                        //beta has 4 columns
                        int mostLikely = scaledBeta[0];
                        int min = scaledBeta[1];
                        //add the third and fourth col widths together
                        int max = scaledBeta[2] + scaledBeta[3];
                        return new int[] { mostLikely, min, max };
                    }
                case TableTypes.TruncatedNormal:
                    {
                        //truncatedNormal has 4 columns
                        int mostLikely = truncatedNormal[0];
                        int min = truncatedNormal[1];
                        //add the third and fourth col widths together
                        int max = truncatedNormal[2] + truncatedNormal[3];
                        return new int[] { mostLikely, min, max };
                    }
                default:
                    {
                        return triangularWidths;
                    }
            }
        }

        public static int[] GetComputedNormalWidths(TableTypes type)
        {
            //normal table has two columns: mean, st dev
            int[] normalWidths = DYNAMIC_COL_WIDTHS[TableTypes.Normal];
            int[] uniformWidths = DYNAMIC_COL_WIDTHS[TableTypes.Uniform];
            int[] triangularWidths = DYNAMIC_COL_WIDTHS[TableTypes.Triangular];
            int[] truncatedNormal = DYNAMIC_COL_WIDTHS[TableTypes.TruncatedNormal];
            int[] scaledBeta = DYNAMIC_COL_WIDTHS[TableTypes.Beta];

            switch (type)
            {
                case TableTypes.Beta:
                    {
                        //beta has 4 columns
                        int mean = scaledBeta[0];
                        //add the second, third and fourth col widths together
                        int stDev = scaledBeta[1] + scaledBeta[2] + scaledBeta[3];
                        return new int[] { mean, stDev };
                    }
                case TableTypes.TruncatedNormal:
                    {
                        //truncatedNormal has 4 columns
                        int mean = truncatedNormal[0];
                        //add the second, third and fourth col widths together
                        int stDev = truncatedNormal[1] + truncatedNormal[2] + truncatedNormal[3];
                        return new int[] { mean, stDev };
                    }
                case TableTypes.Triangular:
                    {
                        //triangular has 3 columns
                        int mean = triangularWidths[0];
                        //add the second and third col widths together
                        int stDev = triangularWidths[1] + triangularWidths[2];
                        return new int[] { mean, stDev };
                    }
                case TableTypes.Uniform:
                    {
                        //if we get here then the uniform col widths were bigger than the normal widths
                        int mean = uniformWidths[0];
                        int stDev = uniformWidths[1];
                        return new int[] { mean, stDev };
                    }
                default:
                    {
                        return normalWidths;
                    }
            }
        }
        public static int[] DynamicColumnWidths(TableTypes type)
        {
            return DYNAMIC_COL_WIDTHS[type];
        }
        /// <summary>
        /// This is to track the widest table in the editor.
        /// </summary>
        public enum TableTypes
        {
            None, Normal, Triangular, Uniform, TruncatedNormal, Beta
        }

        /// <summary>
        /// This is the line with the "Y" that goes over the dynamic columns and the dist type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GetTableTopWidth(TableTypes type)
        {
            return TotalDynamicColumnWidths(type) + COL_DIST_WIDTH;
        }
        public static int TotalDynamicColumnWidths(TableTypes type)
        {
            return DYNAMIC_COL_WIDTHS[type].Sum();

        }
        public static int TotalColumnWidths(TableTypes type)
        {
            int totalDynamicWidths = DYNAMIC_COL_WIDTHS[type].Sum();
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
        /// <summary>
        /// Returnes the table type of the widest table from the rows passed in.
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static TableTypes GetTableTypeForRows(List<CoordinatesFunctionRowItem> rows)
        {
            //List<DistributionType> typesInRows = rows.GroupBy(rowItem => rowItem.SelectedDistributionType).Select(group => group.First()).ToList();
            List<DistributionType> uniqueDistributionTypes = GetUniqueListOfDistributionsFromRows(rows);
            //I am determining the "widest table type" in the following order. In the case of ties, I return the type that
            //is the widest.

            //types and number of y columns:
            //Scaled Beta: 4
            //truncated Normal: 4
            //triangular:3
            //Normal: 2
            //uniform: 2
            //None: 1

            //start with the type that has the most columns. Once i determine that, then 
            //we are done.
            if (uniqueDistributionTypes.Contains(DistributionType.Beta4Parameters) && uniqueDistributionTypes.Contains(DistributionType.TruncatedNormal))
            {
                //there is a tie. They both have 4 columns
                //return the widest one
                int betaDistYColWidths = DYNAMIC_COL_WIDTHS[TableTypes.Beta].Sum();
                int truncNormalDistYColWidths = DYNAMIC_COL_WIDTHS[TableTypes.TruncatedNormal].Sum();
                return betaDistYColWidths > truncNormalDistYColWidths ? TableTypes.Beta : TableTypes.TruncatedNormal;
            }
            else if(uniqueDistributionTypes.Contains(DistributionType.Beta4Parameters))
            {
                return TableTypes.Beta;
            }
            else if(uniqueDistributionTypes.Contains(DistributionType.TruncatedNormal))
            {
                return TableTypes.TruncatedNormal;
            }
            else if (uniqueDistributionTypes.Contains( DistributionType.Triangular))
            {
                return TableTypes.Triangular;
            }
            else if (uniqueDistributionTypes.Contains( DistributionType.Normal) && uniqueDistributionTypes.Contains(DistributionType.Uniform))
            {
                //there is a tie. They both have 2 columns
                //return the widest one
                int normalDistYColWidths = DYNAMIC_COL_WIDTHS[TableTypes.Normal].Sum();
                int uniformDistYColWidths = DYNAMIC_COL_WIDTHS[TableTypes.Uniform].Sum();
                return normalDistYColWidths > uniformDistYColWidths ? TableTypes.Normal : TableTypes.Uniform;
            }
            else if (uniqueDistributionTypes.Contains(DistributionType.Normal))
            {
                return TableTypes.Normal;
            }
            else if(uniqueDistributionTypes.Contains(DistributionType.Uniform))
            {
                return TableTypes.Uniform;
            }
            else
            {
                return TableTypes.None;
            }
        }

        
        //private static bool AreRowsAllNone(List<CoordinatesFunctionRowItem> rows)
        //{
        //    bool retval = true;
        //    foreach (CoordinatesFunctionRowItem row in rows)
        //    {
        //        if (!row.SelectedDistributionType.Equals(DistributionType.Constant))
        //        {
        //            retval = false;
        //            break;
        //        }
        //    }
        //    return retval;
        //}

        private static bool ContainsRowWithType(List<CoordinatesFunctionRowItem> rows, DistributionType type)
        {
            bool retval = false;
            foreach (CoordinatesFunctionRowItem row in rows)
            {
                if (row.SelectedDistributionType.Equals(type))
                {
                    retval = true;
                    break;
                }
            }
            return retval;
        }

    }
}
