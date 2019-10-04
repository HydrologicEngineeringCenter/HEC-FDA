using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Fda.Output
{
    /// <summary>
    /// Interaction logic for DamageWithUncertainty.xaml
    /// </summary>
    public partial class DamageWithUncertainty : Window
    {

        private PlotModel _UncertaintyPlotModel;
        private FdaModel.ComputationPoint.Outputs.Result _Result;
        private List<Statistics.Histogram> _ListOfHistograms;
        private List<double> _Damages;

        public List<Statistics.Histogram> ListOhHistograms
        {
            get { return _ListOfHistograms; }
            set { _ListOfHistograms = value; }
        }

        public FdaModel.ComputationPoint.Outputs.Result Result
        {
            get { return _Result; }
            set { _Result = value; }
        }
        public PlotModel UncertaintyPlotModel
        {
            get { return _UncertaintyPlotModel; }
            set { _UncertaintyPlotModel = value; }
        }
        public DamageWithUncertainty(FdaModel.ComputationPoint.Outputs.Result result)
        {
            Result = result;

            calculateData();
            setUpPlot();
            InitializeComponent();

        }

        private void calculateData()
        {
            List<FdaModel.Utilities.Messager.ErrorMessage> errors = new List<FdaModel.Utilities.Messager.ErrorMessage>();

            _ListOfHistograms = new List<Statistics.Histogram>();
            _Damages = new List<double>();
            int numberOfHistograms = 20;
            //get the ranges
            double damageMin= double.MaxValue;
            double damageMax = 0;

            for (int i = 0; i < Result.Realizations.Count; i++)
            {
                foreach (FdaModel.Functions.BaseFunction func in Result.Realizations[i].Functions)
                {
                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
                    {
                        FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction ord = func.GetOrdinatesFunction();
                        float minOrd = 0;                        

                        foreach (float f in ord.Function.YValues)
                        {
                            if(f>0)
                            {
                                minOrd = f;
                                break;
                            }
                        }
                        if (minOrd < damageMin)
                        {
                            damageMin = minOrd;
                        }
                        if (ord.Function.YValues.Max() > damageMax)
                        {
                            damageMax = ord.Function.YValues.Max();
                        }
                    }
                }
            }

            double interval = (damageMax - damageMin) / (numberOfHistograms - 1);

            for(int j = 0; j<numberOfHistograms; j++)
            {
                _Damages.Add(damageMin + (interval * j));
                _ListOfHistograms.Add(new Statistics.Histogram(10000, 0, 1, false));
            }
         




            for (int i = 0; i < Result.Realizations.Count; i++)
            {

                foreach (FdaModel.Functions.BaseFunction func in Result.Realizations[i].Functions)
                {
                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
                    {
                        for(int j = 0; j<numberOfHistograms;j++)
                        {
                            _ListOfHistograms[j].AddObservation(func.GetXfromY(_Damages[j], ref errors));
                        }
                       

                    }


                }

            }


      

           
        }

        private void setUpPlot()
        {

            _UncertaintyPlotModel = new OxyPlot.PlotModel();


            _UncertaintyPlotModel.Title = "Variation in Damage Frequency";

            LinearAxis XAxis = new LinearAxis();

            XAxis.Position = AxisPosition.Bottom;
            XAxis.Title = "Annual Chance Exceedance";
            XAxis.MajorGridlineStyle = LineStyle.Solid;
            XAxis.MinorGridlineStyle = LineStyle.Dash;
            // XAxis.Minimum = _StageMin;
            //XAxis.Maximum = _StageMax;
            XAxis.StartPosition = 1;
            XAxis.EndPosition = 0;

            _UncertaintyPlotModel.Axes.Add(XAxis);

            LinearAxis YAxis = new LinearAxis();
            YAxis.Position = AxisPosition.Left;

            YAxis.Title = "Damage ($)";
            YAxis.MajorGridlineStyle = LineStyle.Solid;
            YAxis.MinorGridlineStyle = LineStyle.Dash;
            //YAxis.Minimum = _FlowMin;
            //YAxis.Maximum = _FlowMax;
            _UncertaintyPlotModel.Axes.Add(YAxis);

            _UncertaintyPlotModel.LegendBackground = OxyColors.White;
            _UncertaintyPlotModel.LegendBorder = OxyColors.DarkGray;
            _UncertaintyPlotModel.LegendPosition = LegendPosition.TopLeft;

            _UncertaintyPlotModel.PlotMargins = new OxyThickness(60, 10, 10, 40);

            List<Series> listOfSeries = getUncertaintyData();

            //add area series
            AreaSeries shadedArea = new AreaSeries();
            shadedArea.Color = OxyColor.FromRgb(141, 182, 195); //blue grey

            LineSeries l1 = (LineSeries)listOfSeries[0];
            LineSeries l2 = (LineSeries)listOfSeries[2];

            foreach (DataPoint d in l1.Points)
            {
                shadedArea.Points.Add(d);

            }
            foreach (DataPoint d in l2.Points)
            {
                shadedArea.Points2.Add(d);

            }

            _UncertaintyPlotModel.Series.Add(shadedArea);

            //add the line series
            foreach (Series s in listOfSeries)
            {   
                _UncertaintyPlotModel.Series.Add(s);
            }

            _UncertaintyPlotModel.InvalidatePlot(true);

        }

        private List<Series> getUncertaintyData()
        {

            LineSeries seriesZero = new LineSeries();
            seriesZero.Color = OxyColors.Green;
            seriesZero.LineStyle = LineStyle.DashDot;
            seriesZero.Title = "5%";
            LineSeries seriesOne = new LineSeries();
            seriesOne.Color = OxyColors.Blue;
            seriesOne.Title = "Mean";
            LineSeries seriesTwo = new LineSeries();
            seriesTwo.Color = OxyColors.Red;
            seriesTwo.LineStyle = LineStyle.Dash;
            seriesTwo.Title = "95%";

            for (int i = 0; i < _Damages.Count; i++)
            {
                seriesZero.Points.Add(new DataPoint(1-_ListOfHistograms[i].ExceedanceValue(.05), _Damages[i]));
                seriesOne.Points.Add(new DataPoint( 1-_ListOfHistograms[i].GetMean, _Damages[i]));
                seriesTwo.Points.Add(new DataPoint( 1-_ListOfHistograms[i].ExceedanceValue(.95), _Damages[i]));
            }
                    
                
                

            

            List<Series> myList = new List<Series>();
            myList.Add(seriesZero);
            myList.Add(seriesOne);
            myList.Add(seriesTwo);
            return myList;

        }
    }
}
