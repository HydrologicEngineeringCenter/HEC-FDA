using HEC.FDA.Model.compute;
using HEC.FDA.Model.extensions;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.TableWithPlot.Base;
using HEC.FDA.ViewModel.TableWithPlot.Data;
using Statistics.Distributions;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor
{
    public class ParameterEntryVM : BaseLP3Plotter
    {
        #region Backing Fields
        private HistogramDataProvider _dataProvider;
        #endregion

        #region Properties
        public HistogramDataProvider DataProvider
        {
            get { return _dataProvider; }
        } 

        public double Mean
        {
            get
            {
                return LP3Distribution.Mean;
            }
            set
            {
                LP3Distribution.Mean = value;
                NotifyPropertyChanged();
                UpdateTable();
                UpdatePlot();
            }
        }
        public double Standard_Deviation
        {
            get
            {
                return LP3Distribution.StandardDeviation;
            }
            set
            {
                LP3Distribution.StandardDeviation = value;
                NotifyPropertyChanged();
                UpdateTable();
                UpdatePlot();
            }
        }
        public double Skew
        {
            get
            {
                return LP3Distribution.Skewness;
            }
            set
            {
                LP3Distribution.Skewness = value;
                NotifyPropertyChanged();
                UpdateTable();
                UpdatePlot();
            }
        }
        public int SampleSize
        {
            get
            {
                return (int)LP3Distribution.SampleSize;
            }
            set
            {
                LP3Distribution.SampleSize = value;
                NotifyPropertyChanged();
                UpdateTable();
                base.UpdatePlot();
            }
        }
        #endregion

        #region Constructors
        public ParameterEntryVM(XElement xElement)
        {
            LoadFromXML(xElement);
            _dataProvider = new HistogramDataProvider();
            InitializePlotModel();
            Validate();
            UpdateTable();
            UpdatePlot();
        }
        public ParameterEntryVM()
        {

            LP3Distribution = new LogPearson3(3.5, 0.22, 0.1, 60);
            _dataProvider = new HistogramDataProvider();
            InitializePlotModel();
            Validate();
            UpdateTable();
            UpdatePlot();
        }
        #endregion

        #region Save and Load
        public void LoadFromXML(XElement ele)
        {
            var childs = ele.Descendants();
            var distEle = childs.First();
            LP3Distribution = (LogPearson3)Statistics.ContinuousDistribution.FromXML(distEle);
        }
        public XElement ToXML()
        {
            XElement ele = new XElement(GetType().Name);
            ele.Add(LP3Distribution.ToXML());
            return ele;
        }
        #endregion

        #region Table
        private void UpdateTable()
        {
            DataProvider.Data.Clear();
            double[] exceedenceProbs = new double[8] { 0.5, 0.2, 0.1, 0.04, 0.02, 0.01, 0.04, 0.02 };
            RandomProvider rp = new(1234);
            UncertainPairedData LP3asUPD = LP3Distribution.BootstrapToUncertainPairedData(rp, exceedenceProbs);
            DataProvider.UpdateFromUncertainPairedData(LP3asUPD);
            NotifyPropertyChanged(nameof(DataProvider));
        }
        #endregion

    }
}
