using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.ViewModel.Implementations;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class DeterministicControlVM : ValidatingBaseViewModel, IValueUncertainty
    {
        private Deterministic _Deterministic;

        public event EventHandler WasModified;

        public double Value
        {
            get;
            set;
        }

        public DeterministicControlVM():this(0)
        {
        }

        public DeterministicControlVM(double value)
        {
            Value = value;
            _Deterministic = new Deterministic(Value);
        }


        public ContinuousDistribution CreateOrdinate()
        {
            return _Deterministic;
        }

        public FdaValidationResult IsValid()
        {
            //todo: is this right?
            return new FdaValidationResult();
        }
    }
}
