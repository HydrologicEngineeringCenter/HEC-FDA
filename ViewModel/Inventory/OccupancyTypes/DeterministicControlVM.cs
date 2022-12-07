using HEC.FDA.ViewModel.Utilities;
using Statistics;
using Statistics.Distributions;
using System;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// This control is the VM data model for the DeterministicControl.xaml. This control allows the user to 
    /// enter a "Value" for the ratio in the occtype assets of "Content" and "Other".
    /// </summary>
    public class DeterministicControlVM : BaseViewModel, IValueUncertainty
    {
        //This class extends BaseViewModel when the other distribution controls extend ValidatingBaseViewModel.
        //This is so that I can add my own validation rule on the Value since statistics doesn't have the property
        //rule that I want for this control.
        private Deterministic _Deterministic;

        public event EventHandler WasModified;

        public double Value
        {
            get { return _Deterministic.Value; }
            set
            {
                _Deterministic.Value = value;
                NotifyPropertyChanged();
                WasModified?.Invoke(this, EventArgs.Empty);
            }
        }

        public DeterministicControlVM(double value)
        {
            _Deterministic = new Deterministic(value);
            Value = value;

            AddRule(nameof(Value), () =>
            {
                return Value <= 10 && Value >= 0;
            }, "Deterministic value ratio must be between 0 and 10");

        }

        public ContinuousDistribution CreateOrdinate()
        {
            return _Deterministic;
        }

        public FdaValidationResult IsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if(Value>10 || Value<0)
            {
                vr.AddErrorMessage("Deterministic value ratio must be between 0 and 10");
            }
            return new FdaValidationResult();
        }
    }
}
