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
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Xml.Linq;


namespace FdaTester.ModelTester.UnitTests
{
    /// <summary>
    /// Interaction logic for FortranCSharpLogP3Tester.xaml
    /// </summary>
    public partial class FortranCSharpLogP3Tester : Window
    {
        
        private int _objectNumber = 0;
        private List<FortranCSharpLogP3ComparerObject> _myList;
        private OxyPlot.PlotModel _myPlotModel;
        private PlotModel _myWhiskerPlotModel;
        private PlotModel _myPercDiffPlotModel;
        private PlotModel _myStDevPlotModel;


        private string filePath;
        //public List<FortranCSharpLogP3ComparerObject> myList { get; set; } //is this doing anything?


        public OxyPlot.PlotModel myStDevPlotModel
        {
            get { return _myStDevPlotModel; }
            set { _myStDevPlotModel = value; }
        }
        public OxyPlot.PlotModel myWhiskerPlotModel
        {
            get { return _myWhiskerPlotModel; }
            set { _myWhiskerPlotModel = value; }
        }
        public OxyPlot.PlotModel myPlotModel
        {
            get { return _myPlotModel; }
            set { _myPlotModel = value; }
        }
        public OxyPlot.PlotModel myPercDiffPlotModel
        {
            get { return _myPercDiffPlotModel; }
            set { _myPercDiffPlotModel = value; }
        }

        public FortranCSharpLogP3Tester()
        {
            SetUpPlotmodel();
            SetUpWhiskerPlotModel();
            SetUpPercDiffPlotModel();
            SetUpStDevPlotModel();


            InitializeComponent();
            btn_previous.IsEnabled = false;
            //filePath = "C:\\Users\\q0heccdm\\Documents\\FDA2\\Testing Results\\Changing_The_StanDeviation.xml";


        }
        private void btn_view_Click(object sender, RoutedEventArgs e)
        {
            
            if (System.IO.File.Exists(filePath))
            {
                readXML(filePath);
                displayObject(0);

                PlotGrid(0);
                PlotWhiskerPlot();
                PlotStDevPlot();
                // PlotPercDiffPlot();
            }
            else
            {
                return;
            }
        }
        private void displayObject(int displayNumber)
        {
            //clear the old data
            lbl_mean.Content = "";
            lbl_stDev.Content = "";
            lbl_skew.Content = "";
            lbl_POR.Content = "";
            lst_cSharpFlows.Items.Clear();
            lst_fortranFlows.Items.Clear();
            lst_absDiff.Items.Clear();
            lst_percDiff.Items.Clear();
            lst_probs.Items.Clear();
            lbl_totalNumber.Content = "Total # of Tests: " + _myList.Count().ToString();

            //view the data
            lbl_mean.Content = _myList[displayNumber].mean.ToString();
            lbl_stDev.Content = _myList[displayNumber].stDev.ToString();
            lbl_skew.Content = _myList[displayNumber].skew.ToString();
            lbl_POR.Content = _myList[displayNumber].por.ToString();
            lbl_test.Content = _myList[displayNumber].testStatus.ToString();
            txt_objectID.Text = (displayNumber + 1).ToString();

            foreach (double flow in _myList[displayNumber].fortranFlowValues)
            {
                //lst_fortranFlows.Items.Add(flow.ToString());
                lst_fortranFlows.Items.Add(string.Format("{0,10:N}", flow).PadLeft(17));
            }
            foreach (double flow in _myList[displayNumber].cSharpFlowValues)
            {
                lst_cSharpFlows.Items.Add(string.Format("{0,10:N}", flow).PadLeft(17));
            }
            foreach (double flow in _myList[displayNumber].absDiff)
            {
                lst_absDiff.Items.Add(string.Format("{0,10:N}", flow).PadLeft(17));
            }
            foreach (double flow in _myList[displayNumber].percDiff)
            {
                lst_percDiff.Items.Add(flow.ToString("F5").PadLeft(17));
            }
            foreach (double prob in _myList[displayNumber].probabilities)
            {
                lst_probs.Items.Add(prob.ToString().PadLeft(17));
            }
        }

        private void readXML(string filePath)
        {

            //XmlDocument myXmlDocument = new XmlDocument();
            //myXmlDocument.Load(filePath);

            //XmlElement el = (XmlElement)myXmlDocument.SelectSingleNode("Mean");
            _myList = new List<FortranCSharpLogP3ComparerObject>();
            XElement xel = XElement.Load(filePath);
            var elements = xel.Elements();
            foreach (XElement el in elements) //this loops through all the objects
            {
                FortranCSharpLogP3ComparerObject log3 = new FortranCSharpLogP3ComparerObject();
                log3.mean = Convert.ToDouble(el.Attribute("Mean").Value);
                log3.stDev = Convert.ToDouble(el.Attribute("St_Dev").Value);
                log3.skew = Convert.ToDouble(el.Attribute("Skew").Value);
                log3.por = Convert.ToDouble(el.Attribute("POR").Value);
                log3.testStatus = Convert.ToBoolean(el.Attribute("TestStatus").Value);

                var subElements = el.Elements();
                foreach (XElement subEl in subElements)  //this loops through all the elements of each object
                {
                    if (subEl.Name == "Probabilities") //get the probabilities
                    {
                        XAttribute[] probs = subEl.Attributes().ToArray();

                        double[] tempProbs = new double[probs.Count()];
                        for (int i = 0; i < probs.Count(); i++)
                        {
                            tempProbs[i] = Convert.ToDouble(probs[i].Value);

                        }
                        log3.probabilities = tempProbs;
                    }

                    else if (subEl.Name == "CSharpFlows") //get the CSharpFlows
                    {
                        XAttribute[] flows = subEl.Attributes().ToArray();

                        double[] tempFlows = new double[flows.Count()];
                        for (int i = 0; i < flows.Count(); i++)
                        {
                            tempFlows[i] = Convert.ToDouble(flows[i].Value);

                        }
                        log3.cSharpFlowValues = tempFlows;
                    }

                    else if (subEl.Name == "FortranFlows") //get the FortranFlows
                    {
                        XAttribute[] flows = subEl.Attributes().ToArray();

                        double[] tempFlows = new double[flows.Count()];
                        for (int i = 0; i < flows.Count(); i++)
                        {
                            tempFlows[i] = Convert.ToDouble(flows[i].Value);

                        }
                        log3.fortranFlowValues = tempFlows;
                    }

                    else if (subEl.Name == "AbsDiff") //get the absDiff
                    {
                        XAttribute[] diffs = subEl.Attributes().ToArray();

                        double[] tempDiffs = new double[diffs.Count()];
                        for (int i = 0; i < diffs.Count(); i++)
                        {
                            tempDiffs[i] = Convert.ToDouble(diffs[i].Value);

                        }
                        log3.absDiff = tempDiffs;
                    }

                    else if (subEl.Name == "PercDiff") //get the PercDiff
                    {
                        XAttribute[] diffs = subEl.Attributes().ToArray();

                        double[] tempDiffs = new double[diffs.Count()];
                        for (int i = 0; i < diffs.Count(); i++)
                        {
                            tempDiffs[i] = Convert.ToDouble(diffs[i].Value);

                        }
                        log3.percDiff = tempDiffs;
                    }


                }
                //add the object to the list
                _myList.Add(log3);

            }


        }

        private void btn_next_Click(object sender, RoutedEventArgs e)
        {
            ++_objectNumber;
            if (_objectNumber == _myList.Count - 1) { btn_next.IsEnabled = false; }

            if (_objectNumber > 0) { btn_previous.IsEnabled = true; }
            else { btn_previous.IsEnabled = false; }
            //txt_objectID.Text = (_objectNumber + 1).ToString();
            displayObject(_objectNumber);
            PlotGrid(_objectNumber);
        }

        private void btn_previous_Click(object sender, RoutedEventArgs e)
        {
            --_objectNumber;
            if (_objectNumber == 0) { btn_previous.IsEnabled = false; }

            if (_objectNumber < _myList.Count - 1) { btn_next.IsEnabled = true; }
            //txt_objectID.Text = (_objectNumber + 1).ToString();
            displayObject(_objectNumber);
            PlotGrid(_objectNumber);
        }

        private void btn_go_Click(object sender, RoutedEventArgs e)
        {
            //if ((Convert.ToInt32(txt_objectID.Text)) > _myList.Count) { return; }

            //need more validation
            //need to enable the button properly

            int objIndex = Convert.ToInt32(txt_objectID.Text) - 1;

            if (objIndex + 1 > _myList.Count) { return; }
            if (objIndex < 0) { return; }

            if (objIndex > 0)
            {
                btn_previous.IsEnabled = true;
            }
            if (objIndex == 0) { btn_previous.IsEnabled = false; }
            if (objIndex + 1 == _myList.Count) { btn_next.IsEnabled = false; }
            if (objIndex + 1 < _myList.Count) { btn_next.IsEnabled = true; }

            _objectNumber = objIndex;



            displayObject(_objectNumber);
            PlotGrid(_objectNumber);
        }



        private void SetUpPlotmodel()
        {
            _myPlotModel = new OxyPlot.PlotModel();
            _myPlotModel.Title = "Flow Frequency";
            OxyPlot.Axes.LinearAxis YAxis = new OxyPlot.Axes.LinearAxis();
            YAxis.Position = AxisPosition.Left;
            YAxis.Title = "Flow (cfs)";
            YAxis.MajorGridlineStyle = LineStyle.Solid;
            YAxis.MinorGridlineStyle = LineStyle.Dash;
            //YAxis.Maximum = 100;
            //YAxis.Minimum = 0;
            _myPlotModel.Axes.Add(YAxis);
            LinearAxis XAxis = new OxyPlot.Axes.LinearAxis();
            XAxis.Position = OxyPlot.Axes.AxisPosition.Bottom;
            XAxis.Title = "Prob";
            XAxis.MajorGridlineStyle = OxyPlot.LineStyle.Solid;
            XAxis.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
            _myPlotModel.Axes.Add(XAxis);
            _myPlotModel.LegendBackground = OxyPlot.OxyColors.White;
            _myPlotModel.LegendBorder = OxyPlot.OxyColors.DarkGray;
            _myPlotModel.LegendPosition = OxyPlot.LegendPosition.TopRight;
            _myPlotModel.IsLegendVisible = false;
        }
        private void PlotGrid(int objectNumber)
        {
            if (_myPlotModel == null) { _myPlotModel = new OxyPlot.PlotModel(); }
            _myPlotModel.Series.Clear();

            LineSeries cSharpLineSeries = new OxyPlot.Series.LineSeries();
            cSharpLineSeries.Title = "C# Flows";
            //CentralSeries.Title = "Current Structure Value";
            //CentralSeries.MarkerType = OxyPlot.MarkerType.Circle;
            //CentralSeries.Color = OxyPlot.OxyColors.Black;
            _myPlotModel.Series.Add(cSharpLineSeries);
            //add a curve for each necessary element; 
            for (int i = 0; i < _myList[objectNumber].cSharpFlowValues.Count() - 1; i++)
            {
                cSharpLineSeries.Points.Add(new DataPoint(_myList[objectNumber].probabilities[i], _myList[objectNumber].cSharpFlowValues[i]));
                //CentralSeries.Items.Add(New OxyPlot.Series.ColumnItem(i,i));
            }
            //CentralSeries.StrokeThickness = 1;


            LineSeries fortranLineSeries = new OxyPlot.Series.LineSeries();
            fortranLineSeries.Title = "Fortran Flows";
            //CentralSeries.Title = "Current Structure Value";
            //CentralSeries.MarkerType = OxyPlot.MarkerType.Circle;
            //CentralSeries.Color = OxyPlot.OxyColors.Black;
            _myPlotModel.Series.Add(fortranLineSeries);
            //add a curve for each necessary element; 
            for (int i = 0; i < _myList[objectNumber].fortranFlowValues.Count() - 1; i++)
            {
                fortranLineSeries.Points.Add(new DataPoint(_myList[objectNumber].probabilities[i], _myList[objectNumber].fortranFlowValues[i]));
                //CentralSeries.Items.Add(New OxyPlot.Series.ColumnItem(i,i));
            }

            _myPlotModel.IsLegendVisible = true;
            _myPlotModel.InvalidatePlot(true);

        }

        //********************   Whisker Plot ****************************
        private void SetUpWhiskerPlotModel()
        {
            _myWhiskerPlotModel = new PlotModel();
            _myWhiskerPlotModel.Title = "Whisker Plot";
            LinearAxis YAxis = new LinearAxis();
            YAxis.Title = "% Difference Between C# Flows and Fortran Flows";
            YAxis.MajorGridlineStyle = LineStyle.Solid;
            YAxis.MinorGridlineStyle = LineStyle.Dash;
            _myWhiskerPlotModel.Axes.Add(YAxis);
            LinearAxis XAxis = new OxyPlot.Axes.LinearAxis();
            XAxis.Position = OxyPlot.Axes.AxisPosition.Bottom;
            XAxis.Title = "Probability";
            XAxis.MajorGridlineStyle = OxyPlot.LineStyle.Solid;
            XAxis.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
            _myWhiskerPlotModel.Axes.Add(XAxis);
        }

        private void PlotWhiskerPlot()
        {
            BoxPlotSeries myBoxPlotSeries = new BoxPlotSeries();
            myBoxPlotSeries.Fill = OxyColors.CornflowerBlue;
            myBoxPlotSeries.BoxWidth = .01;

            //add the boxes
            for (int i = 0; i < _myList[0].percDiff.Count(); i++)
            {
                List<double> listOfPercents = new List<double>();
                foreach (FortranCSharpLogP3ComparerObject log3 in _myList)
                {

                    listOfPercents.Add(log3.percDiff[i]);
                }
                Statistics.Emperical summaryStats = new Statistics.Emperical(ref listOfPercents);
                double[] outliers = new double[0];

                myBoxPlotSeries.Items.Add(new BoxPlotItem(_myList[0].probabilities[i], summaryStats.GetMin, summaryStats.GetPercentile(0.25), summaryStats.GetMedian, summaryStats.GetPercentile(0.75), summaryStats.GetMax,outliers));

            }

            //add a red line at 0
            LineSeries zeroLine = new LineSeries();
            zeroLine.Color = OxyColors.Red;
            zeroLine.Points.Add(new DataPoint(0, 0));
            zeroLine.Points.Add(new DataPoint(_myList[0].probabilities.Max(), 0));

            _myWhiskerPlotModel.Series.Add(zeroLine);
            _myWhiskerPlotModel.Series.Add(myBoxPlotSeries);

            WhiskerPlot.InvalidatePlot(true);
        }

        //*****************   percDiffPlot **********************************

        private void SetUpPercDiffPlotModel()
        {
            _myPercDiffPlotModel = new PlotModel();
            _myPercDiffPlotModel.Title = "% Diff Plot";
            LinearAxis YAxis = new LinearAxis();
            YAxis.Title = "% Difference Between C# Flows and Fortran Flows";
            YAxis.MajorGridlineStyle = LineStyle.Solid;
            YAxis.MinorGridlineStyle = LineStyle.Dash;
            _myPercDiffPlotModel.Axes.Add(YAxis);
            LinearAxis XAxis = new OxyPlot.Axes.LinearAxis();
            XAxis.Position = OxyPlot.Axes.AxisPosition.Bottom;
            XAxis.Title = "Probability";
            XAxis.MajorGridlineStyle = OxyPlot.LineStyle.Solid;
            XAxis.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
            _myPercDiffPlotModel.Axes.Add(XAxis);


            _myPercDiffPlotModel.LegendBorder = OxyColors.DarkGray;
            _myPercDiffPlotModel.LegendTitle = "LogPearsonIII Test";
            _myPercDiffPlotModel.LegendBackground = OxyColor.FromArgb(150, 255, 255, 255);
            _myPercDiffPlotModel.LegendPosition = LegendPosition.TopRight;
            _myPercDiffPlotModel.IsLegendVisible = false;

        }

        private void PlotPercDiffPlot(int[] csvList)
        {

            _myPercDiffPlotModel.Series.Clear();

            //add the lines
            foreach (int num in csvList)
            {
                LineSeries myPercDiffLineSeries = new LineSeries();
                myPercDiffLineSeries.Title = "Iteration: " + num.ToString();
                for (int j = 0; j < _myList[num - 1].probabilities.Count(); j++)
                {
                    myPercDiffLineSeries.Points.Add(new DataPoint(_myList[num - 1].probabilities[j], _myList[num - 1].percDiff[j]));
                }
                _myPercDiffPlotModel.Series.Add(myPercDiffLineSeries);
                _myPercDiffPlotModel.IsLegendVisible = true;
            }

            //add a red line at 0
            LineSeries zeroLine = new LineSeries();
            zeroLine.Color = OxyColors.Black;
            zeroLine.LineStyle = LineStyle.Dash;
            zeroLine.Points.Add(new DataPoint(0, 0));
            zeroLine.Points.Add(new DataPoint(_myList[0].probabilities.Max(), 0));

            _myPercDiffPlotModel.Series.Add(zeroLine);

            percDiffOxyPlot.InvalidatePlot(true);
        }

        //*********************************************************************

        private void SetUpStDevPlotModel()
        {
            _myStDevPlotModel = new PlotModel();
            _myStDevPlotModel.Title = "% Diff VS Standard Deviation";
            LinearAxis YAxis = new LinearAxis();
            YAxis.Title = "% Difference Between C# Flows and Fortran Flows";
            YAxis.MajorGridlineStyle = LineStyle.Solid;
            YAxis.MinorGridlineStyle = LineStyle.Dash;
            _myStDevPlotModel.Axes.Add(YAxis);
            LinearAxis XAxis = new OxyPlot.Axes.LinearAxis();
            XAxis.Position = OxyPlot.Axes.AxisPosition.Bottom;
            XAxis.Title = "Standard Deviation";
            XAxis.MajorGridlineStyle = OxyPlot.LineStyle.Solid;
            XAxis.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
            _myStDevPlotModel.Axes.Add(XAxis);


            _myStDevPlotModel.LegendBorder = OxyColors.DarkGray;
            _myStDevPlotModel.LegendTitle = "LogPearsonIII Test";
            _myStDevPlotModel.LegendBackground = OxyColor.FromArgb(150, 255, 255, 255);
            _myStDevPlotModel.LegendPosition = LegendPosition.TopRight;
            _myStDevPlotModel.IsLegendVisible = false;

        }

        private void PlotStDevPlot()
        {

            _myStDevPlotModel.Series.Clear();

            //add the lines
            for (int j = 0; j < _myList[0].probabilities.Count(); j++)
            {
                LineSeries myStDevLineSeries = new LineSeries();
                myStDevLineSeries.Title = "Probability: " + _myList[0].probabilities[j].ToString();
                for (int i = 0; i < _myList.Count; i++)
                {
                    myStDevLineSeries.Points.Add(new DataPoint(_myList[i].stDev, _myList[i].percDiff[j]));
                }
                _myStDevPlotModel.Series.Add(myStDevLineSeries);

            }
            _myStDevPlotModel.IsLegendVisible = true;
            //add a red line at 0
            LineSeries zeroLine = new LineSeries();
            zeroLine.Color = OxyColors.Black;
            zeroLine.LineStyle = LineStyle.Dash;
            zeroLine.Points.Add(new DataPoint(0, 0));
            zeroLine.Points.Add(new DataPoint(2, 0));

            _myStDevPlotModel.Series.Add(zeroLine);

            percDiffOxyPlot.InvalidatePlot(true);
        }

        private void btn_FileBrowse_Click(object sender, RoutedEventArgs e)
        {
            string FileName = null;
            Microsoft.Win32.OpenFileDialog OpenFileDialog = new Microsoft.Win32.OpenFileDialog();

            //OpenFileDialog.Filter = "Excel (*.xlsx)|*.xlsx" & "| All files (*.*)|*.*";

            if (OpenFileDialog.ShowDialog() == true)
            {
                txt_filePath.Text = OpenFileDialog.FileName.ToString();
                filePath = txt_filePath.Text;
                //this.Focus();
            }
        }

        private void btn_createNewData_Click(object sender, RoutedEventArgs e)
        {
            Window createNewDataFileWindow = new UnitTests.LogP3_Fortran_vs_CSharp.createFlowData();
            createNewDataFileWindow.Show();

        }

        private void btn_plot_Click(object sender, RoutedEventArgs e)
        {



    

            string[] fieldz = txt_iterationCSV.Text.Split(',');

            int[] myInts = Array.ConvertAll(fieldz, int.Parse);

            PlotPercDiffPlot(myInts);



        }


    }

}
