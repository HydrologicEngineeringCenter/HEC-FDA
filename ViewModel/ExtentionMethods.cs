using Functions;
using Functions.Ordinates;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ViewModel
{
    public static class ExtentionMethods
    {
        /// <summary>
        /// this is dumb, but for some reason anytime a window is opened in all of FDA it
        /// recreates the studyVM. This static prop in a static class fixes it
        /// </summary>
        public static bool IsStudyOpen { get; set; }
        public static void toSqliteTable(this IFdaFunction curve, string tableName)
        {
            //figure out the distribution type.
            //create the appropriate columns
            string[] colNames = null;
            Type[] colTypes = null;
            List<ICoordinate> coords = curve.Coordinates;
            object[][] rows = new object[coords.Count][];
            switch (curve.DistributionType)
            {
                //todo: Refactor: this should be log normal not normal.
                //case Functions.DistributionType.Normal:// Statistics.UncertainCurveDataCollection.DistributionsEnum.LogNormal:
                //    colNames = new string[] { "X Value", "Mean (of log)", "Standard Deviation (of log)" };
                //    colTypes = new Type[] { typeof(double), typeof(double), typeof(double) };
                //    for (int i = 0; i < curve.Count; i++)
                //    {
                //        rows[i] = new object[] { curve.get_X(i), ((Statistics.LogNormal)curve.get_Y(i)).GetMean, ((Statistics.LogNormal)curve.get_Y(i)).GetStDev };
                //    }
                //    break;
                case Functions.IOrdinateEnum.Normal:
                    colNames = new string[] { "X Value", "Mean", "Standard Deviation" };
                    colTypes = new Type[] { typeof(double), typeof(double), typeof(double) };
                    for (int i = 0; i < coords.Count; i++)
                    {
                        ICoordinate coord = coords[i];
                        //the coordinates should be CoordinateVariableY
                        //the Y value should be a distribution
                        IDistributedOrdinate distValue = (IDistributedOrdinate)coord.Y;
                        rows[i] = new object[] { coord.X, distValue.Mean, distValue.StandardDeviation };
                    }
                    break;
                case Functions.IOrdinateEnum.Triangular:
                    colNames = new string[] { "X Value", "Minimum", "Most Likely", "Maximum" };
                    colTypes = new Type[] { typeof(double), typeof(double), typeof(double), typeof(double) };
                    for (int i = 0; i < coords.Count; i++)
                    {
                        ICoordinate coord = coords[i];
                        IDistributedOrdinate distValue = (IDistributedOrdinate)coord.Y;
                        rows[i] = new object[] { coord.X, distValue.Range.Min, distValue.Mean, distValue.Range.Max };
                    }
                    break;
                case Functions.IOrdinateEnum.Uniform:
                    colNames = new string[] { "X Value", "Minimum", "Maximum" };
                    colTypes = new Type[] { typeof(double), typeof(double), typeof(double) };
                    for (int i = 0; i < coords.Count; i++)
                    {
                        ICoordinate coord = coords[i];
                        IDistributedOrdinate distValue = (IDistributedOrdinate)coord.Y;
                        rows[i] = new object[] { coord.X, distValue.Range.Min, distValue.Range.Max };
                    }
                    break;
                case Functions.IOrdinateEnum.Constant:
                    colNames = new string[] { "X Value", "Y Value" };
                    colTypes = new Type[] { typeof(double), typeof(double) };
                    for (int i = 0; i < coords.Count; i++)
                    {
                        ICoordinate coord = coords[i];
                        IDistributedOrdinate distValue = (IDistributedOrdinate)coord.Y;
                        rows[i] = new object[] { coord.X, distValue.Mean };
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
                DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
                for (int i = 0; i < rows.Count(); i++)
                {
                    tbl.AddRow(rows[i]);
                }
                tbl.ApplyEdits();
            }
        }
        
        //todo: Refactor: commenting out.
        //public static void fromSqliteTable(this IFdaFunction curve, string tableName)
        //{
        //    if (!Storage.Connection.Instance.IsConnectionNull)
        //    {
        //        if (!Storage.Connection.Instance.TableNames().Contains(tableName))
        //        {
        //            throw new ArgumentException("Table " + tableName + " does not exist.");
        //        }
        //    }
        //    else
        //    {
        //        //error.
        //        throw new ArgumentNullException("The sqlite connection has not yet been initialized.");
        //    }
        //    DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
        //    List<object[]> rows = tbl.GetRows(0, tbl.NumberOfRows - 1);
        //    if (curve.Count > 0)
        //    {
        //        curve.RemoveRange(0, curve.Count);
        //    }
        //    if (tbl.ColumnNames.Count() == 2)
        //    {
        //        //none.
        //        for (int i = 0; i < rows.Count; i++)
        //        {
        //            curve.Add((double)rows[i][0], new Statistics.None((double)rows[i][1]));
        //        }
        //    }
        //    else if (tbl.ColumnNames.Count() == 3)
        //    {
        //        if (tbl.ColumnNames[1] == "Mean")
        //        {
        //            //normal
        //            for (int i = 0; i < rows.Count; i++)
        //            {
        //                curve.Add((double)rows[i][0], new Statistics.Normal((double)rows[i][1], (double)rows[i][2]));
        //            }
        //        }
        //        else if(tbl.ColumnNames[1] == "Mean (of log)")
        //        {
        //            //log normal
        //            for (int i = 0; i < rows.Count; i++)
        //            {
        //                curve.Add((double)rows[i][0], new Statistics.LogNormal((double)rows[i][1], (double)rows[i][2]));
        //            }
        //        }
        //        else if(tbl.ColumnNames[1] == "Maximum")
        //        {
        //            //uniform
        //            for (int i = 0; i < rows.Count; i++)
        //            {
        //                curve.Add((double)rows[i][0], new Statistics.Uniform((double)rows[i][1], (double)rows[i][2]));
        //            }
        //        }
        //    }
        //    else if (tbl.ColumnNames.Count() == 4)
        //    {
        //        //triangular
        //        for (int i = 0; i < rows.Count; i++)
        //        {
        //            curve.Add((double)rows[i][0], new Statistics.Triangular((double)rows[i][1], (double)rows[i][3], (double)rows[i][2]));
        //        }
        //    }
        //    else
        //    {
        //        throw new ArgumentException("too many columns in this table model. ");
        //    }
        //}


        #region write curve to xml
            //todo: Refactor: Commenting out this whole region
        //public static string CreateXMLCurveString(DistributionType distType, ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.IDistribution> YValues)
        //{
        //    switch (distType)
        //    {
        //        case DistributionType.Normal:
        //            return ExtentionMethods.CreateNormalDistributionXML(XValues, YValues);
        //        case DistributionType.Triangular:
        //            return ExtentionMethods.CreateTriangularDistributionXML(XValues, YValues);
        //        case DistributionType.Uniform:
        //            return ExtentionMethods.CreateUniformDistributionXML(XValues, YValues);
        //        case DistributionType.None:
        //            return ExtentionMethods.CreateNoneDistributionXML(XValues, YValues);

        //    }
        //    return "";
        //}


        //public static string CreateNormalDistributionXML(ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.IDistribution> YValues)
        //{
        //    XElement xElement = new XElement("NormalDistribution");
        //    for (int j = 0; j < XValues.Count; j++)
        //    {
        //        XElement ordinateElement = new XElement("Ordinate");

        //        ordinateElement.SetAttributeValue("x", XValues[j]);
        //        ordinateElement.SetAttributeValue("mean", ((Statistics.Normal)YValues[j]).GetMean);
        //        ordinateElement.SetAttributeValue("stDev", ((Statistics.Normal)YValues[j]).GetStDev);

        //        xElement.Add(ordinateElement);
        //    }
        //    return xElement.ToString();
        //}


        //public static string CreateTriangularDistributionXML(ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        //{
        //    XElement xElement = new XElement("TriangularDistribution");
        //    for (int j = 0; j < XValues.Count; j++)
        //    {
        //        XElement ordinateElement = new XElement("Ordinate");

        //        ordinateElement.SetAttributeValue("x", XValues[j]);
        //        ordinateElement.SetAttributeValue("min", ((Statistics.Triangular)YValues[j]).getMin);
        //        ordinateElement.SetAttributeValue("max", ((Statistics.Triangular)YValues[j]).getMax);
        //        ordinateElement.SetAttributeValue("mostLikely", ((Statistics.Triangular)YValues[j]).getMostlikely);

        //        xElement.Add(ordinateElement);
        //    }
        //    return xElement.ToString();
        //}




        //public static string CreateUniformDistributionXML(ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        //{
        //    XElement xElement = new XElement("UniformDistribution");
        //    for (int j = 0; j < XValues.Count; j++)
        //    {
        //        XElement ordinateElement = new XElement("Ordinate");

        //        ordinateElement.SetAttributeValue("x", XValues[j]);
        //        ordinateElement.SetAttributeValue("min", ((Statistics.Uniform)YValues[j]).GetMin);
        //        ordinateElement.SetAttributeValue("max", ((Statistics.Uniform)YValues[j]).GetMax);

        //        xElement.Add(ordinateElement);
        //    }
        //    return xElement.ToString();
        //}


        //public static string CreateNoneDistributionXML(ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        //{

        //    XElement xElement = new XElement("NoneDistribution");
        //    for (int j = 0; j < XValues.Count; j++)
        //    {
        //        XElement ordinateElement = new XElement("Ordinate");

        //        ordinateElement.SetAttributeValue("x", XValues[j]);
        //        ordinateElement.SetAttributeValue("y", ((Statistics.None)YValues[j]).GetCentralTendency);

        //        xElement.Add(ordinateElement);
        //    }
        //    return xElement.ToString();
        //}

        //#endregion


        //#region read curve from xml string


        //public static Statistics.UncertainCurveIncreasing GetCurveFromXMLString(string xmlString, Statistics.UncertainCurveDataCollection.DistributionsEnum distType)
        //{
            
        //        if (distType == Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal )
        //        {
        //            return GetNormalDistributionFromXML(xmlString);
        //        }
        //        else if (distType == Statistics.UncertainCurveDataCollection.DistributionsEnum.None)
        //        {
        //            return  GetNoneDistributionFromXML(xmlString);
        //        }
        //        else if (distType == Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular)
        //        {
        //            return GetTriangularDistributionFromXML(xmlString);
        //        }
        //        else if (distType == Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform)
        //        {
        //            return GetUniformDistributionFromXML(xmlString);
        //        }
        //    return null;
            
        //}

        //public static Statistics.UncertainCurveIncreasing GetNormalDistributionFromXML(string xmlString)
        //{
        //    List<double> xValues = new List<double>();
        //    List<Statistics.ContinuousDistribution> yValues = new List<Statistics.ContinuousDistribution>();

        //    XDocument xDoc = XDocument.Parse(xmlString);
        //    XElement normalElement = xDoc.Element("NormalDistribution");
        //    foreach (XElement ele in normalElement.Elements("Ordinate"))
        //    {
        //        xValues.Add((double)ele.Attribute("x"));
        //        yValues.Add(new Statistics.Normal(Convert.ToDouble(ele.Attribute("mean").Value), Convert.ToDouble(ele.Attribute("stDev").Value)));

        //    }

        //    return new Statistics.UncertainCurveIncreasing(xValues, yValues, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal);
        //}

        //public static Statistics.UncertainCurveIncreasing GetTriangularDistributionFromXML(string xmlString)
        //{
        //    List<double> xValues = new List<double>();
        //    List<Statistics.ContinuousDistribution> yValues = new List<Statistics.ContinuousDistribution>();

        //    XDocument xDoc = XDocument.Parse(xmlString);
        //    XElement normalElement = xDoc.Element("TriangularDistribution");
        //    foreach (XElement ele in normalElement.Elements("Ordinate"))
        //    {
        //        xValues.Add((double)ele.Attribute("x"));
        //        yValues.Add(new Statistics.Triangular(Convert.ToDouble(ele.Attribute("min").Value), Convert.ToDouble(ele.Attribute("max").Value), Convert.ToDouble(ele.Attribute("mostLikely").Value)));
        //    }
        //    return new Statistics.UncertainCurveIncreasing(xValues, yValues, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular);
        //}

        //public static Statistics.UncertainCurveIncreasing GetUniformDistributionFromXML(string xmlString)
        //{
        //    List<double> xValues = new List<double>();
        //    List<Statistics.ContinuousDistribution> yValues = new List<Statistics.ContinuousDistribution>();

        //    XDocument xDoc = XDocument.Parse(xmlString);
        //    XElement normalElement = xDoc.Element("UniformDistribution");
        //    foreach (XElement ele in normalElement.Elements("Ordinate"))
        //    {
        //        xValues.Add((double)ele.Attribute("x"));
        //        yValues.Add(new Statistics.Uniform(Convert.ToDouble(ele.Attribute("min").Value), Convert.ToDouble(ele.Attribute("max").Value)));
        //    }
        //    return new Statistics.UncertainCurveIncreasing(xValues, yValues, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform);
        //}

        //public static Statistics.UncertainCurveIncreasing GetNoneDistributionFromXML(string xmlString)
        //{
        //    List<double> xValues = new List<double>();
        //    List<Statistics.ContinuousDistribution> yValues = new List<Statistics.ContinuousDistribution>();

        //    XDocument xDoc = XDocument.Parse(xmlString);
        //    XElement normalElement = xDoc.Element("NoneDistribution");
        //    foreach (XElement ele in normalElement.Elements("Ordinate"))
        //    {
        //        xValues.Add((double)ele.Attribute("x"));
        //        yValues.Add(new Statistics.None(Convert.ToDouble(ele.Attribute("y").Value)));
        //    }
        //    return new Statistics.UncertainCurveIncreasing(xValues, yValues, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        //}

        #endregion


    }

}
