using HEC.FDA.Model.paireddata;
using OxyPlot;
using OxyPlot.Series;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class NormalDataPoint
    {
        public double Probability { get; set; }
        public double ZScore { get; set; }
        public double Value { get; set; }
        public NormalDataPoint(double probability, double zScore, double value)
        {
            Probability = probability;
            ZScore = zScore;
            Value = value;
        }
    }
}
