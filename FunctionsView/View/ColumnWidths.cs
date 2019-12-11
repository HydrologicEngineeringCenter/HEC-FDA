using FunctionsView.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsView.View
{
    public static class ColumnWidths
    {
        public static int COL_X_WIDTH = 80;
        public static int COL_DIST_WIDTH = 100;
        public static int COL_INTERP_WIDTH = 100;

        private static readonly Dictionary<TableTypes, int[]> DYNAMIC_COL_WIDTHS = new Dictionary<TableTypes, int[]>()
        {
            { TableTypes.None, new int[]{100 } },
            {TableTypes.Normal, new int[]{50,50} },
            { TableTypes.Triangular, new int[]{80,50,50 } },
            { TableTypes.Default, new int[]{80,80,80,80 } },
        };

        public static int[] GetComputedNormalWidths(TableTypes type)
        {
            //there should never be a type with fewer columns than itself
            //i think i only need to handle cases that have more columns, and itself
            int[] normalWidths = DYNAMIC_COL_WIDTHS[TableTypes.Normal];
            int[] triangularWidths = DYNAMIC_COL_WIDTHS[TableTypes.Triangular];

            switch (type)
            {
                case TableTypes.Triangular:
                    {
                        int mean = triangularWidths[0];
                        //add the second and third col widths together
                        int stDev = triangularWidths[1] + triangularWidths[2];
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
        public enum TableTypes
        {
            None, Normal, Triangular, Default
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

        public static TableTypes GetTableTypeForRows(List<CoordinatesFunctionRowItem> rows)
        {
            //start with the type that has the most columns. Once i determine that, then 
            //we are done.

            if (ContainsRowWithType(rows, "Triangular"))
            {
                return TableTypes.Triangular;
            }
            else if (ContainsRowWithType(rows, "Normal"))
            {
                return TableTypes.Normal;
            }
            else if (AreRowsAllNone(rows))
            {
                return TableTypes.None;
            }
            else
            {
                return TableTypes.Default;
            }
        }

        
        private static bool AreRowsAllNone(List<CoordinatesFunctionRowItem> rows)
        {
            bool retval = true;
            foreach (CoordinatesFunctionRowItem row in rows)
            {
                if (!row.SelectedDistributionType.Equals("None"))
                {
                    retval = false;
                    break;
                }
            }
            return retval;
        }

        private static bool ContainsRowWithType(List<CoordinatesFunctionRowItem> rows, string type)
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
