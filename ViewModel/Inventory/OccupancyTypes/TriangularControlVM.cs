using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.ViewModel.Implementations;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class TriangularControlVM : ValidatingBaseViewModel, IValueUncertainty
    {
        public event EventHandler WasModified;
        private Triangular _Triangular;
        
        public double Min
        {
            get { return _Triangular.Min; }
            set
            {
                _Triangular.Min = value;
                NotifyPropertyChanged();
                WasModified?.Invoke(this, new EventArgs());
            }
        }
        public double Max
        {
            get { return _Triangular.Max; }
            set
            {
                _Triangular.Max = value;
                NotifyPropertyChanged();
                WasModified?.Invoke(this, new EventArgs());
            }
        }
        public double MostLikely
        {
            get { return _Triangular.MostLikely; }
            set
            {
                _Triangular.MostLikely = value;
                NotifyPropertyChanged(nameof(Min));
                WasModified?.Invoke(this, new EventArgs());
            }
        }
        public string MinLabelString { get; set; }
        public string MaxLabelString { get; set; }
        public bool DisplayMostLikely { get; set; }

        public TriangularControlVM(double mode, double min, double max, string minLabelString, string maxLabelString, bool displayMostLikely = false)
        {
            _Triangular = new Triangular(min, mode, max);
            DisplayMostLikely = displayMostLikely;
            MinLabelString = minLabelString;
            MaxLabelString = maxLabelString;

            if (displayMostLikely)
            {
                foreach (KeyValuePair<string, IPropertyRule> r in _Triangular.RuleMap)
                {
                    RuleMap.Add(r.Key, r.Value);
                }
            }
        }

        public ContinuousDistribution CreateOrdinate()
        {
            return _Triangular;
        }

        public FdaValidationResult IsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();          
            if (Max < Min)
            {
                vr.AddErrorMessage("Triangular distribution max cannot be less than min");
            }
            if (Max < MostLikely)
            {
                vr.AddErrorMessage("Triangular distribution max cannot be less than most likely");
            }
            if (Min > MostLikely)
            {
                vr.AddErrorMessage("Triangular distribution most likely cannot be less than min");
            }
            return vr;
        }

    }
}
