using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HEC.FDA.ViewModel.Output;

namespace HEC.FDA.View.Output
{
    /// <summary>
    /// Interaction logic for HistogramViewer.xaml
    /// </summary>
    public partial class HistogramViewer : UserControl
    {
        private PlotModel _HistogramPlotModel;
        private bool _isEAD;

        public PlotModel HistogramPlotModel
        {
            get { return _HistogramPlotModel; }
            set { _HistogramPlotModel = value; }
        }

        public HistogramViewer()
        {
            InitializeComponent();
        }

        private void setUpHistogramPlotModel( long sampleSize, List<HistogramBinVM> bins)
        {
            HistogramPlotModel = new PlotModel();

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

            HistogramPlotModel.Axes.Add(YAxis);
            ColumnSeries myColumnSeries = new ColumnSeries();
            
            CategoryAxis categoryAxis = new CategoryAxis();
            categoryAxis.FontSize = 14;
            categoryAxis.Angle = 60;

            if (_isEAD == true)
            {
                int i = 0;
                
                foreach(HistogramBinVM bin in bins)
                {
                    double min = bin.Min;
                    double max = bin.Max;
                    double binWidth = bin.BinWidth;
                    double midPoint = bin.MidPoint;

                    int binCount = bin.Count;
                    double percValue = binCount / sampleSize;

                    double lowerBinBoundary = (binWidth * i) +min;
                    double upperBinBoundary = (binWidth * (i + 1)) + min;

                    if(midPoint < upperBinBoundary && midPoint >= lowerBinBoundary)
                    {
                        ColumnItem nextColumn = new ColumnItem(percValue);
                        nextColumn.Color = OxyColor.FromRgb(76,0,153); //purple
                        myColumnSeries.Items.Add(nextColumn); 

                    }
                    else
                    {
                        ColumnItem nextColumn = new ColumnItem(percValue);
                        nextColumn.Color = OxyColor.FromRgb(141,182,195);
                        myColumnSeries.Items.Add(nextColumn);
                    }
                    categoryAxis.Labels.Add(String.Format("{0:0,0} - {1:0,0}", lowerBinBoundary, upperBinBoundary));
                    i++;
                }
            }
            else
            {
               
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
            //HistogramViewerVM vm = (HistogramViewerVM)this.DataContext;
            //_isEAD = vm.IsEAD;
            //setUpHistogramPlotModel(vm.SampleSize, vm.Bins);

            //if (_isEAD == true)
            //{
            //    lbl_MeanValueText.Content = "Mean Expected Annual Damages:";
            //    lbl_MeanValueText.Width = 255;
            //    lbl_MeanValue.Content = String.Format("{0:0,0}", vm.EADMean);


            //}
            //else
            //{
            //    //lbl_MeanValueText.Content = "Mean AEP:";
            //    lbl_MeanValue.Content = String.Format("{0:.###}", vm.AEPMean);

            //}
        }
    }
}
