using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel
{
    public static class ExtentionMethods
    {
        public static void toSqliteTable(this Statistics.UncertainCurveDataCollection curve, string tableName)
        {
            //figure out the distribution type.
            //create the appropriate columns
            string[] colNames = null;
            Type[] colTypes = null;
            object[][] rows = new object[curve.Count][];
            switch (curve.Distribution)
            {
                case Statistics.UncertainCurveDataCollection.DistributionsEnum.LogNormal:
                    colNames = new string[] { "X Value", "Mean (of log)", "Standard Deviation (of log)" };
                    colTypes = new Type[] { typeof(double), typeof(double), typeof(double) };
                    for (int i = 0; i < curve.Count; i++)
                    {
                        rows[i] = new object[] { curve.get_X(i), ((Statistics.LogNormal)curve.get_Y(i)).GetMean, ((Statistics.LogNormal)curve.get_Y(i)).GetStDev };
                    }
                    break;
                case Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal:
                    colNames = new string[] { "X Value", "Mean", "Standard Deviation" };
                    colTypes = new Type[] { typeof(double), typeof(double), typeof(double) };
                    for (int i = 0; i < curve.Count; i++)
                    {
                        rows[i] = new object[] { curve.get_X(i), ((Statistics.Normal)curve.get_Y(i)).GetMean, ((Statistics.Normal)curve.get_Y(i)).GetStDev };
                    }
                    break;
                case Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular:
                    colNames = new string[] { "X Value", "Minimum", "Most Likely", "Maximum" };
                    colTypes = new Type[] { typeof(double), typeof(double), typeof(double), typeof(double) };
                    for (int i = 0; i < curve.Count; i++)
                    {
                        rows[i] = new object[] { curve.get_X(i), ((Statistics.Triangular)curve.get_Y(i)).getMin, ((Statistics.Triangular)curve.get_Y(i)).getMostlikely, ((Statistics.Triangular)curve.get_Y(i)).getMax };
                    }
                    break;
                case Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform:
                    colNames = new string[] { "X Value", "Minimum", "Maximum" };
                    colTypes = new Type[] { typeof(double), typeof(double), typeof(double) };
                    for (int i = 0; i < curve.Count; i++)
                    {
                        rows[i] = new object[] { curve.get_X(i), ((Statistics.Uniform)curve.get_Y(i)).GetMin, ((Statistics.Uniform)curve.get_Y(i)).GetMax };
                    }
                    break;
                case Statistics.UncertainCurveDataCollection.DistributionsEnum.None:
                    colNames = new string[] { "X Value", "Y Value" };
                    colTypes = new Type[] { typeof(double), typeof(double) };
                    for (int i = 0; i < curve.Count; i++)
                    {
                        rows[i] = new object[] { curve.get_X(i), ((Statistics.None)curve.get_Y(i)).GetCentralTendency };
                    }
                    break;
                    //case default:
                    //    break;
            }
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                if (Storage.Connection.Instance.TableNames().Contains(tableName))
                {
                    //already exists... delete?
                    Storage.Connection.Instance.DeleteTable(tableName);
                }
                Storage.Connection.Instance.CreateTable(tableName, colNames, colTypes);
                DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
                for (int i = 0; i < rows.Count(); i++)
                {
                    tbl.AddRow(rows[i]);
                }
                tbl.ApplyEdits();
            }
        }
        public static void fromSqliteTable(this Statistics.UncertainCurveDataCollection curve, string tableName)
        {
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                if (!Storage.Connection.Instance.TableNames().Contains(tableName))
                {
                    throw new ArgumentException("Table " + tableName + " does not exist.");
                }
            }
            else
            {
                //error.
                throw new ArgumentNullException("The sqlite connection has not yet been initialized.");
            }
            DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
            List<object[]> rows = tbl.GetRows(0, tbl.NumberOfRows - 1);
            if (curve.Count > 0)
            {
                curve.RemoveRange(0, curve.Count);
            }
            if (tbl.ColumnNames.Count() == 2)
            {
                //none.
                for (int i = 0; i < rows.Count; i++)
                {
                    curve.Add((double)rows[i][0], new Statistics.None((double)rows[i][1]));
                }
            }
            else if (tbl.ColumnNames.Count() == 3)
            {
                if (tbl.ColumnNames[1] == "Mean")
                {
                    //normal
                    for (int i = 0; i < rows.Count; i++)
                    {
                        curve.Add((double)rows[i][0], new Statistics.Normal((double)rows[i][1], (double)rows[i][2]));
                    }
                }
                else if(tbl.ColumnNames[1] == "Mean (of log)")
                {
                    //log normal
                    for (int i = 0; i < rows.Count; i++)
                    {
                        curve.Add((double)rows[i][0], new Statistics.LogNormal((double)rows[i][1], (double)rows[i][2]));
                    }
                }
                else if(tbl.ColumnNames[1] == "Maximum")
                {
                    //uniform
                    for (int i = 0; i < rows.Count; i++)
                    {
                        curve.Add((double)rows[i][0], new Statistics.Uniform((double)rows[i][1], (double)rows[i][2]));
                    }
                }
            }
            else if (tbl.ColumnNames.Count() == 4)
            {
                //triangular
                for (int i = 0; i < rows.Count; i++)
                {
                    curve.Add((double)rows[i][0], new Statistics.Triangular((double)rows[i][1], (double)rows[i][3], (double)rows[i][2]));
                }
            }
            else
            {
                throw new ArgumentException("too many columns in this table model. ");
            }
        }





    }

}
