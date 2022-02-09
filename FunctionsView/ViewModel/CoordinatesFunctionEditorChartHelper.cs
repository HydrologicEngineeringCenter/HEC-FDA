using HEC.Plotting.SciChart2D.DataModel;
using paireddata;
using Statistics;
using System.Collections.Generic;
using System.Windows.Media;

namespace FunctionsView.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// It would be nice to add a combobox that would allow the user to specify the probability lines that they would like. This class
    /// has been designed to make that pretty easy. Just add more probabilities to the PROBABILITIES array and add the associated styles
    /// to the styles dictionary and make sure the names are in the create names method.
    /// </remarks>
    public class CoordinatesFunctionEditorChartHelper
    {
        //if you add to the probabilities just add the series name to "CreateSeriesName" method
        //add probabilities from high to low so that they show up in the legend that way
        private readonly double[] PROBABILITIES = new double[] { .95, .50, .05 };
        private Dictionary<double, ChartStyle> symbolDictionay = new Dictionary<double, ChartStyle>();
        private List<Color> _lineColors = new List<Color>() { Colors.Black, Colors.Red, Colors.Blue, 
             Colors.DarkGoldenrod, Colors.Purple, Colors.Green, Colors.DarkOrange, Colors.Aquamarine,Colors.DarkGray, Colors.Black, Colors.Red, Colors.Blue,
             Colors.DarkGoldenrod, Colors.Purple, Colors.Green, Colors.DarkOrange, Colors.Aquamarine,Colors.DarkGray };


        private readonly string _ChartTitle = "ChartTitle";
        private readonly string _XLabel = "XLabel - cody";
        private readonly string _YLabel = "YLabel - cody";
        private readonly UncertainPairedData _function;

        public CoordinatesFunctionEditorChartHelper(UncertainPairedData function, string xlabel, string ylabel)
        {
            _XLabel = xlabel;
            _YLabel = ylabel;
            LoadStylesDictionary();
            _function = function;
        }

        private void LoadStylesDictionary()
        {
            SciLineData dataStyle_05 = new NumericLineData()
            {
                SymbolSize = 8,
                SymbolType = SymbolType.Triangle,
                //LineStyle = LineStyle.SmallDashes
                StrokeDashArray = new double[] { 6, 6 }
            };
            symbolDictionay.Add(.05, new ChartStyle(dataStyle_05));

            SciLineData dataStyle_50 = new NumericLineData()
            {
                SymbolSize = 8,
                SymbolType = SymbolType.Circle,
                //LineStyle = LineStyle.SmallDashes
                StrokeDashArray = new double[] { 3,0 }
            };

            symbolDictionay.Add(.50, new ChartStyle(dataStyle_50));

            SciLineData dataStyle_95 = new NumericLineData()
            {
                SymbolSize = 8,
                SymbolType = SymbolType.InverseTriangle,
                //LineStyle = LineStyle.SmallDashes
                StrokeDashArray = new double[] { 6, 6 }
            };
            //LineStyleDashArrays.ConvertLineStyleToDashArrays(LineStyle.LargeDashes);

            symbolDictionay.Add(.95, new ChartStyle(dataStyle_95));
        }

        public List<SciLineData> CreateLineData(bool logXAxis = false, bool logYAxis = false, bool probabilityXAxis = false)
        {
            
            
            
                //this is the case that there is only one table
                //if the function is distributed then we will create a series for each of the probabilities defined at the top
                //of this class
                //IOrdinateEnum type = _function.DistributionType;
                //if (type != IOrdinateEnum.Constant)
                //{
                //    return CreateDistributedLineData(_function.Coordinates, _function.Interpolator, _lineColors[0], logXAxis, logYAxis, probabilityXAxis);
                //}
                //else
                //{
                    //if the function is not distributed then we only want to display a single 
                    //line. We don't want to use the probabilities.
                    //double[] xValues = new double[_function.Coordinates.Count];
                    //double[] yValues = new double[_function.Coordinates.Count];

                    //for (int i = 0; i < _function.Coordinates.Count; i++)
                    //{
                    //    ICoordinate coord = _function.Coordinates[i];
                    //    xValues[i] = coord.X.Value();
                    //    yValues[i] = coord.Y.Value();
                    //}

                    SciLineData data = CreateLineData(_function.xs(), _function.ys(), "Constant", .5, _lineColors[0], logXAxis, logYAxis, probabilityXAxis);
                    return new List<SciLineData>() { data };

                //}
            
        }

        //private void AddTheConnetingCoordinates(List<SciLineData> lineData)
        //{
        //    for(int i = 0;i< lineData.Count - 1;i++)
        //    {
        //        SciLineData data1 = lineData[i];
        //        SciLineData data2 = lineData[i + 1];
        //        //i need to determine the type of the line data (points, lines, or digital aka none, linear, piecewise)
        //        if(data1.PlotType == PlotType.Scatter)
        //        {
        //            ((NumericLineData)data1).XData
        //            //data1.XArray.SetValue(data1.XArray.Length;
        //        }
        //    }
        //}


        /// <summary>
        /// This will generate a list of sciline data for a single table. There will be a line for every probability
        /// that is defined at the top of this class.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="interpolator"></param>
        /// <returns></returns>
        //private List<SciLineData> CreateDistributedLineData(List<ICoordinate> coordinates, Color lineColor,bool logXAxis = false, bool logYAxis = false, bool probabilityXAxis = false)
        //{
        //   // List<ICoordinate> coordinates = function.Coordinates;
            
        //    int numberOfYValues = PROBABILITIES.Length;
        //    double[] xValues = new double[coordinates.Count];
        //    List<double[]> yArrays = new List<double[]>();


        //    //initialize the arrays. 
        //    //each probability represents a line.
        //    for(int i = 0;i<PROBABILITIES.Length;i++)
        //    {
        //        yArrays.Add(new double[coordinates.Count]);
        //    }
           

        //    //var x = new double[func.Coordinates.Count];
        //    //var y_05 = new double[func.Coordinates.Count];
        //    //var y_50 = new double[func.Coordinates.Count];
        //    //var y_95 = new double[func.Coordinates.Count];

        //    for (int i = 0; i < coordinates.Count; i++)
        //    {
        //        ICoordinate coord = coordinates[i];
        //        xValues[i] = coord.X.Value();
        //        for(int j = 0;j<PROBABILITIES.Length;j++)
        //        {
        //            yArrays[j][i] = coord.Y.Value( PROBABILITIES[j]);
        //        }
        //    }

        //    List<SciLineData> lineDatas = new List<SciLineData>();
        //    for(int i = 0;i< yArrays.Count;i++)
        //    {
        //        string seriesName = CreateSeriesName(PROBABILITIES[i]);
        //        lineDatas.Add(CreateLineData(xValues, yArrays[i], seriesName, PROBABILITIES[i], lineColor, logXAxis, logYAxis, probabilityXAxis));
        //    }
        //    return lineDatas;
        //}

        

        private string CreateSeriesName(double probability)
        {
            switch(probability)
            {
                case .05:
                    {
                        return "5%";
                    }
                case .1:
                    {
                        return "10%";
                    }
                case .2:
                    {
                        return "20%";
                    }
                case .25:
                    {
                        return "25%";
                    }
                case .3:
                    {
                        return "30%";
                    }
                case .4:
                    {
                        return "40%";
                    }
                case .50:
                    {
                        return "50%";
                    }
                case .6:
                    {
                        return "60%";
                    }
                case .7:
                    {
                        return "70%";
                    }
                case .75:
                    {
                        return "75%";
                    }
                case .8:
                    {
                        return "80%";
                    }
                case .9:
                    {
                        return "90%";
                    }
                case .95:
                    {
                        return "95%";
                    }
                default:
                    {
                        return "Unknown Series";
                    }
            }
        }

        private SciLineData CreateLineData(double[] xs, IDistribution[] ys, string seriesName, double probability, Color color,bool logXAxis = false,  bool logYAxis = false, bool probabilityXAxis = false)
        {
            bool assignPredefinedColor = false;
            if(color != Colors.Transparent)
            {
                assignPredefinedColor = true;
            }
            NumericLineData lineData = null;

            List<double> yVals = new List<double>();
            foreach(IDistribution y in ys)
            {
                yVals.Add(y.CDF(probability));
            }

            lineData = new NumericLineData(xs, yVals.ToArray(), _ChartTitle, seriesName, _XLabel, _YLabel, PlotType.Line);
            if (symbolDictionay.ContainsKey(probability))
            {
                ChartStyle style = symbolDictionay[probability];
                lineData.SymbolSize = style.SymbolSize;
                lineData.SymbolType = style.SymbolType;
                lineData.StrokeDashArray = style.StrokeDashArray;
                if (assignPredefinedColor)
                {
                    lineData.SymbolFillColor = color;
                    lineData.SymbolStrokeColor = color;
                    lineData.StrokeColor = color;
                }
            }
                    
            lineData.UseLogYAxis = logYAxis;
            lineData.UseLogXAxis = logXAxis;
            lineData.UseProbabilityXAxis = probabilityXAxis;
            return lineData;
        }
    }
}
