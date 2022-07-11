using HEC.MVVMFramework.ViewModel.Implementations;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.TableWithPlot
{
    internal class LP3ComputeComponentVM:ValidatingBaseViewModel
    {
        #region Backing Fields
        private string _name;
        private string _xlabel = "xlabel";
        private string _ylabel = "ylabel";
        private string _description = "description";
        //LP3
        private LogPearson3 _LP3 = new LogPearson3();
        private double _mean;
        private double _skew;
        private int _sampleSize;
        private double _standardDeviation;
        ObservableCollection<Double> _observedFlows;
        #endregion

        #region Properties
        public string Units
        {
            get
            {
                return ("(" + XLabel + ", " + YLabel + ")");
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }
        public string XLabel
        {
            get
            {
                return _xlabel;
            }
            set
            {
                _xlabel = value;
                NotifyPropertyChanged();
            }
        }
        public string YLabel
        {
            get
            {
                return _ylabel;
            }
            set
            {
                _ylabel = value;
                NotifyPropertyChanged();
            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                NotifyPropertyChanged();
            }
        }
        //LP3
        public LogPearson3 LP3
        {
            get { return _LP3; }
            set { _LP3 = value; NotifyPropertyChanged(); }
        }
        public double Mean
        {
            get { return _mean; }
            set { _mean = value; NotifyPropertyChanged(); }
        }
        public double Skew
        {
            get { return _skew; }
            set { _skew = value; NotifyPropertyChanged(); }
        }
        public int SampleSize
        {
            get { return _sampleSize; }
            set { _sampleSize = value; NotifyPropertyChanged(); }
        }
        public double StandardDeviation
        {
            get { return _standardDeviation; }
            set { _standardDeviation = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region Constructors
        public LP3ComputeComponentVM( string name = "name", string xlabel = "Exceedence Probability", string ylabel = "Discharge", string description = "description")
        {
            _name = name;
            _xlabel = xlabel;
            _ylabel = ylabel;
            _description = description;
            LoadDefaultParameterValues();
        }
        public LP3ComputeComponentVM(XElement vmEle)
        {
            LoadFromXML(vmEle);
        }
        #endregion

        #region Methods
        private void LoadDefaultParameterValues()
        {
            Mean = 0.1;
            StandardDeviation = 0.01;
            Skew = 0.01;
            SampleSize = 1;
        }

        public virtual XElement ToXML()
        {
            XElement ele = new XElement(this.GetType().Name);
            ele.SetAttributeValue("Name", Name);
            return ele;
        }
        public virtual void LoadFromXML(XElement element)
        {
        }
        #endregion
    }
}

