using Statistics.Histograms;
using System;

namespace paireddata
{
    //TODO: this needs to implement an interface that allows this class and UPD to be used interchangeably 
    public class ImpactAreaStageDamageFunction
    {
        #region Fields 
        private double[] _Stages;
        private IHistogram[] _Histograms;
        private string _DamageCategory;
        private int _ImpactAreaID;

    }
}