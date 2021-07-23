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
using OxyPlot.Axes;
using OxyPlot;
using ViewModel.Output;
using OxyPlot.Series;

namespace View.Output
{
    /// <summary>
    /// Interaction logic for HistogramViewer.xaml
    /// </summary>
    public partial class HistogramViewer : UserControl
    {

        private OxyPlot.PlotModel _HistogramPlotModel;
        //private Statistics.Histogram _Histogram;
        //private FdaModel.ComputationPoint.Outputs.Result _Result;
        private bool _isEAD;

        //public Statistics.Histogram Histogram
        //{
        //    get { return _Histogram; }
        //    set { _Histogram = value; }
        //}
        public OxyPlot.PlotModel HistogramPlotModel
        {
            get { return _HistogramPlotModel; }
            set { _HistogramPlotModel = value; }
        }

        public HistogramViewer()
        {
            //_isEAD = isEAD;
            //if(isEAD == true)
            //{
            //    Histogram = result.EAD;
               
            //}
            //else
            //{
            //    Histogram = result.AEP;
                

            //}



            InitializeComponent();

            


        }

        private void setUpHistogramPlotModel( int sampleSize, List<HistogramBinVM> bins)
        {

            HistogramPlotModel = new OxyPlot.PlotModel();

            if(_isEAD == true)
            {
                HistogramPlotModel.Title = "Distribution of Expected Annual Damages";

            }
            else
            {
                HistogramPlotModel.Title = "Distribution of Annual Exceedance Probabilities";

            }

            LinearAxis YAxis = new LinearAxis();
            YAxis.Position = AxisPosition.Left;
            YAxis.Title = "Frequency";
            YAxis.FontSize = 15;
            YAxis.MajorGridlineStyle = LineStyle.Solid;
            YAxis.MinorGridlineStyle = LineStyle.Dash;
            //'YAxis.Maximum = 100
            //'YAxis.Minimum = 0
            HistogramPlotModel.Axes.Add(YAxis);

            //CategoryAxis XAxis = new CategoryAxis();

            //XAxis.Position = OxyPlot.Axes.AxisPosition.Bottom;
            //XAxis.Title = "Bin Values ($1,000,000)";
            //XAxis.MajorGridlineStyle = OxyPlot.LineStyle.Solid;
            //XAxis.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
            //HistogramPlotModel.Axes.Add(XAxis);

            //HistogramPlotModel.LegendBackground = OxyPlot.OxyColors.White;
            //HistogramPlotModel.LegendBorder = OxyPlot.OxyColors.DarkGray;
            //HistogramPlotModel.LegendPosition = OxyPlot.LegendPosition.BottomRight;


            OxyPlot.Series.ColumnSeries myColumnSeries = new OxyPlot.Series.ColumnSeries();
            
            CategoryAxis categoryAxis = new CategoryAxis();
            categoryAxis.FontSize = 14;
            categoryAxis.Angle = 60;

            if (_isEAD == true)
            {
                int i = 0;
                
                //foreach (int histValue in Histogram.Histogram)
                foreach(HistogramBinVM bin in bins)
                {
                    double min = bin.Min;
                    double max = bin.Max;
                    double binWidth = bin.BinWidth;
                    double midPoint = bin.MidPoint;
                    //if (histValue == 0)
                    //{
                    //    i++;
                    //    continue;
                    //}


                    int binCount = bin.Count;
                    double percValue = binCount / sampleSize;

                    double lowerBinBoundary = (binWidth * i) +min;
                    double upperBinBoundary = (binWidth * (i + 1)) + min;

                    if(midPoint < upperBinBoundary && midPoint >= lowerBinBoundary)
                    {
                        OxyPlot.Series.ColumnItem nextColumn = new OxyPlot.Series.ColumnItem(percValue);
                        nextColumn.Color = OxyPlot.OxyColor.FromRgb(76,0,153); //purple
                        myColumnSeries.Items.Add(nextColumn); ///Histogram.GetSampleSize

                    }
                    else
                    {
                        OxyPlot.Series.ColumnItem nextColumn = new OxyPlot.Series.ColumnItem(percValue);
                        nextColumn.Color = OxyPlot.OxyColor.FromRgb(141,182,195); //blue grey
                        

                        myColumnSeries.Items.Add(nextColumn); ///Histogram.GetSampleSize

                    }


                    categoryAxis.Labels.Add(String.Format("{0:0,0} - {1:0,0}", lowerBinBoundary, upperBinBoundary)); //'bPMSH.Histogram(bPMSH.Histogram.Count - 1 - i).ToString("0.00"))



                    
                    i++;
                }
            }
            else
            {
                //int i = 0;
                //double sampleSize = Histogram.GetSampleSize;
                //foreach (int histValue in Histogram.Histogram)
                //{
                //    if (histValue == 0)
                //    {
                //        i++;
                //        continue;
                //    }

                    
                //    double myValue = (double)histValue;
                //    double percValue = myValue / sampleSize;

                //    double lowerBinBoundary = (Histogram.BinWidth * i) + Histogram.GetMin;
                //    double upperBinBoundary = (Histogram.BinWidth * (i + 1)) + Histogram.GetMin;

                //    if (Histogram.GetMean < upperBinBoundary && Histogram.GetMean >= lowerBinBoundary)
                //    {
                //        OxyPlot.Series.ColumnItem nextColumn = new OxyPlot.Series.ColumnItem(percValue);
                //        nextColumn.Color = OxyPlot.OxyColor.FromRgb(76, 0, 153); //purple
                //        myColumnSeries.Items.Add(nextColumn); ///Histogram.GetSampleSize

                //    }
                //    else
                //    {
                //        OxyPlot.Series.ColumnItem nextColumn = new OxyPlot.Series.ColumnItem(percValue);
                //        nextColumn.Color = OxyPlot.OxyColor.FromRgb(141, 182, 195); //blue grey
                //        myColumnSeries.Items.Add(nextColumn); ///Histogram.GetSampleSize

                //    }

                //    categoryAxis.Labels.Add(String.Format("{0:.####} - {1:.####}", 1 - lowerBinBoundary,1-upperBinBoundary)); //'bPMSH.Histogram(bPMSH.Histogram.Count - 1 - i).ToString("0.00"))
                //    i++;
                //}

            }


            HistogramPlotModel.Axes.Add(categoryAxis);
            //testing
            var testSeries = new ColumnSeries();
            for(int i = 0;i<10;i++)
            {
             testSeries.Items.Add( new ColumnItem(i));

            }
            HistogramPlotModel.Series.Add(testSeries);
            ///////////////////////////////
            //HistogramPlotModel.Series.Add(myColumnSeries);

            HistogramPlotModel.InvalidatePlot(true);


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HistogramViewerVM vm = (HistogramViewerVM)this.DataContext;
            _isEAD = vm.IsEAD;
            setUpHistogramPlotModel(vm.SampleSize, vm.Bins);

            if (_isEAD == true)
            {
                lbl_MeanValueText.Content = "Mean Expected Annual Damages:";
                lbl_MeanValueText.Width = 255;
                lbl_MeanValue.Content = String.Format("{0:0,0}", vm.EADMean);


            }
            else
            {
                //lbl_MeanValueText.Content = "Mean AEP:";
                lbl_MeanValue.Content = String.Format("{0:.###}", vm.AEPMean);

            }
        }
    }
}
