﻿using HEC.FDA.Model.compute;
using HEC.FDA.Model.extensions;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.TableWithPlot.Base;
using HEC.FDA.ViewModel.TableWithPlot.Data;
using Statistics.Distributions;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor
{
    public class ParameterEntryVM : BaseLP3Plotter
    {
        #region Backing Fields
        private HistogramDataProvider _confidenceLimitsDataTable;
        #endregion

        #region Properties
        public HistogramDataProvider ConfidenceLimitsDataTable
        {
            get { return _confidenceLimitsDataTable; }
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
            InitializePlotModel();
            FromXML(xElement);
            _confidenceLimitsDataTable = new HistogramDataProvider();
            Validate();
            UpdateTable();
            UpdatePlot();
        }
        public ParameterEntryVM()
        {
            InitializePlotModel();
            LP3Distribution = new LogPearson3(3.5, 0.22, 0.1, 60);
            _confidenceLimitsDataTable = new HistogramDataProvider();
            Validate();
            UpdateTable();
            UpdatePlot();
        }
        #endregion

        #region Table
        protected void UpdateTable()
        {
            ConfidenceLimitsDataTable.Data.Clear();
            double[] exceedenceProbs = new double[16] { 0.999, 0.99, 0.95, 0.9, 0.8, 0.7, 0.5, 0.2, 0.1, 0.04, 0.02, 0.01, 0.005, 0.004, 0.002, 0.001 };
            RandomProvider rp = new(1234);
            UncertainPairedData LP3asUPD = LP3Distribution.BootstrapToUncertainPairedData(rp, exceedenceProbs);
            ConfidenceLimitsDataTable.UpdateFromUncertainPairedData(LP3asUPD);
            NotifyPropertyChanged(nameof(ConfidenceLimitsDataTable));
        }
        #endregion

    }
}